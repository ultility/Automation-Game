using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Timers;

namespace Automation_Game
{
    internal class OnTouchEvent : Java.Lang.Object, View.IOnTouchListener
    {
        Timer t;
        readonly GameActivity activity;
        View lastView;
        bool finished;
        StructureBlueprint blueprint;
        Structure hole;

        public OnTouchEvent()
        {

        }
        public OnTouchEvent(GameActivity activity)
        {
            this.activity = activity;
            finished = false;
            blueprint = null;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (activity == null)
            {
                int height = v.Height;
                int width = v.Width;
                return true;
            }
            if (blueprint == null)
            {
                if (hole == null)
                {
                    switch (e.Action)
                    {

                        case MotionEventActions.Down:
                            t = new Timer
                            {
                                Interval = 1000
                            };
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
                                    int x = (int)activity.Map.Player.GetX();
                                    int y = (int)activity.Map.Player.GetY();
                                    if (activity.Map.Generator.TerrainMap[x, y].GetItem() == null)
                                    {
                                        Item i = activity.Map.Player.DropItem(int.Parse(lastView.Tag.ToString()) - 1);
                                        if (i is Tool)
                                        {
                                            Console.WriteLine("true");
                                        }
                                        activity.Map.Generator.SetItemPointer(x, y, i);
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
                            Item item = activity.Map.Player.GetInvetory()[int.Parse(v.Tag.ToString()) - 1];
                            if (item.name.Equals(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.TREE_SEED].name))
                            {
                                activity.Map.Player.DropItem(int.Parse(v.Tag.ToString()) - 1);
                                Plant p = new Plant(1f / 60 / 10, 2, MapDraw.structureTypeList[(int)MapDraw.StructureTypes.TREE], true);
                                activity.Map.Generator.TerrainMap[(int)activity.Map.Player.GetX(), (int)activity.Map.Player.GetY()].BuildStructure(p);
                                activity.Updateables.Add(p);
                                activity.Invalidate();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                switch (e.Action)
                {

                    case MotionEventActions.Down:
                        Item item = activity.Map.Player.GetInvetory()[int.Parse(v.Tag.ToString()) - 1];
                        if (blueprint.AddItem(item))
                        {
                            activity.Map.Player.DropItem(int.Parse(v.Tag.ToString()) - 1);
                            activity.UsedBlueprint = blueprint;
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

        public void SetStructure(Structure s)
        {
            this.hole = s;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            finished = true;
            Handler handle = new Handler(Looper.MainLooper);
            handle.Post(Equip);
            ((Timer)sender).Stop();
        }

        private void Equip()
        {
            activity.Map.Player.Equip(int.Parse(lastView.Tag.ToString()) - 1);
            activity.Map.Player.SortInventory();
            activity.Invalidate();
        }
    }
}