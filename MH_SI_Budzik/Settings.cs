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
using Android.Util;
using Android.Database;

namespace MH_SI_Budzik
{
    [Activity(Label = "Ustawienia")]
    public class Settings : Activity
    {
        Button button_ringtone;
        EditText editText1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            button_ringtone = (Button)FindViewById<Button>(Resource.Id.button_ringtone);
            editText1 = (EditText)FindViewById<EditText>(Resource.Id.editText1);
            editText1.Text = MainActivity.czasZasypiania.ToString();

            editText1.TextChanged += (sender, e) =>
            {
                //int x = 0;
                if (Int32.TryParse(editText1.Text, out int x)) MainActivity.czasZasypiania = x;
            };

            button_ringtone.Click += (sender, e) =>
            {
                Intent intent = new Intent(Intent.ActionOpenDocument);
                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("audio/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(intent, 1);
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            System.String uri_string = data.DataString;
            RingtonePlayingService.ringtone_uri = Android.Net.Uri.Parse(uri_string);
            Log.Error("sciezka: ", uri_string);
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}