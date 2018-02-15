using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Java.Util;
using Android.Views;
using System;
using Java.Lang;
using Android.Util;
using Android.Runtime;
using System.Collections.Generic;

namespace MH_SI_Budzik
{
    [Activity(Label = "MH_SI_Budzik", MainLauncher = true)]
    public class MainActivity : Activity
    {
        static AlarmManager alarm_manager;
        TimePicker alarm_timepicker;
        TextView textView3;
        static Intent my_intent;
        static PendingIntent pending_intent;

        //czas zasypiania zmieniany w ustawieniach(domyślnie 0)
        public static int czasZasypiania = 0;

        //funkcja wywoływana przy odpaleniu apki
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //wczytanie wyglądu z main.axml
            SetContentView(Resource.Layout.Main);

            //zainicjalizowanie alarm_managera
            alarm_manager = (AlarmManager)GetSystemService(AlarmService);

            //zainicjowanie timepickera
            alarm_timepicker = FindViewById<TimePicker>(Resource.Id.timePicker1);

            //zainicjowanie tekstu textView3 (godziny zaśnięcia)
            textView3 = FindViewById<TextView>(Resource.Id.textView3);

            //stworzenie kalendarza
            Calendar calendar = Calendar.Instance;

            //stworzenie Intent kierującego do klasy Alarm_Receiver
            my_intent = new Intent(this, typeof(Alarm_Receiver));

            //zainicjowanie przycisku "Ustaw" alarm
            Button textView1 = FindViewById<Button>(Resource.Id.textView1);

            //reakcja na kliknięcie przycisku "Ustaw" alarm
            textView1.Click += (sender, e) =>
            {
                //pobranie czasu z TimePickera
                int hour = alarm_timepicker.Hour;
                int minute = alarm_timepicker.Minute;

                //ustawienie czasu alarmu w kalendarzu(potrzebne do ustawienia alarmu w alarm_managerze)
                calendar.Set(Calendar.HourOfDay, hour);
                calendar.Set(Calendar.Minute, minute);
                calendar.Set(Calendar.Second, 0);

                //pobranie aktualnego czasu
                DateTime dateTime = DateTime.Now;

                //tworzymy zmienną tej samej klasy co aktualny czas i podajemy do niej czas alarmu
                DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, dateTime.Second);

                //liczenie różnicy, sprawdzenie czy nie jest ujemna, jeśli jest to dodaj 24h (alarm dnia następnego)
                double d_roznica = (date - dateTime).TotalMinutes;
                int roznica;
                if (d_roznica < 0) //podajemy double bo różnica może być -0.1 jak i 0.1, a int zaokrąglił by oba przypadki do 0 (ujemny double, a int już dodatni)
                {
                    roznica = (int)(date - dateTime).TotalMinutes + 1440;
                    calendar.Set(Calendar.DayOfYear, dateTime.DayOfYear + 1);
                }
                else roznica = (int)(date - dateTime).TotalMinutes;

                //wyliczenie ile zostaje pełnych cykli snu(1,5h) z uwzględnieniem czasu zasypiania
                int hour_counter = (roznica - czasZasypiania) / 90;

                //jeśli zostanie za dużo czasu na sen, to ograniczamy sen do max. 9 godzin snu
                if (hour_counter > 6) hour_counter = 6;

                //stworzenie i wypełnienie listy z godzinami, kiedy masz się położyć (tworzenie max. 3 propozycji)
                List<DateTime> godziny_snu = new List<DateTime>();
                for (int i = hour_counter; i > 0 && i > (hour_counter - 3); i--)
                {
                    //kożystając z klasy TimeSpan odejmujemy od godziny alarmu czas snu z uwzględnieniem czasu zasypiania - pozwala to wyznaczyć godzinę o której trzeba położyć się spać
                    godziny_snu.Add(date.Add(new TimeSpan(0, -(i * 90) - czasZasypiania, 0)));
                }

                //wyświetlanie godzin o której masz iść spać, podział ze względu na podanie czasu potrzebnego na zaśnięcie
                //wyświetlanie różnych komunikatów w zależności od ilości czasu pozostałego na spanie
                if (czasZasypiania != 0)
                {
                    switch (godziny_snu.Count)
                    {
                        case 1:
                            Set_alarm_text("Połóż się spać o " + godziny_snu[0].Hour + ":" + FormMinut(godziny_snu[0].Minute));
                            break;
                        case 2:
                            Set_alarm_text("Połóż się spać o " + godziny_snu[0].Hour + ":" + FormMinut(godziny_snu[0].Minute) + " lub " + godziny_snu[1].Hour + ":" + FormMinut(godziny_snu[1].Minute));
                            break;
                        case 3:
                            Set_alarm_text("Połóż się spać o " + godziny_snu[0].Hour + ":" + FormMinut(godziny_snu[0].Minute) + ", " + godziny_snu[1].Hour + ":" + FormMinut(godziny_snu[1].Minute) + " lub " + godziny_snu[2].Hour + ":" + FormMinut(godziny_snu[2].Minute));
                            break;
                        default:
                            Set_alarm_text("Natychmiast idź spać!");
                            break;
                    }
                    Toast.MakeText(this, "Alarm ustawiony na " + hour + ":" + FormMinut(minute), ToastLength.Short).Show();
                }
                else
                {
                    switch (godziny_snu.Count)
                    {
                        case 1:
                            Set_alarm_text("Zaśnij o " + godziny_snu[0].Hour + ":" + FormMinut(godziny_snu[0].Minute));
                            Toast.MakeText(this, "Alarm ustawiony na " + hour + ":" + FormMinut(minute), ToastLength.Short).Show();
                            break;
                        case 2:
                            Set_alarm_text("Zaśnij o " + godziny_snu[0].Hour + ":" + FormMinut(godziny_snu[0].Minute) + " lub " + godziny_snu[1].Hour + ":" + FormMinut(godziny_snu[1].Minute));
                            Toast.MakeText(this, "Alarm ustawiony na " + hour + ":" + FormMinut(minute), ToastLength.Short).Show();
                            break;
                        case 3:
                            Set_alarm_text("Zaśnij o " + godziny_snu[0].Hour + ":" + FormMinut(godziny_snu[0].Minute) + ", " + godziny_snu[1].Hour + ":" + FormMinut(godziny_snu[1].Minute) + " lub " + godziny_snu[2].Hour + ":" + FormMinut(godziny_snu[2].Minute));
                            Toast.MakeText(this, "Alarm ustawiony na " + hour + ":" + FormMinut(minute), ToastLength.Short).Show();
                            break;
                        default:
                            Set_alarm_text("Alarm ustawiony na " + hour + ":" + FormMinut(minute));
                            break;
                    }
                }
                //dodanie extra boolean do my_intent, true - włącz dźwięk alarmu gdy sygnał przyjdzie do Alarm_Receiver i następnie do RingtonePlayingService
                my_intent.PutExtra("extra", true);

                //stworzenie PendingIntent, czyli opóźnionego Intent
                pending_intent = PendingIntent.GetBroadcast(this, 0, my_intent, PendingIntentFlags.UpdateCurrent);

                //ustawienie alarmu dla pending_intent na czas zapisany w calendar
                alarm_manager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, pending_intent);
            };

            //zainicjowanie przycisku "Anuluj"
            Button textView2 = FindViewById<Button>(Resource.Id.textView2);

            //reakcja na kliknięcie "Anuluj" alarm
            textView2.Click += (sender, e) =>
            {
                Set_alarm_text("Alarm anulowany!");
                AnulujAlarm(this);

            };

        }

        //funkcja anulowania alarmu
        public static void AnulujAlarm(Context context)
        {
            //anuluj alarm w alarm_menagerze
            alarm_manager.Cancel(pending_intent);

            //dodaj extra boolean do my_intent, false - wylacz dzwiek alarmu
            my_intent.PutExtra("extra", false);

            //wyłacz alarm
            context.SendBroadcast(my_intent);
        }

        //funkcja dodająca pasek z możliwością przejścia do ustawień
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //funkcja wywoływana po naciśnięciu ikony ustawień
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.settings)
            {
                //stworzenie intent i przejście do Activity z ustawieniami
                Intent settings_intent = new Intent(this, typeof(Settings));
                StartActivity(settings_intent);
            }
            return true;
        }

        //ustawienie tekstu textView3 na określony napis
        private void Set_alarm_text(string output)
        {
            textView3.Text = output;
        }

        //formatowanie wyświetlanej godziny
        private System.String FormMinut(int czasMinuty)
        {
            System.String minutes;
            if (czasMinuty < 10) minutes = "0" + czasMinuty;
            else minutes = czasMinuty.ToString();
            return minutes;
        }
    }
}

