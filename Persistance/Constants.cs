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
        //Table names
        public const string DAYOFF_TABLE_NAME = "DayOff";
        public const string EMPLOYEE_TABLE_NAME = "Employee";
        public const string JOB_TABLE_NAME = "Job";

        //Job types
        public const string CURRENT_JOB_TYPE = "Current";
        public const string RESERVE_JOB_TYPE = "Reserve";

        //Day off type
        public const string LEAVE_DAYOFF_TYPE = "Leave";
        public const string UNPAIDLEAVE_DAYOFF_TYPE = "UnpaidLeave";
        public const string SICKLEAVE_DAYOFF_TYPE = "SickLeave";
    }
}