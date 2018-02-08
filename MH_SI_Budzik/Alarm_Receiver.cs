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
            Log.Error("We are in the receiver.", "Ok.");

            // fetch extra strings from the intent
            Boolean get_your_bool = intent.GetBooleanExtra("extra", true);
            Log.Error("What is the key? ", get_your_bool.ToString());


            // create an intent to the ringtone service
            Intent service_intent = new Intent(context, typeof(RingtonePlayingService));

            // pass the extra string to the RingtonePlayingService
            service_intent.PutExtra("extra", get_your_bool);

            // start the ringtone service
            context.StartService(service_intent);
        }
    }
}