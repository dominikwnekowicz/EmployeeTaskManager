using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FakroApp.Fragments;
using FakroApp.Model;
using FakroApp.Persistance;
using static FakroApp.Persistance.Constants;
using FakroApp.Activities;

namespace FakroApp.Adapters
{
    class CurrentJobListViewAdapter : BaseAdapter<Job>
    {

        private Activity activity;
        private List<Job> jobs;
        private List<Work> works;
        private Database database;

        public CurrentJobListViewAdapter(Activity activity, List<Job> jobs)
        {
            this.activity = activity;
            this.jobs = jobs;
            this.database = new Database();
            this.works = (List<Work>)database.GetItems(activity, WORK_TABLE_NAME).Result;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = activity.LayoutInflater.Inflate(Resource.Layout.layout_job, null);

            var job = jobs[position];
            var work = works.FirstOrDefault(w => w.Id == job.WorkId);

            if (position == 0 || job.Date.Date != jobs[position-1].Date.Date)
            {
                TextView jobDateTextView = view.FindViewById<TextView>(Resource.Id.jobDateTextView);
                jobDateTextView.Text = job.Date.Date.ToString("dd-MM-yyyy");
                jobDateTextView.Visibility = ViewStates.Visible;

                View jobDateDividerView = view.FindViewById<View>(Resource.Id.jobDateDividerView);
                jobDateDividerView.Visibility = ViewStates.Visible;
                double sum = 0;
                foreach(var item in jobs)
                {
                    if (item.Date.Date == job.Date.Date)
                    {
                        if (item.IsNormalized) sum += (works.FirstOrDefault(w => w.Id == item.WorkId).Norm * item.Quantity);
                        else sum += item.Time.Value;
                    }
                }
                if(Math.Round(sum, 2)/60 < 7.66)
                {
                    jobDateTextView.SetTextColor(Android.Graphics.Color.Rgb(255, 0, 0));
                    jobDateTextView.Text += " (" + Math.Round((sum + 20) / 4.80, 2) + "%)";
                    jobDateDividerView.SetBackgroundColor(Android.Graphics.Color.Rgb(255, 0, 0));
                }
            }

            TextView jobNameTextView = view.FindViewById<TextView>(Resource.Id.jobNameTextView);
            string text = job.Quantity + "x ";
            if (job.IsNormalized) text += work.Name;
            else text += job.Description;

            if (text.Length > 35) text = text.Substring(0, 32) + "...";
            jobNameTextView.Text = text;

            TextView jobTimeTextView = view.FindViewById<TextView>(Resource.Id.jobTimeTextView);
            if(job.IsNormalized) jobTimeTextView.Text = Math.Round((work.Norm * job.Quantity) / 60, 2).ToString() + "h";
            else jobTimeTextView.Text = Math.Round(job.Time.Value / 60, 2).ToString() + "h";

            view.Click += (o, e) =>
            {
                var dialog_ShowJob = new ShowJobDialogFragment();
                Bundle args = new Bundle();
                args.PutInt(CHOOSEN_JOB_ID_EXTRA_NAME, job.Id);
                dialog_ShowJob.Arguments = args;
                var fragmentTransaction = ((MainActivity)activity).SupportFragmentManager.BeginTransaction();
                dialog_ShowJob.Show(fragmentTransaction, ((MainActivity)activity).TAG);
            };

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return jobs.Count;
            }
        }

        public override Job this[int position]
        {
            get
            {
                return jobs[position];
            }
        }
    }

    class CurrentJobAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}