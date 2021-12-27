using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Media;
using System.Threading;

namespace Automation_Game
{
    [BroadcastReceiver]
    public class MusicBroadcastReciver : BroadcastReceiver
    {
        List<List<MediaPlayer>> musics;
        int active = 0;
        Thread countdownTimer;

        public MusicBroadcastReciver()
        {
        }

        public MusicBroadcastReciver(List<List<MediaPlayer>> music)
        {
            musics = new List<List<MediaPlayer>>();
            musics.AddRange(music);
            Intent i = new Intent("music");
            musics[0][0].Start();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            int play = intent.GetIntExtra("music", -1);
            if (play >= 0 && play < musics.Count)
            {
                active = active ^ play;
                if (countdownTimer != null && countdownTimer.IsAlive)
                {
                    countdownTimer.Abort();
                }
                musics[play][0].Start();
                Console.WriteLine("plays music" + play);
                
            }
            if (active == 0)
            {
                countdownTimer = new Thread(new ThreadStart(Countdown));
                countdownTimer.Start();
            }
        }

        private void Countdown()
        {

        }
        
    }
}