using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
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
    class SawingTable : Structure, IUseable
    {
        CraftingRecipe Recipes;
        Context C;
        Dialog d;
        Button[] slots;
        Button Input;
        Button Output;
        ProccessBar bar;
        public SawingTable(Context context, string name = "SawingTable", int id = (int)GameActivity.IDs.SAWING_TABLE, int size = 1, Item useableItem = null, Item droppedItem = null) : base(name, id, size, useableItem, droppedItem, 0)
        {
            C = context;
            slots = new Button[8];
            d = new Dialog(C);
            d.SetContentView(Resource.Layout.proccessing_layout);
            d.SetCancelable(true);
            d.Window.SetLayout((int)(((Activity)C).Window.DecorView.Width * 0.8), (int)(((Activity)C).Window.DecorView.Height * 0.8));
            slots[0] = (Button)d.FindViewById(Resource.Id.slot1);
            slots[1] = (Button)d.FindViewById(Resource.Id.slot2);
            slots[2] = (Button)d.FindViewById(Resource.Id.slot3);
            slots[3] = (Button)d.FindViewById(Resource.Id.slot4);
            slots[4] = (Button)d.FindViewById(Resource.Id.slot5);
            slots[5] = (Button)d.FindViewById(Resource.Id.slot6);
            slots[6] = (Button)d.FindViewById(Resource.Id.slot7);
            slots[7] = (Button)d.FindViewById(Resource.Id.slot8);
            Output = (Button)d.FindViewById(Resource.Id.output);
            Input = (Button)d.FindViewById(Resource.Id.input);
            bar = new ProccessBar(C);
            ViewGroup parent = (ViewGroup)Output.Parent;
            parent.AddView(bar, parent.ChildCount - 2);
        }

        public void Use(Player p)
        {
            ((Activity)C).RunOnUiThread(() =>
            {
                ViewGroup parent = (ViewGroup)Output.Parent;
                bar.LayoutParameters = new LinearLayout.LayoutParams(parent.Width - Output.Width - Input.Width, parent.Height - Input.Height - Output.Height);
                Item[] inv = p.GetInvetory();
                int iconSize = GameActivity.spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                Bitmap icon;
                for (int i = 0; i < inv.Length; i++)
                {
                    if (inv[i] != null)
                    {
                        icon = GameActivity.GetIcon(inv[i].id, C);
                        if (inv[i] is Tool tool)
                        {
                            Paint paint = new Paint();
                            paint.SetStyle(Paint.Style.FillAndStroke);
                            double percentage = 1;
                            if (tool.name.Equals("axe", StringComparison.OrdinalIgnoreCase))
                            {
                                percentage = (double)tool.durability / CraftingStation.AXE_DURABILITY;
                            }
                            else if (tool.name.Equals("pickaxe", StringComparison.OrdinalIgnoreCase))
                            {
                                percentage = (double)tool.durability / CraftingStation.PICKAXE_DURABILITY;
                            }
                            else if (tool.name.Equals("pickaxe", StringComparison.OrdinalIgnoreCase))
                            {
                                percentage = (double)tool.durability / CraftingStation.SHOVEL_DURABILITY;
                            }
                            if (percentage > .50)
                            {
                                paint.Color = Color.Green;
                            }
                            else if (percentage > .20)
                            {
                                paint.Color = Color.Orange;
                            }
                            else
                            {
                                paint.Color = Color.Red;
                            }
                            Canvas c = new Canvas(icon);
                            int left = c.Width / 11;
                            int right = c.Width / 11 * 10;
                            int top = c.Height / 11;
                            int bottom = c.Height / 11 * 10;
                            c.DrawRect(left, top * 9, (float)(right * percentage), bottom, paint);
                        }
                    }
                    else
                    {
                        icon = GameActivity.GetIcon(-1, C);
                    }
                    slots[i].Background = new BitmapDrawable(C.Resources, icon);
                    icon.Dispose();
                }
                d.Show();
                
            });
        }
    }
}