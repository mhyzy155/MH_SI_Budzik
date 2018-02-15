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

namespace MH_SI_Budzik
{
    [BroadcastReceiver(Enabled = true)]
    public class Alarm_Receiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //pobranie extra boolean z intent
            Boolean get_your_bool = intent.GetBooleanExtra("extra", true);
            Log.Error("Wartosc boola: ", get_your_bool.ToString()); //sprawdzenie poprawności wykonywania programu


            //stworzenie Intentu prowadzącego do klasy RingtonePlayingService
            Intent service_intent = new Intent(context, typeof(RingtonePlayingService));

            //przekazanie extra boolean dalej do RingtonePlayingService
            service_intent.PutExtra("extra", get_your_bool);

            //uruchomienie RingtonePlayingService za pomocą service_intent
            context.StartService(service_intent);
        }
    }
}