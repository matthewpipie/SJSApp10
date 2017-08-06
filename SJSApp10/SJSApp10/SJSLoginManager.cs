using System;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using Xamarin.Auth;

namespace SJSApp10
{
    class SJSLoginManager
    {
        public static readonly string BASE_URL = "https://sjs.myschoolapp.com/";
        public static readonly string TOKEN_CACHE_FILE = "SJSAppToken.dat";
        public static readonly string APP_NAME = "SJS App";

        private async void GetAntiAjaxToken(Action<string> action)
        {

            string urlAddress = BASE_URL + "app";
            string token = "token placeholder thingo";

            WebRequest request = WebRequest.Create(urlAddress);
            WebResponse response = await request.GetResponseAsync();

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;

            //if (response. == null)
            //{
            readStream = new StreamReader(receiveStream);
            //}
            //else
            //{
            //readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            //}

            string data = readStream.ReadToEnd();

            string[] datas = data.Split('"');
            int index = Array.IndexOf(datas, "__RequestVerificationtoken");

            token = datas[index + 4];

            response.Close();
            readStream.Close();
            //}
            action(token);

        }

        private string GetPath(string path)
        {
            #if __ANDROID__
                // Just use whatever directory SpecialFolder.Personal returns
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            #else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            #endif
            return Path.Combine(libraryPath, path);
        }

        private string token { set; get; } = null;
        public SJSLoginManager()
        {
            string path = GetPath(TOKEN_CACHE_FILE);
            token = File.Exists(path) ? File.ReadAllText(path) : null;
        }

        public async void SubmitCredentials(string username, string password, Action callback)
        {
            Account acct = new Account
            {
                Username = username
            };
            acct.Properties.Add("Password", password);
            await AccountStore.Create().SaveAsync(acct, APP_NAME);
            callback();
        }

        private string[] GetCredentials()
        {
            var acct = AccountStore.Create().FindAccountsForService(APP_NAME).FirstOrDefault();
            if (acct == null)
            {
                return null;
            }

            return new string[] { acct.Username, acct.Properties["Password"] };
        }

        private void GenerateNewToken(Action<bool> callback)
        {
            GetAntiAjaxToken(async (string resp) =>
            {
                string[] credentials = GetCredentials();
                if (credentials == null)
                {
                    callback(false);
                    return;
                }

                byte[] data = Encoding.ASCII.GetBytes($"Username={credentials[0]}&Password={credentials[1]}");

                WebRequest request = WebRequest.Create(BASE_URL + "api/SignIn");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("requestverificationtoken", resp);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                string responseContent = null;

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader sr99 = new StreamReader(stream))
                        {
                            responseContent = sr99.ReadToEnd();
                        }
                    }

                    dynamic o = JsonConvert.DeserializeObject(responseContent);
                    if (!o.LoginSuccessful)
                    {
                        callback(false);
                        return;
                    }

                    string a = response.Headers["Set-Cookie"];
                    string[] parsed1 = a.Split(',');
                    string[] parsed2 = parsed1[1].Split(';');
                    string cookie = parsed2[0];

                    token = cookie;
                    File.WriteAllText(GetPath(TOKEN_CACHE_FILE), token);
                    callback(true);
                }

            });
        }

        public async void MakeAPICall(string call, Action<Object> callback)
        {
            if (token == null)
            {
                GenerateNewToken((bool success) =>
                {
                    if (success)
                    {
                        MakeAPICall(call, callback);
                    }
                    else
                    {
                        callback(null);
                    }
                });
                return;
            }
            string[] split = call.Split('?'); // 0 is url, 1 is data
            byte[] data = Encoding.ASCII.GetBytes(split[1]);
            WebRequest request = WebRequest.Create(BASE_URL + "api/" + split[0]);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("cookie", token);
            request.ContentLength = data.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            string responseContent = null;

            using (WebResponse response = await request.GetResponseAsync())
            {

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr99 = new StreamReader(stream))
                    {
                        responseContent = sr99.ReadToEnd();
                    }
                }

                dynamic o = JsonConvert.DeserializeObject(responseContent);
                if (o.Error != null)
                {
                    //token is invalid

                    GenerateNewToken((bool success) =>
                    {
                        if (success)
                        {
                            MakeAPICall(call, callback);
                        }
                        else
                        {
                            callback(null);
                        }
                    });

                    return;
                }
                callback(o);
            }

        }
    }
}
