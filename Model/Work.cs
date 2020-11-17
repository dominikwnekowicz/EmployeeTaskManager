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
    //List of works
    public class Work
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string WorkCode { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public double Norm { get; set; }

        [NotNull]
        public DateTime AddedDate { get; set; }

    }
}