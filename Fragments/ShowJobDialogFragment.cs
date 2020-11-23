﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FakroApp.Model;
using FakroApp.Persistance;
using static FakroApp.Persistance.Constants;

namespace FakroApp.Fragments
{
    public class ShowJobDialogFragment : Android.Support.V4.App.DialogFragment
    {
        private readonly string TAG = "ShowJobDialogFragment";

        View view;
        Database database;
        Work choosenWork;
        List<Work> works;
        TextView showJobDialogChooseWorkTextView;
        TextView showJobDialogTitleTextView;
        TextView showJobDialogIsNormalizedTextView;
        TextView showJobDialogTimeTextView;
        TextView showJobDialogJobTypeTextView;
        TextView showJobDialogQuantityTextView;

        private Job job;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.dialog_showJob, container, false);

            database = new Database();

            var showJobDialogEditButton = view.FindViewById<Button>(Resource.Id.showJobDialogEditButton);
            showJobDialogEditButton.Click += ShowJobDialogEditButton_Click;

            var showJobDialogDeleteButton = view.FindViewById<Button>(Resource.Id.showJobDialogDeleteButton);
            showJobDialogDeleteButton.Click += ShowJobDialogDeleteButton_Click; ;

            var showJobDialogAddButton = view.FindViewById<Button>(Resource.Id.showJobDialogAddButton);
            showJobDialogAddButton.Click += ShowJobDialogAddButton_Click;

            var showJobDialogCloseButton = view.FindViewById<Button>(Resource.Id.showJobDialogCloseButton);
            showJobDialogCloseButton.Click += ShowJobDialogCloseButton_Click;

            var cancelshowJobDialogButton = view.FindViewById<Button>(Resource.Id.showJobDialogCloseButton);

            showJobDialogTitleTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogTitleTextView);

            showJobDialogJobTypeTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogJobTypeTextView);

            showJobDialogIsNormalizedTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogIsNormalizedTextView);

            showJobDialogTimeTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogTimeTextView);

            showJobDialogQuantityTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogQuantityTextView);

            showJobDialogChooseWorkTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogChooseWorkTextView);

            int jobId = Arguments.GetInt(CHOOSEN_JOB_ID_EXTRA_NAME);
            var jobs = (List<Job>)database.GetItems(this.Activity, JOB_TABLE_NAME).Result;
            job = jobs.FirstOrDefault(j => j.Id == jobId);

            works = (List<Work>)database.GetItems(this.Activity, WORK_TABLE_NAME).Result;
            choosenWork = works.FirstOrDefault(w => w.Id == job.WorkId);

            if (job.Type == CURRENT_JOB_TYPE)
            {
                showJobDialogJobTypeTextView.Text = GetString(Resource.String.Current);
                showJobDialogAddButton.Visibility = ViewStates.Gone;
                showJobDialogEditButton.Visibility = ViewStates.Visible;
            }
            else if (job.Type == RESERVE_JOB_TYPE)
            {
                showJobDialogJobTypeTextView.Text = GetString(Resource.String.Reserve);
                showJobDialogAddButton.Visibility = ViewStates.Visible;
                showJobDialogEditButton.Visibility = ViewStates.Gone;
            }

            showJobDialogQuantityTextView.Text = job.Quantity.ToString();

            var isNormalized = job.IsNormalized;
            LinearLayout showJobDialogDescriptionLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.showJobDialogDescriptionLinearLayout);
            LinearLayout showJobDialogWorkIdLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.showJobDialogWorkIdLinearLayout);
            if (isNormalized == true)
            {
                showJobDialogChooseWorkTextView.Text = choosenWork.WorkCode;
                showJobDialogIsNormalizedTextView.Text = GetString(Resource.String.IsNormalized) + ": " + GetString(Resource.String.Yes);
                showJobDialogTitleTextView.Text = choosenWork.Name;
                showJobDialogDescriptionLinearLayout.Visibility = ViewStates.Gone;
                showJobDialogWorkIdLinearLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                showJobDialogTimeTextView.Text = job.Time.Value.ToString();
                showJobDialogIsNormalizedTextView.Text = GetString(Resource.String.IsNormalized) + ": " + GetString(Resource.String.No);
                showJobDialogTitleTextView.Text = job.Description;
                showJobDialogDescriptionLinearLayout.Visibility = ViewStates.Visible;
                showJobDialogWorkIdLinearLayout.Visibility = ViewStates.Gone;
            }

            return view;
        }

        private void ShowJobDialogAddButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ShowJobDialogDeleteButton_Click(object sender, EventArgs e)
        {
            database.DeleteItem(this.Activity, job, JOB_TABLE_NAME);

            Dismiss();
        }

        private void ShowJobDialogEditButton_Click(object sender, EventArgs e)
        {
            var dialog_EditJob = new AddJobDialogFragment();
            Bundle args = new Bundle();
            args.PutInt(CHOOSEN_JOB_ID_EXTRA_NAME, job.Id);
            dialog_EditJob.Arguments = args;
            var fragmentTransaction = this.Activity.SupportFragmentManager.BeginTransaction();
            dialog_EditJob.Show(fragmentTransaction, TAG);


            Dismiss();

        }

        private void ShowJobDialogCloseButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            Activity activity = this.Activity;
            ((IDialogInterfaceOnDismissListener)activity).OnDismiss(dialog);
        }
    }
}