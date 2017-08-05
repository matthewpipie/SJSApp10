using Android.Content;
using System;
using System.IO;
using System.Net;
using System.Text;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System;
using System.Net;
using System.Collections.Specialized;
using SJSApp10.Droid;

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
        private async void getVerificationToken(Action<string> action)
        {

        }
        public void Run2(string username, string password, Action<string> action)
        {


            getVerificationToken(async (string resp) =>
            {
                byte[] data = Encoding.ASCII.GetBytes($"Username={username}&Password={password}");

                WebRequest request = WebRequest.Create("");
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
                }

                action(responseContent);

            });

        }
    }
}

