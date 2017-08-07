using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SJSApp10.Droid
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			SetContentView (Resource.Layout.Login);

            // Create your application here
            FindViewById<Button>(Resource.Id.submit).Click += delegate
            {
                string username = FindViewById<EditText>(Resource.Id.username).Text;
                string password = FindViewById<EditText>(Resource.Id.password).Text;
                SJSManager.Instance.SetCredentials(username, password, true, () =>
                {
                    Intent resultIntent = new Intent();
                    resultIntent.PutExtra("username", username);
                    resultIntent.PutExtra("password", password);
                    SetResult(Result.Ok, resultIntent);
                    Finish();
                });
            };
        }
    }
}