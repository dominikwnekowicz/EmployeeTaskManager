using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views;
using Android.Widget;
using FakroApp.Model;
using FakroApp.Adapters;
using FakroApp.Persistance;
using static FakroApp.Persistance.Constants;
using Refractored.Fab;
using FakroApp.Fragments;

namespace FakroApp.Activities
{
    [Activity(Label = "WorkActivity")]
    public class WorkActivity : AppCompatActivity, IDialogInterfaceOnDismissListener
    {
        const string TAG = WORK_ACTIVITY_TAG;

        SupportToolbar toolbar;
        ListView worksListView;
        List<Work> works;
        WorkListViewAdapter worksListViewAdapter;
        Database database;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_work);
            SetResult(Result.Canceled);

            toolbar = FindViewById<SupportToolbar>(Resource.Id.workToolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            // Create your application here


            database = new Database();
            works = (List<Work>)database.GetItems(this, WORK_TABLE_NAME).Result;
            List<Work> latestWorks = new List<Work>();
            foreach(var work in works)
            {
                if (latestWorks.Any(w => w.WorkCode == work.WorkCode)) latestWorks[latestWorks.FindIndex(w => w.WorkCode == work.WorkCode)] = work;
                else latestWorks.Add(work);
            }
            worksListView = FindViewById<ListView>(Resource.Id.worksListView);
            worksListViewAdapter = new WorkListViewAdapter(this, latestWorks, TAG);
            worksListView.Adapter = worksListViewAdapter;

            var workFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.workFloatingActionButton);
            workFloatingActionButton.Click += WorkFloatingActionButton_Click;
        }

        private void WorkFloatingActionButton_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            AddWorkDialogFragment dialog_AddWork = new AddWorkDialogFragment();
            dialog_AddWork.Show(transaction, TAG);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            switch(id)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            works = (List<Work>)database.GetItems(this, WORK_TABLE_NAME).Result;
            worksListViewAdapter = new WorkListViewAdapter(this, works, TAG);
            worksListView.Adapter = worksListViewAdapter;
        }
    }
}