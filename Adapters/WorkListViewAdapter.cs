﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using FakroApp.Activities;
using FakroApp.Fragments;
using FakroApp.Model;
using FakroApp.Persistance;
using static FakroApp.Persistance.Constants;
namespace FakroApp.Adapters
{
    class WorkListViewAdapter : BaseAdapter<Work>
    {

        Activity activity;
        List<Work> works;
        readonly string tag;

        public WorkListViewAdapter(Activity activity, List<Work> works, string tag)
        {
            this.activity = activity;
            this.works = works;
            this.tag = tag;
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
            var view = activity.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);

            var work = works[position];

            TextView text1 = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            if(work.Name.Length > 40) text1.Text = work.Name.Substring(0, 37) + "... (" + work.WorkCode + ")";
            else text1.Text = work.Name + " (" + work.WorkCode + ")";

            TextView text2 = view.FindViewById<TextView>(Android.Resource.Id.Text2);
            text2.Text = "Norma: " + work.Norm + " min.";

            if (tag == WORK_ACTIVITY_TAG)
            {
                view.Click += (o, e) =>
                {
                        Intent intent = new Intent(activity, typeof(MainActivity));
                        intent.PutExtra(CHOOSEN_WORK_ID_EXTRA_NAME, work.Id);
                        activity.SetResult(Result.Ok, intent);
                        activity.Finish();
                };

            }
            else if(tag == MAIN_ACTIVITY_TAG)
            {
                var minutes = DataManager.GetDailyMinutes(activity, DateTime.Now);
                var quantity = DataManager.GetNeededQuantity(activity, work);
                if (quantity != 0) text2.Text = "Do końca dnia: " + quantity + "x";
                var mainActivity = ((MainActivity)activity);
                view.Click += (o, e) =>
                {
                    var dialog_AddJob = new AddJobDialogFragment();
                    Bundle args = new Bundle();
                    args.PutInt(CHOOSEN_WORK_ID_EXTRA_NAME, work.Id);
                    dialog_AddJob.Arguments = args;
                    var fragmentTransaction = mainActivity.SupportFragmentManager.BeginTransaction();
                    dialog_AddJob.Show(fragmentTransaction, WORK_LISTVIEW_ADAPTER_TAG);
                    var drawerLayout = mainActivity.FindViewById<DrawerLayout>(Resource.Id.mainLayoutDrawerLayout);
                    var rightDrawer_mainLayoutLinearLayout = mainActivity.FindViewById<LinearLayout>(Resource.Id.rightDrawer_mainLayoutLinearLayout);
                    drawerLayout.CloseDrawer(rightDrawer_mainLayoutLinearLayout);
                };
            }

            //fill in your items
            //holder.Title.Text = "new text here";

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return works.Count;
            }
        }

        public override Work this[int position]
        {
            get
            {
                return works[position];
            }
        }
    }

    class WorkListViewAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}