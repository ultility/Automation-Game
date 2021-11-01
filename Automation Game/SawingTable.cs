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
using System.Threading.Tasks;

namespace Automation_Game
{
    class SawingTable : Structure, IUseable, IUpdateable
    {
        List<CraftingRecipe> Recipes;
        Context C;
        Dialog d;
        Button[] slots;
        Button Input;
        Button Output;
        ProccessBar bar;
        public Item InputItem { get; private set; }
        public Item OutputItem { get; private set; }
        public SawingTable(Context context, string name = "SawingTable", int id = (int)GameActivity.IDs.SAWING_TABLE, int size = 1, Item useableItem = null, Item droppedItem = null) : base(name, id, size, useableItem, droppedItem, 0)
        {
            Recipes = new List<CraftingRecipe>();
            Recipes.Add(new CraftingRecipe(ItemType.Create((int)ItemType.ItemTypes.STICK), new Delivery(ItemType.Create((int)ItemType.ItemTypes.LOG), 1)));
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
            bar = new ProccessBar(C, BitmapFactory.DecodeResource(C.Resources, Resource.Drawable.proccess_bar_empty), BitmapFactory.DecodeResource(C.Resources, Resource.Drawable.proccess_bar_filled));
            ViewGroup parent = (ViewGroup)Output.Parent;
            ((Activity)C).RunOnUiThread(() =>
            {
                parent.AddView(bar, parent.ChildCount - 2);
            });
        }

        public Task Update()
        {

            if (InputItem != null && OutputItem == null)
            {
                return Task.Run(() =>
                {
                    bar.Progress++;
                    if (bar.Progress == 100)
                    {
                        foreach (CraftingRecipe recipe in Recipes)
                        {
                            if (recipe.Recipe[0].item.Equals(InputItem))
                            {
                                OutputItem = new Item(recipe.Result);
                                InputItem = null;
                                UpdateButton(Output, OutputItem);
                                UpdateButton(Input, InputItem);
                                bar.Progress = 0;
                                break;
                            }
                        }
                    }
                });
            }
            else if (InputItem == null)
            {
                bar.Progress = 0;
            }
            return Task.CompletedTask;
        }

        public void Use(Player p)
        {
            Use(p, true);
        }

        private void Use(Player p, bool show)
        {
            ((Activity)C).RunOnUiThread(() =>
            {
                if (Input.Width == 0)
                {
                    for (int i = 0; i < slots.Length; i++)
                    {
                        Button btn = slots[i];
                        btn.Tag = i;
                        btn.Click += (Object obj, EventArgs e) =>
                        {
                            int tag = (int)btn.Tag;
                            Item Pushed = p.GetInvetory()[tag];
                            if (InputItem == null && Pushed != null)
                            {
                                foreach (CraftingRecipe recipe in Recipes)
                                {
                                    if (recipe.Recipe[0].item.Equals(Pushed))
                                    {
                                        InputItem = p.DropItem(tag);
                                    }
                                    break;
                                }
                            }
                            Use(p, false);
                        };
                    }
                    Output.Click += (Object obj, EventArgs e) =>
                    {
                        if (p.GiveItem(OutputItem))
                        {
                            OutputItem = null;
                            Use(p, false);
                        }
                    };
                    Input.Click += (Object obj, EventArgs e) =>
                    {
                        if (p.GiveItem(InputItem))
                        {
                            InputItem = null;
                            Use(p, false);
                        }
                    };
                }
                ViewGroup parent = (ViewGroup)Output.Parent;
                RelativeLayout.LayoutParams param = new RelativeLayout.LayoutParams(parent.Width - ((Output.Width + Input.Width) * 2), Output.Height);
                param.AddRule(LayoutRules.RightOf, Input.Id);
                param.LeftMargin = (int)((Output.Width + Input.Width) * 0.5);
                bar.LayoutParameters = param;
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
                            Paint p = new Paint();
                            p.SetStyle(Paint.Style.FillAndStroke);
                            double percentage = 1;
                            if (tool.name.Equals("axe", StringComparison.OrdinalIgnoreCase))
                            {
                                percentage = (double)tool.durability / ItemType.itemTypeList[(int)ItemType.ItemTypes.AXE].durability;
                            }
                            else if (tool.name.Equals("pickaxe", StringComparison.OrdinalIgnoreCase))
                            {
                                percentage = (double)tool.durability / ItemType.itemTypeList[(int)ItemType.ItemTypes.PICKAXE].durability;
                            }
                            else if (tool.name.Equals("pickaxe", StringComparison.OrdinalIgnoreCase))
                            {
                                percentage = (double)tool.durability / ItemType.itemTypeList[(int)ItemType.ItemTypes.SHOVEL].durability;
                            }
                            if (percentage > .50)
                            {
                                p.Color = Color.Green;
                            }
                            else if (percentage > .20)
                            {
                                p.Color = Color.Orange;
                            }
                            else
                            {
                                p.Color = Color.Red;
                            }
                            Canvas c = new Canvas(icon);
                            int left = c.Width / 11;
                            int right = c.Width / 11 * 10;
                            int top = c.Height / 11;
                            int bottom = c.Height / 11 * 10;
                            c.DrawRect(left, top * 9, (float)(right * percentage), bottom, p);
                        }
                    }
                    else
                    {
                        icon = GameActivity.GetIcon(-1, C);
                    }
                    slots[i].Background = new BitmapDrawable(C.Resources, icon);
                    icon.Dispose();

                }
                UpdateButton(Output, OutputItem);
                UpdateButton(Input, InputItem);
                if (show)
                {
                    d.Show();
                }

            });
        }

        private void UpdateButton(Button btn, Item itm)
        {
            Bitmap icon;
            if (itm != null)
            {
                icon = GameActivity.GetIcon(itm.id, C);
                if (itm is Tool tool)
                {
                    Paint paint = new Paint();
                    paint.SetStyle(Paint.Style.FillAndStroke);
                    double percentage = 1;
                    if (tool.name.Equals("axe", StringComparison.OrdinalIgnoreCase))
                    {
                        percentage = (double)tool.durability / ItemType.itemTypeList[(int)ItemType.ItemTypes.AXE].durability;
                    }
                    else if (tool.name.Equals("pickaxe", StringComparison.OrdinalIgnoreCase))
                    {
                        percentage = (double)tool.durability / ItemType.itemTypeList[(int)ItemType.ItemTypes.PICKAXE].durability;
                    }
                    else if (tool.name.Equals("pickaxe", StringComparison.OrdinalIgnoreCase))
                    {
                        percentage = (double)tool.durability / ItemType.itemTypeList[(int)ItemType.ItemTypes.SHOVEL].durability;
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
            btn.Background = new BitmapDrawable(C.Resources, icon);
            icon.Dispose();
        }
    }
}