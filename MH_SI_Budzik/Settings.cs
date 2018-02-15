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

            //wczytanie wyglądu z settings.axml
            SetContentView(Resource.Layout.Settings);

            //zainicjowanie przycisku zmiany dzwonka
            button_ringtone = FindViewById<Button>(Resource.Id.button_ringtone);

            //zainicjowanie pola do wpisania czasu potrzebnego na zaśnięcie
            editText1 = FindViewById<EditText>(Resource.Id.editText1);

            //pobranie wartości aktualnie ustawionego czasu zasypiania i wstawienie go w wyżej zainicjowane pole
            editText1.Text = MainActivity.czasZasypiania.ToString();

            //reakcja na zmianę tekstu wpisanego w pole do wpisywania czasu potrzebnego na zaśnięcie
            editText1.TextChanged += (sender, e) =>
            {
                //zamiana wpisanego tekstu(cyfr w postaci stringa) na inta i nadpisanie czasu zasypiania nową wartością
                if (Int32.TryParse(editText1.Text, out int x)) MainActivity.czasZasypiania = x;
            };

            //reakcja na naciśnięcie przycisku zmiany dźwięku alarmu
            button_ringtone.Click += (sender, e) =>
            {
                //otwarcie eksploratora plików(tylko pliki audio)
                Intent intent = new Intent(Intent.ActionOpenDocument);
                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("audio/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(intent, 1);
            };
        }

        //funkcja pobiera ścieżkę wybranego wcześniej w eksploratorze pliku i nadpisuje domyślną ścieżkę w klasie RingtonePlayingService
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            System.String uri_string = data.DataString;
            RingtonePlayingService.ringtone_uri = Android.Net.Uri.Parse(uri_string);
            Log.Error("sciezka: ", uri_string);
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}