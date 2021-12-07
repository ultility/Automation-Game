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
        MediaPlayer menu, game;
        public MusicBroadcastReciver(MediaPlayer menu, MediaPlayer game)
        {
            this.menu = menu;
            this.game = game;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            int play = intent.GetIntExtra("music", -1);
            switch (play)
            {
                case 0:
                    game.Stop();
                    menu.Start();
                    break;
                case 1:
                    menu.Stop();
                    game.Start();
                    break;
                default:
                    break;
            }
        }
    }
}