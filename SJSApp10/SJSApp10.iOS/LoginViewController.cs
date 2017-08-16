using Foundation;
using System;
using UIKit;

namespace SJSApp10.iOS
{
    public partial class LoginViewController : UIViewController
    {
        public Action ret;
        public LoginViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LoginButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                SJSManager.Instance.SetCredentials(UsernameField.Text, PasswordField.Text, true, () =>
                {
                    ret();
                    this.DismissViewController(true, () => { });
                });
            };
        }
    }
}