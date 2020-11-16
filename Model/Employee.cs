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
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        [NotNull]
        public string Login { get; set; }

        [NotNull]
        public int PackerCode { get; set; }

        public DateTime WorkingSince{ get; set; }

        public int Bonus { get; set; }

    }
}