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

namespace FakroApp.Persistance
{
    public class Constants
    {
        //Drawers
        public const string RIGHT_DRAWER_TAG = "RightDrawer";
        public const string LEFT_DRAWER_TAG = "LeftDrawer";
        public const string MAIN_ACTIVITY_TAG = "MainActivity";
        public const string WORK_ACTIVITY_TAG = "WorkActivity";
        public const string WORK_LISTVIEW_ADAPTER_TAG = "WorkListViewAdapter";

        //Table names
        public const string DAYOFF_TABLE_NAME = "DayOff";
        public const string JOB_TABLE_NAME = "Job";
        public const string WORK_TABLE_NAME = "Work";

        //Job types
        public const string CURRENT_JOB_TYPE = "Current";
        public const string RESERVE_JOB_TYPE = "Reserve";

        //Day off type
        public const string LEAVE_DAYOFF_TYPE = "Leave";
        public const string UNPAIDLEAVE_DAYOFF_TYPE = "UnpaidLeave";
        public const string SICKLEAVE_DAYOFF_TYPE = "SickLeave";

        //Intents Extras Names
        public const string CHOOSEN_WORK_ID_EXTRA_NAME = "ChoosenWorkId";
        public const string CHOOSEN_JOB_ID_EXTRA_NAME = "ChoosenJobId";
    }
}