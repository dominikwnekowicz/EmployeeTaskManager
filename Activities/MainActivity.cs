using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Runtime;
using Android.Widget;
using Plugin.Permissions;
using FakroApp.Model;
using FakroApp.Adapters;
using FakroApp.Persistance;
using static FakroApp.Persistance.Constants;
using System.Collections.Generic;
using System;
using System.Linq;
using Android.Content;
using FakroApp.Fragments;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Support.V4.Content;
using System.Globalization;
using Android.Support.V4.Widget;
using Android.Content.Res;

namespace FakroApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity, IDialogInterfaceOnDismissListener
    {

        public readonly string TAG = MAIN_ACTIVITY_TAG;

        Database database;
        SupportToolbar toolbar;
        List<Job> jobs;
        List<Work> works;
        ListView currentJobsListView;
        ListView reserveJobsListView;
        ListView mainLayoutRightDrawerWorksListView;
        CurrentJobListViewAdapter currentJobsListViewAdapter;
        ReserveJobListViewAdapter reserveJobsListViewAdapter;
        DrawerLayout drawerLayout;
        LinearLayout rightDrawer_mainLayoutLinearLayout;
        MyActionBarDrawerToggle drawerToggle;

        WorkListViewAdapter mainLayoutRightDrawerWorksListViewAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            toolbar = FindViewById<SupportToolbar>(Resource.Id.mainToolbar);
            SetSupportActionBar(toolbar);

            var mainTabLayout = FindViewById<TabLayout>(Resource.Id.mainTabLayout);
            mainTabLayout.TabSelected += MainTabLayout_TabSelected;

            database = new Database();
            currentJobsListView = FindViewById<ListView>(Resource.Id.currentJobsListView);
            reserveJobsListView = FindViewById<ListView>(Resource.Id.reserveJobsListView);
            LoadJobAdapter();

            var addJobButton = FindViewById<ImageButton>(Resource.Id.AddJobButton);
            addJobButton.Click += (o, e) =>
            {
                Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                AddJobDialogFragment dialog_AddJob = new AddJobDialogFragment();
                dialog_AddJob.Show(transaction, TAG);
            };

            //Drawers

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.mainLayoutDrawerLayout);

            drawerToggle = new MyActionBarDrawerToggle(this, drawerLayout);

            drawerLayout.AddDrawerListener(drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            drawerToggle.SyncState();

            rightDrawer_mainLayoutLinearLayout = FindViewById<LinearLayout>(Resource.Id.rightDrawer_mainLayoutLinearLayout);
            rightDrawer_mainLayoutLinearLayout.Tag = RIGHT_DRAWER_TAG;

            CountMonthlyNorm();
        }

        private void MainTabLayout_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {
            if (e.Tab.Position == 0)
            {
                currentJobsListView.Visibility = ViewStates.Visible;
                reserveJobsListView.Visibility = ViewStates.Gone;

            }
            else if (e.Tab.Position == 1)
            {
                currentJobsListView.Visibility = ViewStates.Gone;
                reserveJobsListView.Visibility = ViewStates.Visible;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }

        private void LoadJobAdapter()
        {
            jobs = (List<Job>)database.GetItems(this, JOB_TABLE_NAME).Result;
            currentJobsListViewAdapter = new CurrentJobListViewAdapter(this, jobs.Where(j => j.Type == CURRENT_JOB_TYPE).ToList());
            currentJobsListView.Adapter = currentJobsListViewAdapter;
            reserveJobsListViewAdapter = new ReserveJobListViewAdapter(this, jobs.Where(j => j.Type == RESERVE_JOB_TYPE).ToList());
            reserveJobsListView.Adapter = reserveJobsListViewAdapter;

            works = (List<Work>)database.GetItems(this, WORK_TABLE_NAME).Result;
            List<Work> latestWorks = new List<Work>();
            foreach (var work in works)
            {
                if (latestWorks.Any(w => w.WorkCode == work.WorkCode)) latestWorks[latestWorks.FindIndex(w => w.WorkCode == work.WorkCode)] = work;
                else latestWorks.Add(work);
            }
            mainLayoutRightDrawerWorksListView = FindViewById<ListView>(Resource.Id.mainLayoutRightDrawerWorksListView);
            mainLayoutRightDrawerWorksListViewAdapter = new WorkListViewAdapter(this, latestWorks, TAG);
            mainLayoutRightDrawerWorksListView.Adapter = mainLayoutRightDrawerWorksListViewAdapter;
        }

        private void CountMonthlyNorm()
        {
            double monthMinutes = 0;
            List<double> dailyMinutes = new List<double>();
            works = (List<Work>)database.GetItems(this, WORK_TABLE_NAME).Result;
            for (int i = 1; i <= DateTime.Today.Day; i++)
            {
                var dailyJobs = jobs.Where(j => j.Date.Month == DateTime.Today.Month && j.Date.Year == DateTime.Today.Year && j.Date.Day == i && j.Type == CURRENT_JOB_TYPE);
                if (dailyJobs.Any())
                {
                    double dateMinutes = 0;
                    foreach (var job in dailyJobs)
                    {
                        if(job.IsNormalized) dateMinutes += works.FirstOrDefault(w => w.Id == job.WorkId).Norm * job.Quantity;
                        else dateMinutes += Convert.ToDouble(job.Time, CultureInfo.InvariantCulture);
                    }
                    dailyMinutes.Add(dateMinutes);
                }
            }
            foreach (var minutes in dailyMinutes) monthMinutes += minutes;
            var monthlyNorm = Math.Round((monthMinutes / dailyMinutes.Count)/4.6, 2);
            TextView monthNormTextView = FindViewById<TextView>(Resource.Id.monthNormTextView);
            monthNormTextView.Text = "Miesięczna norma: " + monthlyNorm + "%";
            TextView monthTimeTextView = FindViewById<TextView>(Resource.Id.monthTimeTextView);
            monthTimeTextView.Text = Math.Round((monthMinutes / dailyMinutes.Count)/60, 2).ToString() + "h";
            monthNormTextView.Visibility = ViewStates.Visible;
            monthTimeTextView.Visibility = ViewStates.Visible;
            if ((monthMinutes / dailyMinutes.Count) < 460)
            {
                monthNormTextView.SetTextColor(Android.Graphics.Color.Red);
                monthTimeTextView.SetTextColor(Android.Graphics.Color.Red);
            }
            else
            {
                monthNormTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)));
                monthTimeTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.colorAccent)));
            }

            if (!dailyMinutes.Any())
            {
                monthNormTextView.Visibility = ViewStates.Gone;
                monthTimeTextView.Visibility = ViewStates.Gone;
            }
        }

        //Menu item selected
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    //Ensure the right drawer is closed
                    drawerLayout.CloseDrawer(rightDrawer_mainLayoutLinearLayout);
                    drawerToggle.OnOptionsItemSelected(item);
                    return true;

                case Resource.Id.action_rightDrawerMainMenu:
                    if (drawerLayout.IsDrawerOpen(rightDrawer_mainLayoutLinearLayout))
                    {
                        //Right Drawer is already open, close it
                        drawerLayout.CloseDrawer(rightDrawer_mainLayoutLinearLayout);
                    }
                    else
                    {
                        //Right Drawer is already closed, open it and just in case close left drawer
                        drawerLayout.OpenDrawer(rightDrawer_mainLayoutLinearLayout);
                        //drawerLayout.CloseDrawer(leftDrawer_mainLayoutLinearLayout);
                    }

                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //Using for drawers

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (drawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }
            else
            {
                outState.PutString("DrawerState", "Closed");
            }
            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            drawerToggle.OnConfigurationChanged(newConfig);
        }


        public void OnDismiss(IDialogInterface dialog)
        {
            LoadJobAdapter();
            CountMonthlyNorm();
        }
    }
}