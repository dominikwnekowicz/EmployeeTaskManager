using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace FakroApp.Persistance
{
    public class Permissions : Application
    {
        public static async Task CheckPermissions(Activity activity)
        {
            var storage = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
            if (storage != PermissionStatus.Granted)
            {
                storage = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();

                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage) || storage == PermissionStatus.Denied)
                {
                    AlertDialog.Builder dialog = new AlertDialog.Builder(activity);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Potrzebne uprawnienia");
                    alert.SetMessage("Do poprawnego działania aplikacji wymagane są uprawnienia do pamięci.");
                    alert.SetButton("OK", (c, ev) =>
                    {
                        activity.FinishAffinity();
                        alert.Hide();
                    });
                    alert.Show();
                }
            }
        }

    }
}