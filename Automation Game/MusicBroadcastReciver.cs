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

namespace Automation_Game
{
    [BroadcastReceiver]
    public class MusicBroadcastReciver : BroadcastReceiver
    {
        List<MediaPlayer> musics;

        public MusicBroadcastReciver()
        {
        }

        public MusicBroadcastReciver(List<MediaPlayer> music)
        {
            musics = new List<MediaPlayer>();
            musics.AddRange(music);
        }
        public override void OnReceive(Context context, Intent intent)
        {
            int play = intent.GetIntExtra("music", -1);
            if (play >= 0 && play < musics.Count)
            {
                musics[play].Start();
            }
            else
            {
                foreach (MediaPlayer player in musics)
                {
                    player.Stop();
                }
            }
        }
    }
}