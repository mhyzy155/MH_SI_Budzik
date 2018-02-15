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
        //klasa odtwarzająca dźwięk
        MediaPlayer media_song;

        //zmienna wskazująca czy ten Service jest używany
        Boolean isRunning;

        //ustawienie domyślnego dźwięku alarmu(a raczej ścieżki do pliku dzwiękowego)
        public static Android.Net.Uri ringtone_uri = Android.Net.Uri.Parse("android.resource://MH_SI_Budzik.MH_SI_Budzik/" + Resource.Raw.hitsound);

        //nie robi nic ale trzeba nadpisać tę funkcję bo nie bedzie działać
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        //obsługa przychodzących intentów
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Info("Przychodzacy service", "id: " + startId + ", intent: " + intent); //sprawdzenie poprawności wykonywania programu

            //pobranie extra boolean z intent
            Boolean state = intent.GetBooleanExtra("extra", true);

            //z extra boolean wynika, że alarm powinien być włączony, a service nie jest uruchomiony - włącz dzwięk alarmu i odpal nowe Activity(z rozwiązywaniem rówania)
            if (state && !this.isRunning)
            {
                // create an instance of the media player
                media_song = MediaPlayer.Create(this, ringtone_uri);
                media_song.Looping = true;
                media_song.Start();

                this.isRunning = true;
                state = false;

                Intent zagadka_intent = new Intent(this, typeof(Zagadka));
                StartActivity(zagadka_intent);

            }
            //boolean - alarm powienien być wyłączony a service dalej jest uruchomiony - wyłącz dźwięk
            else if (!state && this.isRunning)
            {
                media_song.Stop();
                media_song.Reset();

                this.isRunning = false;
            };
            return StartCommandResult.NotSticky;
        }

        //destruktor
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}