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
    public class Employee
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Identifier { get; set; }

        public int PackerCode { get; set; }

        public DateTime WorkingSince{ get; set; }

        public int Bonus { get; set; }

    }
}