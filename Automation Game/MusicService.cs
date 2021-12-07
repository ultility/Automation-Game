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
            MediaPlayer menu = MediaPlayer.Create(this, Resource.Raw.Minecraft_Music___Menu_1);
            MediaPlayer game = MediaPlayer.Create(this, Resource.Raw.Terraria_Soundtrack___01___Overworld_Day);
            broadcastReciver = new MusicBroadcastReciver(menu, game);
            IntentFilter f = new IntentFilter();
            RegisterReceiver(broadcastReciver, f);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return base.OnStartCommand(intent, flags, startId);
        }
    }
}