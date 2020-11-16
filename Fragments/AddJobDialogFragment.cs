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

            var addJobDialogButton = view.FindViewById<Button>(Resource.Id.AddJobDialogButton);
            addJobDialogButton.Click += AddJobDialogButton_Click;

            Spinner jobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.JobTypeSpinner);

            var cancelAddJobDialogButton = view.FindViewById<Button>(Resource.Id.CancelAddJobDialogButton);
            cancelAddJobDialogButton.Click += CancelAddJobDialogButton_Click;

            quantityNumberPicker = view.FindViewById<NumberPicker>(Resource.Id.QuantityNumberPicker);
            quantityNumberPicker.MaxValue = 9999;
            quantityNumberPicker.MinValue = 1;
            quantityNumberPicker.WrapSelectorWheel = false;


            return view;
        }

        private void CancelAddJobDialogButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void AddJobDialogButton_Click(object sender, EventArgs e)
        {
            Database database = new Database();

            EditText workIdEditText = view.FindViewById<EditText>(Resource.Id.WorkIdEditText);
            var workId = workIdEditText.Text.Replace(".", ",");

            var quantity = quantityNumberPicker.Value;

            Spinner jobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.JobTypeSpinner);
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