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
using SQLite;

namespace FakroApp.Model
{
    public class DayOff
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public DateTime From { get; set; }

        [NotNull]
        public DateTime To { get; set; }

        [NotNull]
        public string Type { get; set; }
    }
}