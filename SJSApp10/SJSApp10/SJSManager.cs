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
        public void SetCredentials(string username, string password, Action callback)
        {
            LoginManager.SubmitCredentials(username, password, callback);
        }
        public Object GetAssignments(Action<Object> callback)
        {
            LoginManager.MakeAPICall("DataDirect/AssignmentCenterAssignments/?format=json&filter=2&dateStart=8%2F5%2F2017&dateEnd=8%2F5%2F2017&persona=2&statusList=&sectionList=", (Object o) => 
            {
                if (o != null)
                {
                    AssignmentCache = o;
                }
                callback(o);
            });
            return AssignmentCache;
        }
        
    }
}

