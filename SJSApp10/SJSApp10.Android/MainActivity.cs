using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SJSApp10.Droid
{
	[Activity (Label = "SJSApp10.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        //int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
            
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.button);
			
			button.Click += delegate {
                //button.Text = string.Format ("{0} clicks!", count++);
                string username = FindViewById<EditText>(Resource.Id.username).Text;
                string password = FindViewById<EditText>(Resource.Id.password).Text;
                //new SJSManager().Run(pass, FindViewById<TextView>(Resource.Id.response));
                new SJSManager().Run2(username, password, (string resp) => { FindViewById<TextView>(Resource.Id.response).Text = resp; });
                
			};
		}
        //public void Update(TextView tv, string res)
        //{
            //tv.Text = res;
        //}
	}
}


