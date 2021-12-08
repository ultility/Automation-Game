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

namespace Automation_Game
{
    [Service]
    class MusicService : Service
    {
        MusicBroadcastReciver broadcastReciver;
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            List<MediaPlayer> music = new List<MediaPlayer>();
            music.Add(MediaPlayer.Create(this, Resource.Raw.menu1));
            music.Add(MediaPlayer.Create(this, Resource.Raw.day1));
            broadcastReciver = new MusicBroadcastReciver(music);
            IntentFilter f = new IntentFilter("music");
            RegisterReceiver(broadcastReciver, f);
        }

        public override void OnDestroy()
        {
            UnregisterReceiver(broadcastReciver);
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return base.OnStartCommand(intent, flags, startId);
        }
    }
}