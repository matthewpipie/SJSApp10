using Foundation;
using System;
using UIKit;
using Newtonsoft.Json;

namespace SJSApp10.iOS
{
    public partial class ScheduleViewController : UIViewController
    {
        public ScheduleViewController(IntPtr handle) : base(handle)
        {

        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            GetAssignmentsButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                GetAndDisplayAssignments();
            };
        }
        private void GetAndDisplayAssignments()
        {
            Object cached = SJSManager.Instance.GetAssignments(DateTime.Today, DateTime.Today, (Object o) =>
            {
                if (o == null)
                {
                    LoginViewController loginView = this.Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
                    if (loginView != null)
                    {
                        loginView.ret = () => { GetAndDisplayAssignments(); };
                        //this.NavigationController.PushViewController(loginView, true);
                        this.PresentViewController(loginView, true, () => { });
                    }

                }
                else
                {
                    ResultLabel.Text = JsonConvert.SerializeObject(o);
                }
            });
            ResultLabel.Text = JsonConvert.SerializeObject(cached);
            // temp
            ResultLabel.Text = "loadin";
        }
    }
}