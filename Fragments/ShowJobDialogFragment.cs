using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using FakroApp.Activities;
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
        TextView showJobDialogQuantityTextView;
        NumberPicker showJobDialogAddQuantityNumberPicker;
        List<Job> jobs;
        int needToday = 0;
        private Job job;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.dialog_showJob, container, false);

            database = new Database();

            int jobId = Arguments.GetInt(CHOOSEN_JOB_ID_EXTRA_NAME);
            jobs = (List<Job>)database.GetItems(this.Activity, JOB_TABLE_NAME).Result;
            job = jobs.FirstOrDefault(j => j.Id == jobId);

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

            showJobDialogIsNormalizedTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogIsNormalizedTextView);

            var showJobDialogNeedTodayTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogNeedTodayTextView);

            showJobDialogTimeTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogTimeTextView);

            showJobDialogQuantityTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogQuantityTextView);

            showJobDialogChooseWorkTextView = view.FindViewById<TextView>(Resource.Id.showJobDialogChooseWorkTextView);

            showJobDialogAddQuantityNumberPicker = view.FindViewById<NumberPicker>(Resource.Id.showJobDialogAddQuantityNumberPicker);
            showJobDialogAddQuantityNumberPicker.MaxValue = job.Quantity;
            showJobDialogAddQuantityNumberPicker.MinValue = 1;
            showJobDialogAddQuantityNumberPicker.WrapSelectorWheel = false;
            showJobDialogAddQuantityNumberPicker.ValueChanged += ShowJobDialogAddQuantityNumberPicker_ValueChanged;

            works = (List<Work>)database.GetItems(this.Activity, WORK_TABLE_NAME).Result;
            choosenWork = works.FirstOrDefault(w => w.Id == job.WorkId);
            LinearLayout showJobDialogAddQuantityLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.showJobDialogAddQuantityLinearLayout);
            LinearLayout showJobDialogIsNormalizedLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.showJobDialogIsNormalizedLinearLayout);
            LinearLayout showJobDialogNeedTodayLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.showJobDialogNeedTodayLinearLayout);

            if (job.Type == CURRENT_JOB_TYPE)
            {
                showJobDialogAddButton.Visibility = ViewStates.Gone;
                showJobDialogAddQuantityLinearLayout.Visibility = ViewStates.Gone;
                showJobDialogNeedTodayLinearLayout.Visibility = ViewStates.Gone;
                showJobDialogEditButton.Visibility = ViewStates.Visible;
                showJobDialogIsNormalizedLinearLayout.Visibility = ViewStates.Visible;
            }
            else if (job.Type == RESERVE_JOB_TYPE)
            {
                var dailyTime = DataManager.GetDailyMinutes(this.Activity);
                needToday = Convert.ToInt32(Math.Ceiling((460 - dailyTime) / choosenWork.Norm));
                if (dailyTime < 460)
                {
                    if (needToday <= job.Quantity) showJobDialogAddQuantityNumberPicker.Value = Convert.ToInt32(Math.Ceiling((460 - dailyTime) / choosenWork.Norm));
                    else
                    {
                        showJobDialogAddQuantityNumberPicker.Value = job.Quantity;
                    }
                    showJobDialogNeedTodayTextView.Text = needToday.ToString();
                }
                else
                {
                    showJobDialogNeedTodayTextView.Text = "0";
                }

                if (showJobDialogAddQuantityNumberPicker.MaxValue < needToday) showJobDialogAddQuantityNumberPicker.SetBackgroundResource(Resource.Drawable.numberPickerShapeTransparentRed);
                else showJobDialogAddQuantityNumberPicker.SetBackgroundResource(Resource.Drawable.numberPickerShapeTransparentGreen);

                showJobDialogAddButton.Visibility = ViewStates.Visible;
                showJobDialogAddQuantityLinearLayout.Visibility = ViewStates.Visible;
                showJobDialogNeedTodayLinearLayout.Visibility = ViewStates.Visible;
                showJobDialogEditButton.Visibility = ViewStates.Gone;
                showJobDialogIsNormalizedLinearLayout.Visibility = ViewStates.Gone;
            }

            showJobDialogQuantityTextView.Text = job.Quantity.ToString();

            var isNormalized = job.IsNormalized;
            LinearLayout showJobDialogDescriptionLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.showJobDialogDescriptionLinearLayout);
            LinearLayout showJobDialogWorkIdLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.showJobDialogWorkIdLinearLayout);
            if (isNormalized == true)
            {
                showJobDialogChooseWorkTextView.Text = choosenWork.WorkCode;
                showJobDialogIsNormalizedTextView.Text = GetString(Resource.String.Yes);
                showJobDialogTitleTextView.Text = choosenWork.Name;
                showJobDialogDescriptionLinearLayout.Visibility = ViewStates.Gone;
                showJobDialogWorkIdLinearLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                showJobDialogTimeTextView.Text = job.Time.Value.ToString();
                showJobDialogIsNormalizedTextView.Text = GetString(Resource.String.No);
                showJobDialogTitleTextView.Text = job.Description;
                showJobDialogDescriptionLinearLayout.Visibility = ViewStates.Visible;
                showJobDialogWorkIdLinearLayout.Visibility = ViewStates.Gone;
            }

            return view;
        }

        private void ShowJobDialogAddQuantityNumberPicker_ValueChanged(object sender, NumberPicker.ValueChangeEventArgs e)
        {
            if (e.NewVal < needToday) showJobDialogAddQuantityNumberPicker.SetBackgroundResource(Resource.Drawable.numberPickerShapeTransparentRed);
            else showJobDialogAddQuantityNumberPicker.SetBackgroundResource(Resource.Drawable.numberPickerShapeTransparentGreen);
        }

        private void ShowJobDialogAddButton_Click(object sender, EventArgs e)
        {
            var addJob = new Job { Date = DateTime.Now, Quantity = showJobDialogAddQuantityNumberPicker.Value, WorkId = job.WorkId, Type = CURRENT_JOB_TYPE, Description = job.Description, IsNormalized = job.IsNormalized, Time = job.Time };
            if (addJob.Quantity == job.Quantity) database.DeleteItem(this.Activity, job, JOB_TABLE_NAME);
            else
            {
                job.Quantity = job.Quantity - addJob.Quantity;
                database.UpdateItem(this.Activity, job, JOB_TABLE_NAME);
            }
            if (jobs.Any(j => j.Date.Date == DateTime.Now.Date && (j.WorkId == addJob.WorkId) && j.Type == CURRENT_JOB_TYPE))
            {
                var existingJob = jobs.First(j => j.Date.Date == DateTime.Now.Date && addJob.WorkId == j.WorkId && j.Type == CURRENT_JOB_TYPE);
                existingJob.Quantity += addJob.Quantity;
                database.UpdateItem(this.Activity, existingJob, JOB_TABLE_NAME);
            }
            else
            {
                database.PutItem(this.Activity, addJob, JOB_TABLE_NAME);
            }

            Dismiss();

            this.Activity.FinishAffinity();
            var intent = new Intent(this.Activity, typeof(MainActivity));
            StartActivity(intent);
        }

        private void ShowJobDialogDeleteButton_Click(object sender, EventArgs e)
        {
            database.DeleteItem(this.Activity, job, JOB_TABLE_NAME);

            Dismiss();
        }

        private void ShowJobDialogEditButton_Click(object sender, EventArgs e)
        {
            Dismiss();
            var dialog_EditJob = new AddJobDialogFragment();
            Bundle args = new Bundle();
            args.PutInt(CHOOSEN_JOB_ID_EXTRA_NAME, job.Id);
            dialog_EditJob.Arguments = args;
            var fragmentTransaction = this.Activity.SupportFragmentManager.BeginTransaction();
            dialog_EditJob.Show(fragmentTransaction, TAG);



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