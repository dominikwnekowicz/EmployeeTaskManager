using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
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
        Database database;
        Work choosenWork;
        List<Work> works;
        Button addJobDialogChooseWorkButton;
        TextView addJobDialogTitleTextView;
        CheckBox addJobDialogIsNotNormalizedCheckBox;
        bool isNormalized;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.dialog_addJob, container, false);

            database = new Database();

            var addJobDialogButton = view.FindViewById<Button>(Resource.Id.addJobDialogAddButton);
            addJobDialogButton.Click += AddJobDialogButton_Click;

            addJobDialogTitleTextView = view.FindViewById<TextView>(Resource.Id.addJobDialogTitleTextView);

            Spinner jobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.addJobDialogJobTypeSpinner);

            var cancelAddJobDialogButton = view.FindViewById<Button>(Resource.Id.addJobDialogCancelButton);
            cancelAddJobDialogButton.Click += CancelAddJobDialogButton_Click;

            addJobDialogIsNotNormalizedCheckBox = view.FindViewById<CheckBox>(Resource.Id.addJobDialogIsNotNormalizedCheckBox);
            addJobDialogIsNotNormalizedCheckBox.CheckedChange += AddJobDialogIsNotNormalizedCheckBox_CheckedChange;

            quantityNumberPicker = view.FindViewById<NumberPicker>(Resource.Id.addJobDialogQuantityNumberPicker);
            quantityNumberPicker.MaxValue = 9999;
            quantityNumberPicker.MinValue = 1;
            quantityNumberPicker.WrapSelectorWheel = false;

            addJobDialogChooseWorkButton = view.FindViewById<Button>(Resource.Id.addJobDialogChooseWorkButton);
            addJobDialogChooseWorkButton.Click += AddJobDialogChooseWorkButton_Click;


            return view;
        }

        private void AddJobDialogIsNotNormalizedCheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            isNormalized = e.IsChecked;
            LinearLayout addJobDialogDescriptionLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.addJobDialogDescriptionLinearLayout);
            LinearLayout addJobDialogWorkIdLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.addJobDialogWorkIdLinearLayout);
            if (isNormalized == true)
            {
                addJobDialogTitleTextView.Text = GetString(Resource.String.AddJob);
                addJobDialogTitleTextView.SetTextColor(Android.Graphics.Color.Black);
                addJobDialogIsNotNormalizedCheckBox.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.colorAccent)));
                addJobDialogDescriptionLinearLayout.Visibility = ViewStates.Visible;
                addJobDialogWorkIdLinearLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                addJobDialogIsNotNormalizedCheckBox.SetTextColor(Android.Graphics.Color.Black);
                addJobDialogChooseWorkButton.Text = GetString(Resource.String.Choose);
                addJobDialogDescriptionLinearLayout.Visibility = ViewStates.Gone;
                addJobDialogWorkIdLinearLayout.Visibility = ViewStates.Visible;
            }
        }

        private void AddJobDialogChooseWorkButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this.Context, typeof(WorkActivity));
            StartActivityForResult(intent, 0);
        }

        private void CancelAddJobDialogButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void AddJobDialogButton_Click(object sender, EventArgs e)
        {

            EditText addJobDialogDescriptionEditText = view.FindViewById<EditText>(Resource.Id.addJobDialogDescriptionEditText);
            var description = addJobDialogDescriptionEditText.Text;

            if (choosenWork != null || (isNormalized && !String.IsNullOrWhiteSpace(description)))
            {
                Database database = new Database();
                int? workId = null;
                if(choosenWork != null) workId = choosenWork.Id;
                var quantity = quantityNumberPicker.Value;
                Spinner jobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.addJobDialogJobTypeSpinner);
                var jobType = (string)jobTypeSpinner.SelectedItem;
                if (jobType == GetString(Resource.String.Current)) jobType = CURRENT_JOB_TYPE;
                else if (jobType == GetString(Resource.String.Reserve)) jobType = RESERVE_JOB_TYPE;

                Job job;
                var jobs = (List<Job>)database.GetItems(this.Activity, JOB_TABLE_NAME).Result;
                if (jobs.Any(j => j.Date.Date == DateTime.Now.Date && (j.WorkId == workId)))
                {
                    job = jobs.First(j => j.Date.Date == DateTime.Now.Date && workId == j.WorkId);
                    job.Quantity += quantity;

                    database.UpdateItem(this.Activity, job, JOB_TABLE_NAME);
                }
                else
                {
                    job = new Job { Date = DateTime.Now, Quantity = quantity, WorkId = workId, Type = jobType, Description = description, IsNormalized = isNormalized };
                    database.PutItem(this.Activity, job, JOB_TABLE_NAME);
                }


                Dismiss();
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            switch(requestCode)
            {
                case 0:
                    if(resultCode == -1)//Result.Ok
                    {
                        var choosenWorkId = data.GetIntExtra(CHOOSEN_WORK_ID_EXTRA_NAME, 0);
                        works = (List<Work>)database.GetItems(this.Activity, WORK_TABLE_NAME).Result;
                        choosenWork = works.Last(w => w.Id == choosenWorkId);
                        addJobDialogTitleTextView.Text = choosenWork.Name;
                        addJobDialogTitleTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.colorPrimaryDark)));
                        addJobDialogChooseWorkButton.Text = GetString(Resource.String.Change);
                    }
                    return;
            }
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            Activity activity = this.Activity;
            ((IDialogInterfaceOnDismissListener)activity).OnDismiss(dialog);
        }
    }
}