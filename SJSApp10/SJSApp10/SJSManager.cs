//using System.Collections.Generic;
using Xamarin.Auth;
using Newtonsoft.Json;
using System.Text;
using System;
//using System.Collections.Specialized;

namespace SJSApp10
{
    public class SJSManager
    {
        // Constants
        public const string SAC_SUGGESTIONS_LINK = "https://docs.google.com/a/sjs.org/forms/d/1bBOdqDCeVLpvA3GkW1WzxvkZhLm8NkHg-IXZP8jUo9I/viewform";
        public const string NAVIANCE_LINK = "https://connection.naviance.com/family-connection/auth/login/?hsid=sjs";

        // Private vars
        private SJSLoginManager LoginManager;

        private Object AssignmentCache { get; set; }
        private Object ScheduleCache { get; set; }
        private DateTime ScheduleCacheDate { get; set; }

        // Constructor
        private SJSManager() {
            LoginManager = new SJSLoginManager();
        }

        // Singleton setup
        private static SJSManager instance;
        public static SJSManager Instance {
            get
            {
                if (instance == null)
                {
                    instance = new SJSManager();
                }
                return instance;
            }
        }

        // Methods
        public void SetCredentials(string username, string password, bool clearToken, Action callback)
        {
            LoginManager.SubmitCredentials(username, password, clearToken, callback);
        }
        public Object GetSchedule(DateTime day, Action<Object> callback)
        {
            string date = day.Month.ToString() + "%2F" + day.Day.ToString() + "%2F" + day.Year.ToString();
            LoginManager.MakeAPICall("schedule/MyDayCalendarStudentList/?scheduleDate=" + date + "&personaId=2", (Object o) =>
            {
                if (o != null)
                {
                    ScheduleCache = o;
                    ScheduleCacheDate = day;
                }
                callback(o);
            });
            return (ScheduleCacheDate == day) ? ScheduleCache : null;
        }
        public Object GetAssignments(DateTime start, DateTime end, Action<Object> callback)
        {
            // %2F is the HTML encoding for a slash (/) which for some really weird reason myschoolapp uses
            // They also use M-D-YYYY which is just really strange and not standard at all
            string startDate = start.Month.ToString() + "%2F" + start.Day.ToString() + "%2F" + start.Year.ToString();
            string endDate = end.Month.ToString() + "%2F" + end.Day.ToString() + "%2F" + end.Year.ToString();
            LoginManager.MakeAPICall("DataDirect/AssignmentCenterAssignments/?format=json&filter=2&dateStart=" + startDate + "&dateEnd=" + endDate + "&persona=2&statusList=&sectionList=", (Object o) => 
            {
                if (o != null)
                {
                    AssignmentCache = o;
                }
                callback(o);
            });
            return AssignmentCache;
        }
        




        // TEMPORARY METHODS ONLY USED IN TESTING
        public void InvalidateToken()
        {
            LoginManager.InvalidateToken();
        }
        public void DeleteToken()
        {
            LoginManager.DeleteToken();
        }
    }
}

