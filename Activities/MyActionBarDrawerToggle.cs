using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using static FakroApp.Persistance.Constants;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace FakroApp.Activities
{
    class MyActionBarDrawerToggle : SupportActionBarDrawerToggle
    {
        private AppCompatActivity activity;
        public MyActionBarDrawerToggle(AppCompatActivity activity, DrawerLayout drawerLayout)
            : base(activity, drawerLayout, Resource.String.openDrawer, Resource.String.closeDrawer)
        {
            this.activity = activity;
        }

        public override void OnDrawerOpened(View drawerView)
        {
            if ((string)drawerView.Tag == LEFT_DRAWER_TAG)
            {
                base.OnDrawerOpened(drawerView);
            }
        }

        public override void OnDrawerClosed(View drawerView)
        {
            if ((string)drawerView.Tag == LEFT_DRAWER_TAG)
            {
                base.OnDrawerClosed(drawerView);
            }
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            if ((string)drawerView.Tag == LEFT_DRAWER_TAG)
            {
                base.OnDrawerSlide(drawerView, slideOffset);
            }
        }

    }
}