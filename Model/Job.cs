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
    //Job given every day

    public class Job
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }

        [NotNull]
        public int Quantity { get; set; }

        [NotNull]
        public DateTime Date { get; set; }

        [NotNull]
        public string Type { get; set; }

        public int WorkId { get; set; }

        [NotNull]
        public bool NotNormalized { get; set; }
    }
}