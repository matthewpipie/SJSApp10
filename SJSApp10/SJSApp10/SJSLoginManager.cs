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
            int index = Array.IndexOf(datas, "__RequestVerificationToken");

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

        private string Token { set; get; } = null;
        public SJSLoginManager()
        {
            string path = GetPath(TOKEN_CACHE_FILE);
            Token = File.Exists(path) ? File.ReadAllText(path) : null;
        }

        public async void SubmitCredentials(string username, string password, bool clearToken, Action callback)
        {
            // Delete previous credentials
            var account = AccountStore.Create().FindAccountsForService(APP_NAME).FirstOrDefault();
            while (account != null)
            {
                await AccountStore.Create().DeleteAsync(account, APP_NAME);
                account = AccountStore.Create().FindAccountsForService(APP_NAME).FirstOrDefault();
            }

            // Store new account info
            Account acct = new Account
            {
                Username = username
            };
            acct.Properties.Add("Password", password);
            await AccountStore.Create().SaveAsync(acct, APP_NAME);
            if (clearToken)
            {
                Token = null;
            }
            callback();
        }

        public static string[] GetCredentials()
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
                    // dont ask me why but u need this for it not to crash
                    bool pls = o.LoginSuccessful;
                    if (!pls)
                    {
                        callback(false);
                        return;
                    }

                    string a = response.Headers["Set-Cookie"];
                    string[] parsed1 = a.Split(',');
                    string[] parsed2 = parsed1[1].Split(';');
                    string cookie = parsed2[0];

                    Token = cookie;
                    File.WriteAllText(GetPath(TOKEN_CACHE_FILE), Token);
                    callback(true);
                }

            });
        }

        public async void MakeAPICall(string call, Action<dynamic> callback)
        {
            if (Token == null)
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
            HttpWebRequest request;
            //string call = Uri.EscapeDataString(callf);
            /*if (method == HTTPMethod.POST)
            {
                string[] split = call.Split('?'); // 0 is url, 1 is data
                byte[] data = Encoding.ASCII.GetBytes(split[1]);
                request = (HttpWebRequest)WebRequest.Create(BASE_URL + "api/" + split[0]);
                request.ContentLength = data.Length;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            else
            {*/
                request = (HttpWebRequest)WebRequest.Create(BASE_URL + "api/" + call);
                request.Method = "GET";
                //request.ContentType = "application/json; charset=utf-8";
                //request.ContentLength = 0;
                //request.Accept = "*/*";
                //request.Host = "sjs.myschoolapp.com";
                //request.Accept = "*/*";
                //request.Headers.Add("Accept-Encoding", "deflate, gzip");
                //request.AutomaticDecompression = DecompressionMethods.GZip;
                //request.UserAgent = "test test test test";
                //request.UseDefaultCredentials = true;
            //}
            request.Headers.Add("Cookie", Token);
            

            string responseContent = null;
            dynamic o = null;

            bool fail = false;
            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader sr99 = new StreamReader(stream))
                        {
                            responseContent = sr99.ReadToEnd();
                        }
                    }
                    o = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    try
                    {
                        if (o.Error != null)
                        {
                            fail = true;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            catch (Exception e)
            {
                fail = true;
            }
            if (fail)
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
            else
            {
                callback(o);
            }
        }




        // TEMPORARY METHODS ONLY USED IN TESTING
        public void InvalidateToken()
        {
            Token = "1234";
        }
        public void DeleteToken()
        {
            Token = null;
        }
    }
}
