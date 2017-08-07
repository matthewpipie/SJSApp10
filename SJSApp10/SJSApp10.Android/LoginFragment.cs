using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SJSApp10.Droid
{
    public class LoginFragment : DialogFragment
    {
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);
            LayoutInflater inflater = this.Activity.LayoutInflater;
            builder.SetView(inflater.Inflate(Resource.Layout.LoginFragmentLayout, null))
                .SetPositiveButton("SIGN IN", (senderAlert, args) =>
                {
                    Toast.MakeText(this.Activity, this.Dialog.FindViewById<EditText>(Resource.Id.username).Text, ToastLength.Short).Show();
                    string username = this.Dialog.FindViewById<EditText>(Resource.Id.username).Text;
                    string password = this.Dialog.FindViewById<EditText>(Resource.Id.password).Text;
                    SJSManager.Instance.SetCredentials(username, password, true, () =>
                    {
                        Intent resultIntent = new Intent();
                        resultIntent.PutExtra("username", username);
                        resultIntent.PutExtra("password", password);
                        this.Activity.SetResult(Result.Ok, resultIntent);
                    });

                })
                .SetNegativeButton("CANCEL", (senderAlert, args) =>
                {
                    Toast.MakeText(this.Activity, this.Activity.FindViewById<EditText>(Resource.Id.password).Text, ToastLength.Short).Show();
                });
            return builder.Create();
        }

    }
}