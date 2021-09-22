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
        StructureBlueprint blueprint;
        public OnTouchEvent(GameActivity activity)
        {
            this.activity = activity;
            finished = false;
            blueprint = null;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (blueprint == null)
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
                        if (t != null)
                        {
                            t.Stop();
                            if (!finished && lastView is Button btn)
                            {
                                int x = (int)activity.map.player.GetX();
                                int y = (int)activity.map.player.GetY();
                                if (activity.map.generator.terrainMap[x, y].GetItem() == null)
                                {
                                    Item i = activity.map.player.dropItem(int.Parse(lastView.Tag.ToString()) - 1);
                                    if (i is Tool)
                                    {
                                        Console.WriteLine("true");
                                    }
                                    activity.map.generator.SetItemPointer(x, y, i);
                                    activity.Invalidate();
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (e.Action)
                {

                    case MotionEventActions.Down:
                        Item item = activity.map.player.GetInvetory()[int.Parse(v.Tag.ToString()) - 1];
                        if (blueprint.AddItem(item))
                        {
                            activity.map.player.dropItem(int.Parse(v.Tag.ToString()) - 1);
                            activity.Invalidate();
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        public void SetBlueprint(StructureBlueprint sb)
        {
            this.blueprint = sb;
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
            activity.map.player.SortInventory();
            activity.Invalidate();
        }
    }
}