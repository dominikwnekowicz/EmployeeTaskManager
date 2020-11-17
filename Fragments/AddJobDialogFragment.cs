﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FakroApp.Activities;
using FakroApp.Model;
using FakroApp.Persistance;
using static FakroApp.Persistance.Constants;

namespace FakroApp.Fragments
{
    class AddJobDialogFragment : Android.Support.V4.App.DialogFragment
    {
        View view;
        NumberPicker quantityNumberPicker;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.dialog_addJob, container, false);

            var addJobDialogButton = view.FindViewById<Button>(Resource.Id.addJobDialogAddButton);
            addJobDialogButton.Click += AddJobDialogButton_Click;

            Spinner jobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.addJobDialogJobTypeSpinner);

            var cancelAddJobDialogButton = view.FindViewById<Button>(Resource.Id.addJobDialogCancelButton);
            cancelAddJobDialogButton.Click += CancelAddJobDialogButton_Click;

            quantityNumberPicker = view.FindViewById<NumberPicker>(Resource.Id.addJobDialogQuantityNumberPicker);
            quantityNumberPicker.MaxValue = 9999;
            quantityNumberPicker.MinValue = 1;
            quantityNumberPicker.WrapSelectorWheel = false;

            Button addJobDialogChooseWorkButton = view.FindViewById<Button>(Resource.Id.addJobDialogChooseWorkButton);
            addJobDialogChooseWorkButton.Click += AddJobDialogChooseWorkButton_Click;


            return view;
        }

        private void AddJobDialogChooseWorkButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this.Context, typeof(WorksActivity));
            StartActivity(intent);
        }

        private void CancelAddJobDialogButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void AddJobDialogButton_Click(object sender, EventArgs e)
        {
            Database database = new Database();

            EditText workIdEditText = view.FindViewById<EditText>(Resource.Id.addJobDialogDescriptionEditText);
            var workCode = workIdEditText.Text.Replace(".", ",");
            var works = (List<Work>)database.GetItems(this.Activity, WORK_TABLE_NAME).Result;
            var workId = works.LastOrDefault(w => w.WorkCode == workCode).Id;
            var quantity = quantityNumberPicker.Value;

            Spinner jobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.addJobDialogJobTypeSpinner);
            var jobType = (string)jobTypeSpinner.SelectedItem;
            if (jobType == GetString(Resource.String.Current)) jobType = CURRENT_JOB_TYPE;
            else if (jobType == GetString(Resource.String.Reserve)) jobType = RESERVE_JOB_TYPE;
            Job job;
            var jobs = (List<Job>)database.GetItems(this.Activity, JOB_TABLE_NAME).Result;
            if (jobs.Any(j => j.Date.Date == DateTime.Now.Date && j.WorkId == workId))
            {
                job = jobs.First(j => j.Date.Date == DateTime.Now.Date && workId == j.WorkId);
                job.Quantity += quantity;

                database.UpdateItem(this.Activity, job, JOB_TABLE_NAME);
            }
            else
            {
                job = new Job { Date = DateTime.Now, Quantity = quantity, WorkId = workId, Type = jobType };
                database.PutItem(this.Activity, job, JOB_TABLE_NAME);
            }


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