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

namespace FakroApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IDialogInterfaceOnDismissListener
    {
        Database database;
        SupportToolbar toolbar;
        List<Job> jobs;
        ListView jobsListView;
        JobListViewAdapter jobsListViewAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            toolbar = FindViewById<SupportToolbar>(Resource.Id.mainToolbar);
            SetSupportActionBar(toolbar);

            database = new Database();
            jobs = (List<Job>)database.GetItems(this, JOB_TABLE_NAME).Result;
            jobsListView = FindViewById<ListView>(Resource.Id.jobsListView);
            jobsListViewAdapter = new JobListViewAdapter(this, jobs);
            jobsListView.Adapter = jobsListViewAdapter;

            double monthNormsSum = 0;
            List<double> dailyNorms = new List<double>();
            var works = Work.GetWorks();
            for(int i = 1; i <= DateTime.Today.Day; i++)
            {
                var dailyJobs = jobs.Where(j => j.Date.Month == DateTime.Today.Month && j.Date.Year == DateTime.Today.Year && j.Date.Day == i);
                if(dailyJobs.Any())
                {
                    double dailyMinutes = 0;
                    foreach (var job in dailyJobs)
                    {
                        dailyMinutes += works.FirstOrDefault(w => w.Id == job.WorkId).Norm * job.Quantity;
                    }
                    dailyNorms.Add((dailyMinutes+20)/4.8);
                }
            }
            foreach (var norm in dailyNorms) monthNormsSum += norm;
            var monthlyNorm = Math.Round(monthNormsSum / dailyNorms.Count, 2);
            TextView monthNormTextView = FindViewById<TextView>(Resource.Id.monthNormTextView);
            monthNormTextView.Text = "Miesięczna norma: " + monthlyNorm + "%";
            if (monthlyNorm < 99)
            {
                monthNormTextView.SetTextColor(Android.Graphics.Color.Red);
            }

            if (!dailyNorms.Any()) monthNormTextView.Visibility = Android.Views.ViewStates.Gone;

            var addJobButton = FindViewById<ImageButton>(Resource.Id.AddJobButton);
            addJobButton.Click += (o, e) =>
            {
                Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                AddJobDialogFragment dialog_AddJob = new AddJobDialogFragment();
                dialog_AddJob.Show(transaction, "dialog_fragment");
            };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }

        public void OnDismiss(IDialogInterface dialog)
        {
            jobs = (List<Job>)database.GetItems(this, JOB_TABLE_NAME).Result;
            jobsListViewAdapter = new JobListViewAdapter(this, jobs);
            jobsListView.Adapter = jobsListViewAdapter;
        }
    }
}