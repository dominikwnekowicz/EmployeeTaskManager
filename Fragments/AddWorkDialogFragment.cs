using System;
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
    public class AddWorkDialogFragment : Android.Support.V4.App.DialogFragment
    {
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.dialog_addWork, container, false);

            var addWorkDialogButton = view.FindViewById<Button>(Resource.Id.addWorkDialogAddButton);
            addWorkDialogButton.Click += AddWorkDialogButton_Click;

            var cancelAddWorkDialogButton = view.FindViewById<Button>(Resource.Id.addWorkDialogCancelButton);
            cancelAddWorkDialogButton.Click += CancelAddWorkDialogButton_Click;


            return view;
        }

        private void CancelAddWorkDialogButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void AddWorkDialogButton_Click(object sender, EventArgs e)
        {
            Database database = new Database();

            EditText workIdEditText = view.FindViewById<EditText>(Resource.Id.addJobDialogDescriptionEditText);
            var workCode = workIdEditText.Text.Replace(".", ",");
            var works = (List<Work>)database.GetItems(this.Activity, WORK_TABLE_NAME).Result;
            var workId = works.LastOrDefault(w => w.WorkCode == workCode).Id;

            Spinner jobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.addJobDialogJobTypeSpinner);
            var jobType = (string)jobTypeSpinner.SelectedItem;
            if (jobType == GetString(Resource.String.Current)) jobType = CURRENT_JOB_TYPE;
            else if (jobType == GetString(Resource.String.Reserve)) jobType = RESERVE_JOB_TYPE;
            Job job;
            var jobs = (List<Job>)database.GetItems(this.Activity, JOB_TABLE_NAME).Result;
            if (jobs.Any(j => j.Date.Date == DateTime.Now.Date && j.WorkId == workId))
            {
                job = jobs.First(j => j.Date.Date == DateTime.Now.Date && workId == j.WorkId);

                database.UpdateItem(this.Activity, job, JOB_TABLE_NAME);
            }
            else
            {
                job = new Job { };
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