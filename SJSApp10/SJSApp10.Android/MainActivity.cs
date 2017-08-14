using System;
using Newtonsoft.Json;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

namespace SJSApp10.Droid
{
	[Activity (Label = "SJSApp10.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : AppCompatActivity
    {
        //int count = 1;
        public const int LOGIN_VAL = 2;
        DrawerLayout drawerLayout;

		protected override void OnCreate (Bundle bundle)
		{
            
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            // Set up slide menu

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            // Init toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();


            SJSManager.Instance.InvalidateToken();
            SJSManager.Instance.GetDayAndAnnouncement(DateTime.Today, (DayAndAnnouncement o) => { });

            MainFragment frag = new MainFragment();
            FragmentTransaction ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.fragment_container, frag);
            ft.AddToBackStack(null);
            ft.Commit();
        }
        
        
        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.schedule):
                    // React on 'Schedule' selection
                    // ...
                    break;
                case (Resource.Id.assignmentCenter):
                    break;
                case (Resource.Id.usAnnouncements):
                    break;
                case (Resource.Id.sacSuggestions):
                    //Intent browserIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(SJSManager.SAC_SUGGESTIONS_LINK));
                    //Device.OpenUri(new Uri(SJSManager.SAC_SUGGESTIONS_LINK));
                    break;
                case (Resource.Id.naviance):
                    Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(SJSManager.NAVIANCE_LINK));
                    StartActivity(browserIntent);
                    //Device.OpenUri(new Uri(SJSManager.NAVIANCE_LINK));
                    break;
            }

            // Close drawer
            
            drawerLayout.CloseDrawers();
        }
    }
}
