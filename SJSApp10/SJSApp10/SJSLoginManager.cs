using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace SJSApp10
{
    class SJSLoginManager
    {
        public static readonly string BASE_URL = "https://sjs.myschoolapp.com/";
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
        public void Run2(string username, string password, Action<string> action)
        {

        }

        private string token {
            get {
                isTokenUsed = true;
                return token;
            }
            set
            {
                token = value;
            }
        }
        private bool isTokenUsed = true;
        public SJSLoginManager()
        {
            string path = "";
            if (File.Exists(path))
            token = File.ReadAllText(path);
            if (token == null)
            {
                GenerateNewtoken(() => { });
            }
        }

        public void SubmitCredentials(string username, string password)
        {
            //TODO
        }

        private string[] GetCredentials()
        {           //TODO
            return new string[] { "", "" };
        }

        private void GenerateNewtoken(Action callback)
        {
            if (!isTokenUsed) return;
            GetAntiAjaxToken(async (string resp) =>
            {
                // TODO:
                string[] credentials = GetCredentials();

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

                using (WebResponse response = await request.GetResponseAsync())
                {

                    string a = response.Headers["Set-Cookie"];
                    string[] parsed1 = a.Split(',');
                    string[] parsed2 = parsed1[1].Split(';');
                    string cookie = parsed2[0];

                    token = cookie;
                    callback();
                }

            });
        }

        public async void MakeAPICall(string call, Action<Object> callback)
        {
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
                if (o.Error == null)
                {
                    //token is invalid
                    GenerateNewtoken(() => { MakeAPICall(call, callback); });
                    //MakeAPICall(call, callback);
                    return;
                }
                callback(o);
            }

        }
    }
}
