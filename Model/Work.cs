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

namespace FakroApp.Model
{
    //List of works
    public class Work
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Norm { get; set; }


        private List<Work> works = new List<Work>
        {
            new Work {Id = "4,325", Name = "Zasilacz elektro", Norm = 4.52},
            new Work {Id = "4,342", Name = "Zasilacz świetlik", Norm = 4.52},
            new Work {Id = "6,872", Name = "ZRH12 Pakowanie do zestawu", Norm = 5.85}
        };

        public static List<Work> GetWorks()
        {
            var work = new Work();
            return work.works;
        }


    }
}