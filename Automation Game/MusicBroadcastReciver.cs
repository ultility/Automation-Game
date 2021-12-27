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
        

        public MusicBroadcastReciver()
        {
        }

        public MusicBroadcastReciver(List<List<MediaPlayer>> music)
        {
            musics = new List<List<MediaPlayer>>();
            musics.AddRange(music);
            Intent i = new Intent("music");
            active = active | 0b1;
            musics[0][0].Start();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            int play = intent.GetIntExtra("music", -1);
            if (play >= 0 && play < musics.Count)
            {
                active = active ^ (1 << play);
                if ((active & (1 << play)) == (1 << play))
                {
                    musics[play][0].Start();
                    MusicService.Stop = false;
                }
                else
                {
                    musics[play][0].Stop();
                }
                Console.WriteLine("plays music" + play);
                
            }
            if (active == 0)
            {
                MusicService.Stop = true;
            }
        }
        
    }
}