using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FakroApp.Persistance;
using Plugin.Permissions;

namespace FakroApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme", MainLauncher = true)]
    public class PermissionsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Thread permissionsThread = new Thread(async wt =>
            {
                await CheckPermissionsAsync();
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            });
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Create your application here
            permissionsThread.Start();
        }

        private async System.Threading.Tasks.Task CheckPermissionsAsync()
        {
            await Permissions.CheckPermissions(this);
        }



        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }
    }
}