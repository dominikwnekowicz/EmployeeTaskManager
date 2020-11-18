using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FakroApp.Model;
using FakroApp.Persistance;
using static FakroApp.Persistance.Constants;

namespace FakroApp.Fragments
{
    public class AddWorkDialogFragment : Android.Support.V4.App.DialogFragment
    {
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.dialog_addWork, container, false);

            var addWorkDialogButton = view.FindViewById<Button>(Resource.Id.addWorkDialogAddButton);
            addWorkDialogButton.Click += AddWorkDialogButton_Click;

            var cancelAddWorkDialogButton = view.FindViewById<Button>(Resource.Id.addWorkDialogCancelButton);
            cancelAddWorkDialogButton.Click += CancelAddWorkDialogButton_Click;


            return view;
        }

        private void CancelAddWorkDialogButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private string ChangeSymbols(string text)
        {
            foreach(var character in text)
            {
                if (!Char.IsDigit(character) && character != ',') text = text.Replace(character, ',');
            }
            return text;
        }

        private void AddWorkDialogButton_Click(object sender, EventArgs e)
        {
            Database database = new Database();

            EditText addWorkDialogWorkCodeEditText = view.FindViewById<EditText>(Resource.Id.addWorkDialogWorkCodeEditText);
            var workCode = ChangeSymbols(addWorkDialogWorkCodeEditText.Text);
            var works = (List<Work>)database.GetItems(this.Activity, WORK_TABLE_NAME).Result;
            Work latestWork = null;
            if(works.Any(w => w.WorkCode == workCode)) latestWork = works.Last(w => w.WorkCode == workCode);

            EditText addWorkNameCodeEditText = view.FindViewById<EditText>(Resource.Id.addWorkNameCodeEditText);
            var workName = addWorkNameCodeEditText.Text;

            EditText addWorkNormCodeEditText = view.FindViewById<EditText>(Resource.Id.addWorkNormCodeEditText);
            var workNorm = Convert.ToDouble(ChangeSymbols(addWorkNormCodeEditText.Text));

            if (latestWork != null && workNorm == latestWork.Norm)
            {
                latestWork.Name = workName;
                database.UpdateItem(this.Activity, latestWork, WORK_TABLE_NAME);
            }
            else
            {
                var work  = new Work { Name = workName, Norm = workNorm, WorkCode = workCode, AddedDate = DateTime.Now };
                database.PutItem(this.Activity, work, WORK_TABLE_NAME);
            }


            Dismiss();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            Activity activity = this.Activity;
            ((IDialogInterfaceOnDismissListener)activity).OnDismiss(dialog);
        }
    }
}