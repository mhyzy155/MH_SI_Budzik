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

namespace MH_SI_Budzik
{
    [Activity(Label = "Rozwiąż równanie")]
    public class Zagadka : Activity
    {
        EditText zagadka_editText;
        Button zagadka_button;
        TextView zagadka_text2;
        int odp = 0;

        //obiekt do losowania liczb
        Random random = new Random();

        //funkcja wywoływana przy tworzeniu się Activity Zagadka
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //wczytanie wyglądu z zagadka.axml
            SetContentView(Resource.Layout.Zagadka);

            //losowanie jak długie ma być rówanie(ile członów)
            int ilosc_wsp = random.Next(1, 5);

            //stworzenie listy współczynników(przemnożone przez siebie dają jeden człon) oraz listy znaków członów
            List<int> wsp = new List<int>();
            List<int> wsp2 = new List<int>();
            List<bool> znak = new List<bool>();

            //losowanie wartości wyrazu wolnego
            int wsp_wolny = random.Next(1, 16);

            //losowanie współczynników, znaków oraz liczenie sumy, czyli wyniku rówania
            int suma = wsp_wolny;
            for (int i = 0; i < ilosc_wsp; i++)
            {
                wsp.Add(random.Next(1, 12 - ilosc_wsp * 2));
                wsp2.Add(random.Next(1, 12 - ilosc_wsp * 2));
                if (random.NextDouble() < 0.5)
                {
                    znak.Add(false); //false - minus, true - plus
                    suma -= (wsp[i] * wsp2[i]);
                }
                else
                {
                    znak.Add(true);
                    suma += (wsp[i] * wsp2[i]);
                }
            }

            //warunek, że suma(wynik) jest większa od 0 (do pola odpwiedzi można wpisać tylko liczby całkowite nieujemne)
            //nie chcemy, aby wyszło 0 bo taka jest wartość domyślna odpowiedzi
            if (suma <= 0)
            {
                //suma ma losową wartość z zakresu od 1 do 4, dostosowujemy wartość wyrazu wolnego
                int plus = random.Next(1, 5);
                wsp_wolny += Math.Abs(suma) + plus;
                suma = plus;
            }

            //zainicjowanie pola odpowiedzi, przycisku i tekstu z rówaniem
            zagadka_editText = FindViewById<EditText>(Resource.Id.zagadka_editText);
            zagadka_button = FindViewById<Button>(Resource.Id.zagadka_button);
            zagadka_text2 = FindViewById<TextView>(Resource.Id.zagadka_text2);

            //zapisanie rówania w formie tekstu na ekranie
            zagadka_text2.Text += wsp_wolny;
            for (int i = 0; i < ilosc_wsp; i++)
            {
                if (znak[i]) zagadka_text2.Text += "+";
                else zagadka_text2.Text += "-";
                zagadka_text2.Text += wsp[i] + "*" + wsp2[i];
            }
            zagadka_text2.Text += "=?";

            //reakcja na zmianę w polu odpowiedzi
            zagadka_editText.TextChanged += (sender, e) =>
            {
                //zamiana stringa na wartość odpowiedzi
                if (Int32.TryParse(zagadka_editText.Text, out int x)) odp = x;
            };

            //reakcja na naciśnięcie przycisku - reaguje tylko gdy odpowiedź jest poprawna
            zagadka_button.Click += (sender, e) =>
            {
                if (odp == suma)
                {
                    Toast.MakeText(this, "Równanie rozwiązane prawidłowo", ToastLength.Short).Show();

                    //wyłącz alarm
                    MainActivity.AnulujAlarm(this);

                    //usuń to Activity, przerzuć z powrotem do gównego ekranu apki
                    Finish();
                }
            };
        }

        //zablokowanie możliwości cofnięcia do głównego ekranu bez rozwiązania rówania
        public override void OnBackPressed()
        {

        }
    }


}