using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Automation_Game
{
    public class StorageChest : Structure, IUseable
    {
        Inventory inv;
        Context context;
        Dialog d;
        public StorageChest(Context c) : base("Chest", (int)GameActivity.IDs.STORAGE_BOX, 1, new Item("Axe", (int)GameActivity.IDs.AXE, 1), (Item)null, 1)
        {
            inv = new Inventory(16);
            context = c;
            d = new Dialog(context);
        }

        public bool AddItem(Item item)
        {
            return inv.AddItem(item);
        }

        public Item RemoveItem(int n)
        {
            return inv.RemoveItem(n, true);
        }

        public void Use(Player p)
        {
            ((Activity)context).RunOnUiThread(() =>
            {
                LinearLayout ll1 = new LinearLayout(context);
                d.SetContentView(ll1);
                int WindowWidth = (int)(((Activity)context).Window.DecorView.Width * 0.8);
                int WindowHeight = (int)(((Activity)context).Window.DecorView.Height * 0.8);
                d.Window.SetLayout(WindowWidth, WindowHeight);
                ScrollView left = new ScrollView(context, Orientation.Vertical);
                ScrollView right = new ScrollView(context, Orientation.Vertical);
                ImageView divider = new ImageView(context);
                left.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11 * 5, ViewGroup.LayoutParams.MatchParent);
                right.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11 * 5, WindowHeight);
                divider.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11, WindowHeight);
                ll1.AddView(left);
                ll1.AddView(divider);
                ll1.AddView(right);
                divider.Background = new ColorDrawable(Color.Black);

                Item[] player_inventory = p.GetInvetory();
                Item[] storage_inventory = GetInventory();
                LinearLayout.LayoutParams LayoutParam = new LinearLayout.LayoutParams(left.LayoutParameters.Width / 7 * 5, (int)(left.LayoutParameters.Width / 7 * (4.0 / 3)));
                LayoutParam.LeftMargin = left.LayoutParameters.Width / 7;
                LayoutParam.TopMargin = left.LayoutParameters.Width / 14;
                LinearLayout.LayoutParams ButtonParams = new LinearLayout.LayoutParams((int)(left.LayoutParameters.Width / 7 * (4.0 / 3)), (int)(left.LayoutParameters.Width / 7 * (4.0 / 3)));
                LinearLayout ItemLine = null;
                LinearLayout LeftMain = new LinearLayout(context);
                LeftMain.Orientation = Orientation.Vertical;
                LeftMain.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, ButtonParams.Height * (int)Math.Ceiling(storage_inventory.Length / 2.0) + LayoutParam.TopMargin * (int)Math.Ceiling(storage_inventory.Length / 2.0));
                LinearLayout RightMain = new LinearLayout(context);
                RightMain.Orientation = Orientation.Vertical;
                RightMain.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, ButtonParams.Height * (int)Math.Ceiling(player_inventory.Length / 2.0) + LayoutParam.TopMargin * (int)Math.Ceiling(player_inventory.Length / 2.0));
                left.AddView(LeftMain);
                right.AddView(RightMain);
                for (int i = 0; i < storage_inventory.Length; i++)
                {
                    Button item = new Button(context);
                    if (i % 2 == 0)
                    {
                        ItemLine = new LinearLayout(context);
                        ItemLine.LayoutParameters = LayoutParam;
                        ButtonParams.LeftMargin = 0;
                        LeftMain.AddView(ItemLine);
                    }
                    else
                    {
                        ButtonParams.LeftMargin = left.LayoutParameters.Width / 7;
                    }
                    item.LayoutParameters = ButtonParams;
                    Bitmap icon = BitmapFactory.DecodeResource(context.Resources, context.Resources.GetIdentifier("inventory_slot_border", "drawable", context.PackageName)).Copy(Bitmap.Config.Argb8888, true);
                    int iconSize = GameActivity.spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                    if (storage_inventory[i] != null)
                    {
                        Canvas c = new Canvas(icon);
                        Rect src = new Rect((storage_inventory[i].id % MapDraw.spriteSheetColoumnCount) * iconSize, (storage_inventory[i].id / MapDraw.spriteSheetColoumnCount) * iconSize, (storage_inventory[i].id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (storage_inventory[i].id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
                        int rectleft = c.Width / 11;
                        int rectright = c.Width / 11 * 10;
                        int recttop = c.Height / 11;
                        int rectbottom = c.Height / 11 * 10;
                        Rect dst = new Rect(rectleft, recttop, rectright, rectbottom);
                        Bitmap itemIcon = Bitmap.CreateBitmap(GameActivity.spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
                        Bitmap scaled = Bitmap.CreateScaledBitmap(itemIcon, rectright - rectleft, rectbottom - recttop, false);
                        c.DrawBitmap(scaled, null, dst, null);
                        if (storage_inventory[i] is Tool tool)
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
                            c.DrawRect(rectleft, recttop * 9, (float)(rectright * percentage), rectbottom, paint);
                        }
                    }
                    item.Background = new BitmapDrawable(context.Resources, icon);
                    item.Click += (object sender, EventArgs e) =>
                    {
                        int index = (int)((View)sender).Tag;
                        if (storage_inventory[index] != null)
                        {
                            if (p.GiveItem(storage_inventory[index]))
                            {
                                RemoveItem(index);
                                Use(p);
                            }
                        }
                    };
                    icon.Dispose();
                    ItemLine.AddView(item);
                }

                for (int i = 0; i < player_inventory.Length; i++)
                {
                    Button item = new Button(context);
                    if (i % 2 == 0)
                    {
                        ItemLine = new LinearLayout(context);
                        ItemLine.LayoutParameters = LayoutParam;
                        ButtonParams.LeftMargin = 0;
                        RightMain.AddView(ItemLine);
                    }
                    else
                    {
                        ButtonParams.LeftMargin = left.LayoutParameters.Width / 7;
                    }
                    item.LayoutParameters = ButtonParams;
                    Bitmap icon = BitmapFactory.DecodeResource(context.Resources, context.Resources.GetIdentifier("inventory_slot_border", "drawable", context.PackageName)).Copy(Bitmap.Config.Argb8888, true);
                    int iconSize = GameActivity.spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                    if (player_inventory[i] != null)
                    {
                        Canvas c = new Canvas(icon);
                        Rect src = new Rect((player_inventory[i].id % MapDraw.spriteSheetColoumnCount) * iconSize, (player_inventory[i].id / MapDraw.spriteSheetColoumnCount) * iconSize, (player_inventory[i].id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (player_inventory[i].id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
                        int rectleft = c.Width / 11;
                        int rectright = c.Width / 11 * 10;
                        int recttop = c.Height / 11;
                        int rectbottom = c.Height / 11 * 10;
                        Rect dst = new Rect(rectleft, recttop, rectright, rectbottom);
                        Bitmap itemIcon = Bitmap.CreateBitmap(GameActivity.spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
                        Bitmap scaled = Bitmap.CreateScaledBitmap(itemIcon, rectright - rectleft, rectbottom - recttop, false);
                        c.DrawBitmap(scaled, null, dst, null);
                        if (player_inventory[i] is Tool tool)
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
                            c.DrawRect(rectleft, recttop * 9, (float)(rectright * percentage), rectbottom, paint);
                        }
                    }
                    item.Background = new BitmapDrawable(context.Resources, icon);
                    item.Tag = i;
                    item.Click += (object sender, EventArgs e) =>
                    {
                        int index = (int)((View)sender).Tag;
                        if (player_inventory[index] != null)
                        {
                            if (AddItem(player_inventory[index]))
                            {
                                p.DropItem(index);
                                Use(p);
                            }
                        }
                    };
                    icon.Dispose();
                    ItemLine.AddView(item);
                }

                d.Show();
            });
        }

        public Item[] GetInventory()
        {
            Item[] inventory = new Item[inv.size];
            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = inv.GetItem(i);
            }
            return inventory;
        }
    }
}