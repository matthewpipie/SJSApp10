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

            GetAndDisplaySchedule();
        }
        private void GetAndDisplaySchedule()
        {
            Newtonsoft.Json.Linq.JArray cached = SJSManager.Instance.GetSchedule(new DateTime(2017, 8, 23), (Newtonsoft.Json.Linq.JArray o) =>
            {
                if (o == null)
                {
                    LoginViewController loginView = this.Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
                    if (loginView != null)
                    {
                        loginView.ret = () => { GetAndDisplaySchedule(); };
                        //this.NavigationController.PushViewController(loginView, true);
                        this.PresentViewController(loginView, true, () => { });
                    }

                }
                else
                {
                    DisplaySchedule(o);
                }
            });
            DisplaySchedule(cached);
            // temp
            //ResultLabel.Text = "loadin";
        }
        private void DisplaySchedule(Newtonsoft.Json.Linq.JArray o)
        {
            scheduleTableView.LeftEntries.Clear();
            scheduleTableView.RightEntries.Clear();
            try
            {
                foreach (var schoolClass in o.Children() )
                {
                    //var itemProperties = schoolClass.Children<Newtonsoft.Json.Linq.JProperty>();
                    string title = schoolClass["CourseTitle"].ToString();
                    string startTime = schoolClass["MyDayStartTime"].ToString();
                    string endTime = schoolClass["MyDayEndTime"].ToString();
                    string room = schoolClass["RoomNumber"].ToString();
                    //ScheduleTableView.entries.Add(startTime + " - " + endTime + " " + title + " (" + room + ")");
                    scheduleTableView.LeftEntries.Add(startTime + " - " + endTime);
                    scheduleTableView.RightEntries.Add(title + (String.IsNullOrEmpty(room) ? "" : " (" + room + ")"));
                }
                scheduleTableView.ReloadData();
            }
            catch (Exception e)
            {

            }
        }
    }
}