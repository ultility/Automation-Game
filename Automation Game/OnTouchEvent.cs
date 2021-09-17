using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Interop;
using System;
using System.Timers;

namespace Automation_Game
{
    internal class OnTouchEvent : Java.Lang.Object, View.IOnTouchListener
    {
        Timer t;
        GameActivity activity;
        View lastView;
        bool finished;
        public OnTouchEvent(GameActivity activity)
        {
            this.activity = activity;
            finished = false;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    t = new Timer();
                    t.Interval = 1000;
                    t.Elapsed += Elapsed;
                    t.Start();
                    lastView = v;
                    finished = false;
                    break;
                case MotionEventActions.Up:
                    t.Stop();
                    if (!finished && lastView is Button btn)
                    {
                        activity.map.player.dropItem(int.Parse(lastView.Tag.ToString()) - 1);
                        activity.Invalidate();
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            finished = true;
            Handler handle = new Handler(Looper.MainLooper);
            handle.Post(equip);
        }

        private void equip()
        {
            activity.map.player.Equip(int.Parse(lastView.Tag.ToString()) - 1);
            activity.Invalidate();
        }
    }
}