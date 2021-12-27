using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Automation_Game
{
    [Service]
    class MusicService : Service
    {
        public static bool Stop;
        MusicBroadcastReciver broadcastReciver;
        int ms = 0;
        const int TIME_TILL_CLOSE = 60;
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override void OnCreate()
        {
            Stop = true;
            base.OnCreate();
            List<List<MediaPlayer>> music = new List<List<MediaPlayer>>();
            List<MediaPlayer> temp = new List<MediaPlayer>();
            temp.Add(MediaPlayer.Create(this, Resource.Raw.menu1));
            music.Add(temp);
            temp = new List<MediaPlayer>();
            temp.Add(MediaPlayer.Create(this, Resource.Raw.day1));
            music.Add(temp);
            broadcastReciver = new MusicBroadcastReciver(music);
            IntentFilter f = new IntentFilter("music");
            RegisterReceiver(broadcastReciver, f);
            Console.WriteLine("\n\nregistered\n\n");
        }

        public override void OnDestroy()
        {
            UnregisterReceiver(broadcastReciver);
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Stop = false;
            Thread countdownTimer = new Thread(new ThreadStart(Countdown));
            countdownTimer.Start();
            return base.OnStartCommand(intent, flags, startId);
        }

        private void Countdown()
        {
            while (true)
            {
                ms = TIME_TILL_CLOSE;
                while (ms > 0 && Stop)
                {
                    ms--;
                    Thread.Sleep(1000);
                }
                if (ms <= 0)
                {
                    StopSelf();
                    break;
                }
            }
        }
    }
}