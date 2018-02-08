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
        AlarmManager alarm_manager;
        TimePicker alarm_timepicker;
        TextView textView3;
        PendingIntent pending_intent;
        public static int czasZasypiania = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //this.context = this;

            // initialize our alarm manager
            alarm_manager = (AlarmManager)GetSystemService(AlarmService);

            // initialize out timepicker
            alarm_timepicker = (TimePicker)FindViewById(Resource.Id.timePicker1);

            // initialize our text update box
            textView3 = (TextView)FindViewById(Resource.Id.textView3);

            // create an instance of a calendar
            Calendar calendar = Calendar.Instance;

            //create an intent to the Alarm Receiver class
            Intent my_intent = new Intent(this, typeof(Alarm_Receiver));

            // initialize start button
            Button textView1 = (Button)FindViewById(Resource.Id.textView1);

            // create an onClick listener to start the alarm
            textView1.Click += (sender, e) =>
            {
                int hour = alarm_timepicker.Hour;
                int minute = alarm_timepicker.Minute;

                calendar.Set(Calendar.HourOfDay, hour);
                calendar.Set(Calendar.Minute, minute);

                DateTime dateTime = DateTime.Now;

                DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, dateTime.Second);
                double d_roznica = (date - dateTime).TotalMinutes;
                int roznica;
                if (d_roznica < 0)
                {
                    roznica = (int)(date - dateTime).TotalMinutes + 1440;
                    calendar.Set(Calendar.DayOfYear, dateTime.DayOfYear + 1);
                }
                else roznica = (int)(date - dateTime).TotalMinutes;
                //Toast.MakeText(this, (date - dateTime).TotalMinutes.ToString(), ToastLength.Short).Show();
                int hour_counter = (roznica - czasZasypiania) / 90;
                if (hour_counter > 6) hour_counter = 6;
                List<DateTime> godziny_snu = new List<DateTime>();
                for (int i = hour_counter; i > 0 && i > (hour_counter - 3); i--)
                {
                    godziny_snu.Add(date.Add(new TimeSpan(0, -(i * 90) - czasZasypiania, 0)));
                }

                //Toast.MakeText(this, hour_counter.ToString(), ToastLength.Short).Show();
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
                // put in extra string into my_intent, tells the clock that you pressed the "alarm on" button
                my_intent.PutExtra("extra", true);

                // create a pending intent that delays the intent until the specified calendar time
                pending_intent = PendingIntent.GetBroadcast(this, 0, my_intent, PendingIntentFlags.UpdateCurrent);

                // set the alarm manager
                alarm_manager.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pending_intent);
            };

            // initialize stop button
            Button textView2 = (Button)FindViewById(Resource.Id.textView2);

            // create an onClick listener to stop the alarm
            textView2.Click += (sender, e) =>
            {
                Set_alarm_text("Alarm anulowany!");

                // cancel the alarm
                alarm_manager.Cancel(pending_intent);

                // put extra string into my_intent, tells the clock that you pressed "alarm off" button
                my_intent.PutExtra("extra", false);

                // stop the ringtone
                SendBroadcast(my_intent);

            };

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.settings)
            {
                Intent settings_intent = new Intent(this, typeof(Settings));
                StartActivity(settings_intent);
            }
            return true;
        }

        private void Set_alarm_text(string output)
        {
            textView3.Text = output;
        }

        private System.String FormMinut(int czasMinuty)
        {
            System.String minutes;
            if (czasMinuty < 10) minutes = "0" + czasMinuty;
            else minutes = czasMinuty.ToString();
            return minutes;
        }
    }
}

