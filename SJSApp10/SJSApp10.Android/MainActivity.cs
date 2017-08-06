using System;
using Newtonsoft.Json;
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
        public const int LOGIN_VAL = 2;

		protected override void OnCreate (Bundle bundle)
		{
            
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			//Button submit = FindViewById<Button> (Resource.Id.submit);
			Button getAssignments = FindViewById<Button> (Resource.Id.assign);
			//Button read = FindViewById<Button> (Resource.Id.read);
			//Button write = FindViewById<Button> (Resource.Id.write);
			
			getAssignments.Click += delegate {
                //button.Text = string.Format ("{0} clicks!", count++);
                //string username = FindViewById<EditText>(Resource.Id.username).Text;
                //string password = FindViewById<EditText>(Resource.Id.password).Text;
                //new SJSManager().Run(pass, FindViewById<TextView>(Resource.Id.response));
                //new SJSManager().Run2(username, password, (string resp) => { FindViewById<TextView>(Resource.Id.response).Text = resp; });
                //SJSManager.Instance.SetCredentials(username, password, true, () =>
                //{

                //});
                GetAndDisplayAssignments();
                
                
			};
            FindViewById<Button>(Resource.Id.storeTrash).Click += delegate
            {
                SJSManager.Instance.SetCredentials("", "", false, () => { });
            };
            FindViewById<Button>(Resource.Id.storeTrash2).Click += delegate
            {
                SJSManager.Instance.SetCredentials("", "", true, () => { });
            };
            FindViewById<Button>(Resource.Id.deleteToken).Click += delegate
            {
                SJSManager.Instance.DeleteToken();
            };
            FindViewById<Button>(Resource.Id.inval).Click += delegate
            {
                SJSManager.Instance.InvalidateToken();
            };
            /*write.Click += delegate
            {
                //new SJSManager().write(FindViewById<TextView>(Resource.Id.response).Text);
                FindViewById<TextView>(Resource.Id.response2).Text = "done written it";
            };
            read.Click += delegate
            {
                //FindViewById<TextView>(Resource.Id.response2).Text = new SJSManager().read();
            };*/

        }
        //public void Update(TextView tv, string res)
        //{
        //tv.Text = res;
        //}
        private void GetAndDisplayAssignments()
        {
            Object cached = SJSManager.Instance.GetAssignments((Object o) =>
            {
                if (o == null)
                {
                    Intent i = new Intent(this, typeof(LoginActivity));    
                    StartActivityForResult(i, LOGIN_VAL);
                }
                else
                {
                    FindViewById<TextView>(Resource.Id.response).Text = JsonConvert.SerializeObject(o);
                }
            });
            FindViewById<TextView>(Resource.Id.response).Text = JsonConvert.SerializeObject(cached);
            // temp
            FindViewById<TextView>(Resource.Id.response).Text = "loadin";
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (requestCode)
            {
                case (LOGIN_VAL):
                    //if (resultCode == Result.Ok)
                    //{
                        GetAndDisplayAssignments();
                    //}
                    break;
                default:
                    break;
            }
        }
    }
}
