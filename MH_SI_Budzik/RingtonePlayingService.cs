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
using Android.Media;

namespace MH_SI_Budzik
{
    [Service]
    class RingtonePlayingService : Service
    {
        MediaPlayer media_song;
        Boolean isRunning;
        public static Android.Net.Uri ringtone_uri = Android.Net.Uri.Parse("android.resource://MH_SI_Budzik.MH_SI_Budzik/" + Resource.Raw.hitsound);
        //int start_id;

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Info("LocalService", "Received start id" + startId + ": " + intent);

            Boolean state = intent.GetBooleanExtra("extra", true);
            Log.Error("Ringtone state: ", state.ToString());
            //Log.Error("startId WAS set to: ", startId.ToString());
            if (state && !this.isRunning)
            {
                // create an instance of the media player
                media_song = MediaPlayer.Create(this, ringtone_uri);
                media_song.Looping = true;
                media_song.Start();

                // set up the notification service
                NotificationManager notify_manager = (NotificationManager)GetSystemService(NotificationService);
                // set up the intent that goes to the MainActivity
                Intent notification_intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                PendingIntent notification_pending_intent = PendingIntent.GetActivity(this, 0, notification_intent, 0);
                //make the notification parameters
                Notification notification_popup = new Notification.Builder(this).SetContentTitle("Tytul").SetContentText("Tekst").SetSmallIcon(Resource.Drawable.ic_notification).SetContentIntent(notification_pending_intent).SetAutoCancel(true).Build();

                // set up notification start command
                notify_manager.Notify(0, notification_popup);

                this.isRunning = true;
                state = false;
            }
            else if (!state && this.isRunning)
            {
                media_song.Stop();
                media_song.Reset();

                this.isRunning = false;
            };
            //Log.Error("startId IS set to: ", startId.ToString());



            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            Toast.MakeText(this, "on Destroy called", ToastLength.Short).Show();
            base.OnDestroy();
        }
    }
}