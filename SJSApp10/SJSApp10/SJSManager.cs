//using System.Collections.Generic;
using Xamarin.Auth;
using System.IO;
using System.Net;
using System.Text;
using System;
//using System.Collections.Specialized;

namespace SJSApp10
{
    public class SJSManager
    {
        public SJSManager ()
        {
        }
        /*public void Run(string password, TextView tv)
          {
          using (var wb = new WebClient()) {
          var data = new NameValueCollection();
          data["username"] = "mgiordano";
          data["password"] = password;

                wb.UploadValuesCompleted += (sender, e) => { new MainActivity().Update(tv, e.Result.Clone().ToString()); } ;

                wb.UploadValuesAsync(new System.Uri("http://httpbin.org/post"), "POST", data);
            }
        }*/
        private async void GetVerificationToken(Action<string> action)
        {

            string urlAddress = "https://sjs.myschoolapp.com/app";
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
        private string getPath()
        {
            #if __ANDROID__
            // Just use whatever directory SpecialFolder.Personal returns
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
            #else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
            #endif
            var path = Path.Combine(libraryPath, "meems.txt");
            return path;

        }
        public string read()
        {
            return File.ReadAllText(getPath());
        }
        public void write(string str)
        {
            File.WriteAllText(getPath(), str);
        }
        public void Run2(string username, string password, Action<string> action)
        {


            GetVerificationToken(async (string resp) =>
            {
                byte[] data = Encoding.ASCII.GetBytes($"Username={username}&Password={password}");

                WebRequest request = WebRequest.Create("https://sjs.myschoolapp.com/api/SignIn");
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
                    
                    
                    string a = response.Headers["Set-Cookie"];
                    string[] parsed1 = a.Split(',');
                    string[] parsed2 = parsed1[1].Split(';');
                    string cookie = parsed2[0];
                    action(cookie);
                }

                //action(responseContent);

            });

        }
    }
}

