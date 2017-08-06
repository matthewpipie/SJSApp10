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
        //public SJSManager ()
        //{
        //}
        /*public void Run(string password, TextView tv)
          {
          using (var wb = new WebClient()) {
          var data = new NameValueCollection();
          data["username"] = "mgiordano";
          data["password"] = password;

                wb.UploadValuesCompleted += (sender, e) => { new MainActivity().Update(tv, e.Result.Clone().ToString()); } ;

                wb.UploadValuesAsync(new System.Uri("http://httpbin.org/post"), "POST", data);
            }
        }*/

        /*private string getPath()
        {
            #if __ANDROID__
            // Just use whatever directory SpecialFolder.Personal returns
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
            #else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
            #endif
            var path = Path.Combine(libraryPath, "meems.txt");
            return path;

        }*/
        /*public string read()
        {
            return File.ReadAllText(getPath());
        }
        public void write(string str)
        {
            File.WriteAllText(getPath(), str);
        }*/
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
        public Object GetAssignments(Action<Object> action)
        {
            LoginManager.MakeAPICall("DataDirect/AssignmentCenterAssignments/?format=json&filter=2&dateStart=8%2F5%2F2017&dateEnd=8%2F5%2F2017&persona=2&statusList=&sectionList=", (Object o) => { AssignmentCache = o; action(o); });
            return AssignmentCache;
        }
        
    }
}

