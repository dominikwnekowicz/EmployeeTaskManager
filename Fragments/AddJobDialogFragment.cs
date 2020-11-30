using System;
using System.Collections.Generic;
using System.Globalization;
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
        NumberPicker addJobDialogQuantityNumberPicker;
        Database database;
        Work choosenWork;
        List<Work> works;
        Button addJobDialogChooseWorkButton;
        TextView addJobDialogTitleTextView;
        EditText addJobDialogTimeEditText;
        CheckBox addJobDialogIsNormalizedCheckBox;
        Spinner addJobDialogJobTypeSpinner;

        Job job;
        bool isNormalized = true;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.dialog_addJob, container, false);

            database = new Database();

            var addJobDialogAddButton = view.FindViewById<Button>(Resource.Id.addJobDialogAddButton);
            addJobDialogAddButton.Click += AddJobDialogAddButton_Click;

            addJobDialogTitleTextView = view.FindViewById<TextView>(Resource.Id.addJobDialogTitleTextView);

            addJobDialogTimeEditText = view.FindViewById<EditText>(Resource.Id.addJobDialogTimeEditText);

            EditText addJobDialogDescriptionEditText = view.FindViewById<EditText>(Resource.Id.addJobDialogDescriptionEditText);

            addJobDialogJobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.addJobDialogJobTypeSpinner);
            addJobDialogJobTypeSpinner.ItemSelected += AddJobDialogJobTypeSpinner_ItemSelected;

            var addJobDialogCancelButton = view.FindViewById<Button>(Resource.Id.addJobDialogCancelButton);
            addJobDialogCancelButton.Click += AddJobDialogCancelButton_Click;

            addJobDialogIsNormalizedCheckBox = view.FindViewById<CheckBox>(Resource.Id.addJobDialogIsNormalizedCheckBox);
            addJobDialogIsNormalizedCheckBox.CheckedChange += AddJobDialogIsNormalizedCheckBox_CheckedChange;

            addJobDialogQuantityNumberPicker = view.FindViewById<NumberPicker>(Resource.Id.addJobDialogQuantityNumberPicker);
            addJobDialogQuantityNumberPicker.MaxValue = 9999;
            addJobDialogQuantityNumberPicker.MinValue = 1;
            addJobDialogQuantityNumberPicker.WrapSelectorWheel = false;

            addJobDialogChooseWorkButton = view.FindViewById<Button>(Resource.Id.addJobDialogChooseWorkButton);
            addJobDialogChooseWorkButton.Click += AddJobDialogChooseWorkButton_Click;
            
            if(Arguments != null)
            {
                int jobId = Arguments.GetInt(CHOOSEN_JOB_ID_EXTRA_NAME);
                addJobDialogAddButton.Text = GetString(Resource.String.Save);
                var jobs = (List<Job>)database.GetItems(this.Activity, JOB_TABLE_NAME).Result;
                job = jobs.FirstOrDefault(j => j.Id == jobId);
                SelectWork(job.WorkId);

                if (job.Type == CURRENT_JOB_TYPE)
                {
                    addJobDialogJobTypeSpinner.SetSelection(0);
                }
                else if (job.Type == RESERVE_JOB_TYPE)
                {
                    addJobDialogJobTypeSpinner.SetSelection(1);
                }

                addJobDialogQuantityNumberPicker.Value = job.Quantity;

                addJobDialogDescriptionEditText.Text = job.Description;

                var isNormalized = job.IsNormalized;

                if(job.Time.HasValue) addJobDialogTimeEditText.Text = job.Time.Value.ToString(); ;  
            }
            
            return view;
        }

        private void AddJobDialogJobTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch(e.Position)
            {
                case 0:
                    addJobDialogIsNormalizedCheckBox.Enabled = true;
                    return;
                case 1:
                    addJobDialogIsNormalizedCheckBox.Enabled = false;
                    addJobDialogIsNormalizedCheckBox.Selected = true;
                    return;
                default:
                    return;

            }
        }

        private void AddJobDialogIsNormalizedCheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            isNormalized = e.IsChecked;
            LinearLayout addJobDialogDescriptionLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.addJobDialogDescriptionLinearLayout);
            LinearLayout addJobDialogWorkIdLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.addJobDialogWorkIdLinearLayout);
            LinearLayout addJobDialogTimeLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.addJobDialogTimeLinearLayout);
            if (isNormalized == true)
            {
                addJobDialogIsNormalizedCheckBox.SetTextColor(Android.Graphics.Color.Black);
                addJobDialogChooseWorkButton.Text = GetString(Resource.String.Choose);
                addJobDialogDescriptionLinearLayout.Visibility = ViewStates.Gone;
                addJobDialogWorkIdLinearLayout.Visibility = ViewStates.Visible;
                addJobDialogTimeLinearLayout.Visibility = ViewStates.Gone;
                addJobDialogJobTypeSpinner.Enabled = true;
            }
            else
            {
                addJobDialogTitleTextView.Text = GetString(Resource.String.AddJob);
                addJobDialogTitleTextView.SetTextColor(Android.Graphics.Color.Black);
                addJobDialogIsNormalizedCheckBox.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.colorAccent)));
                addJobDialogDescriptionLinearLayout.Visibility = ViewStates.Visible;
                addJobDialogWorkIdLinearLayout.Visibility = ViewStates.Gone;
                addJobDialogTimeLinearLayout.Visibility = ViewStates.Visible;
                addJobDialogJobTypeSpinner.SetSelection(0);
                addJobDialogJobTypeSpinner.Enabled = false;
            }
        }

        private void AddJobDialogChooseWorkButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this.Context, typeof(WorkActivity));
            StartActivityForResult(intent, 0);
        }

        private void AddJobDialogCancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }


        private string ChangeSymbols(string text)
        {
            foreach (var character in text)
            {
                if (!Char.IsNumber(character) && character != '.') text = text.Replace(character, '.');
            }
            return text;
        }
        private void AddJobDialogAddButton_Click(object sender, EventArgs e)
        {

            EditText addJobDialogDescriptionEditText = view.FindViewById<EditText>(Resource.Id.addJobDialogDescriptionEditText);
            var description = addJobDialogDescriptionEditText.Text;

            if (choosenWork != null || (!isNormalized && !String.IsNullOrWhiteSpace(description)))
            {
                int? workId = null;
                if(choosenWork != null || (job != null && job.WorkId != choosenWork.Id)) workId = choosenWork.Id;
                var quantity = addJobDialogQuantityNumberPicker.Value;
                addJobDialogJobTypeSpinner = view.FindViewById<Spinner>(Resource.Id.addJobDialogJobTypeSpinner);
                var jobType = (string)addJobDialogJobTypeSpinner.SelectedItem;
                if (jobType == GetString(Resource.String.Current)) jobType = CURRENT_JOB_TYPE;
                else if (jobType == GetString(Resource.String.Reserve)) jobType = RESERVE_JOB_TYPE;
                double? time = null;

                if (!String.IsNullOrWhiteSpace(addJobDialogTimeEditText.Text)) time = Convert.ToDouble(ChangeSymbols(addJobDialogTimeEditText.Text), CultureInfo.InvariantCulture);

                var jobs = (List<Job>)database.GetItems(this.Activity, JOB_TABLE_NAME).Result;
                if ((jobType == CURRENT_JOB_TYPE && jobs.Any(j => j.Date.Date == DateTime.Now.Date && (j.WorkId == workId) && j.Type == jobType)) || jobType == RESERVE_JOB_TYPE && jobs.Any(j => j.WorkId == workId && j.Type == jobType))
                {
                    if (job == null)
                    {
                        if(jobType == CURRENT_JOB_TYPE) job = jobs.First(j => j.Date.Date == DateTime.Now.Date && workId == j.WorkId && j.Type == jobType);
                        else job = jobs.First(j => workId == j.WorkId && j.Type == jobType);
                        job.Quantity += quantity;
                    }
                    else job.Quantity = quantity;
                    job.Time = time;
                    job.IsNormalized = isNormalized;
                    job.Type = jobType;
                    job.WorkId = workId;

                    database.UpdateItem(this.Activity, job, JOB_TABLE_NAME);
                }
                else
                {
                    job = new Job { Date = DateTime.Now, Quantity = quantity, WorkId = workId, Type = jobType, Description = description, IsNormalized = isNormalized, Time = time };
                    database.PutItem(this.Activity, job, JOB_TABLE_NAME);
                }


                Dismiss();
            }
        }

        private void SelectWork(int? choosenWorkId)
        {
            works = (List<Work>)database.GetItems(this.Activity, WORK_TABLE_NAME).Result;
            choosenWork = works.Last(w => w.Id == choosenWorkId);
            addJobDialogTitleTextView.Text = choosenWork.Name;
            addJobDialogTitleTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.colorPrimaryDark)));
            addJobDialogChooseWorkButton.Text = GetString(Resource.String.Change);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            switch(requestCode)
            {
                case 0:
                    if(resultCode == -1)//Result.Ok
                    {
                        var choosenWorkId = data.GetIntExtra(CHOOSEN_WORK_ID_EXTRA_NAME, 0);
                        SelectWork(choosenWorkId);
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