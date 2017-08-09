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
        // Private vars
        private SJSLoginManager LoginManager;
        private Object AssignmentCache { get; set; }

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

