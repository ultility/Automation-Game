using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using System;

namespace Automation_Game
{
    class CraftingStation : Structure
    {
        public const int AXE_DURABILITY = 20;
        public const int PICKAXE_DURABILITY = 20;
        public const int SHOVEL_DURABILITY = 20;
        readonly CraftingRecipe[] recipes;
        readonly Context c;
        Player p;
        public CraftingStation(Context context, string name = "CraftingStation", int id = (int)GameActivity.IDs.CRAFTING_STATION, int size = 1, Item useableItem = null, Item droppedItem = null) : base(name, id, size, useableItem, droppedItem, 0)
        {
            recipes = new CraftingRecipe[3];
            Delivery[] axeRecipe = new Delivery[2];
            axeRecipe[0] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].name, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].id, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].sizePercentage), 1);
            axeRecipe[1] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].name, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].id, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].sizePercentage), 1);
            recipes[0] = new CraftingRecipe(new Tool("Axe", (int)GameActivity.IDs.AXE, AXE_DURABILITY), axeRecipe);
            recipes[1] = new CraftingRecipe(new Tool("Pickaxe", (int)GameActivity.IDs.PICKAXE, PICKAXE_DURABILITY), axeRecipe);
            recipes[2] = new CraftingRecipe(new Tool("Shovel", (int)GameActivity.IDs.SHOVEL, SHOVEL_DURABILITY), axeRecipe);
            c = context;
        }

        public void Use(Player p)
        {
            this.p = p;
            ((Activity)c).RunOnUiThread(DisplayMenu);
        }

        private void DisplayMenu()
        {
            AlertDialog d;
            AlertDialog.Builder build = new AlertDialog.Builder(c);
            build.SetCancelable(true);
            LinearLayout l1 = new LinearLayout(c)
            {
                LayoutParameters = new LinearLayout.LayoutParams(((Activity)c).Window.DecorView.Width * 8 / 10, ((Activity)c).Window.DecorView.Height * 8 / 10)
            };
            build.SetView(l1);
            for (int i = 0; i < recipes.Length; i++)
            {
                Button tool = new Button(c);
                Bitmap spriteSheet = BitmapFactory.DecodeResource(c.Resources, c.Resources.GetIdentifier("sprite_sheet", "drawable", c.PackageName));
                int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                Bitmap icon = Bitmap.CreateBitmap(spriteSheet, iconSize * (recipes[i].Result.id % MapDraw.spriteSheetColoumnCount), iconSize * (recipes[i].Result.id / MapDraw.spriteSheetColoumnCount), iconSize, iconSize);
                tool.Background = new BitmapDrawable(c.Resources, icon);
                tool.Click += Axe_Click;
                tool.Tag = i;
                l1.AddView(tool);
                spriteSheet.Dispose();
            }
            d = build.Create();
            d.Show();
            d.Window.SetLayout(((Activity)c).Window.DecorView.Width * 8 / 10, ((Activity)c).Window.DecorView.Height * 8 / 10);
        }
        /*private void displaymenu()
        {
            Dialog d = new Dialog(c);
            LinearLayout ll1 = new LinearLayout(c);
            d.SetContentView(ll1);
            View
            int WindowWidth = (int)(Window.DecorView.Width * 0.8);
            int WindowHeight = (int)(Window.DecorView.Height * 0.8);
            d.Window.SetLayout(WindowWidth, WindowHeight);
            ScrollView left = new ScrollView(this);
            ScrollView right = new ScrollView(this);
            ImageView divider = new ImageView(this);
            left.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11 * 5, ViewGroup.LayoutParams.MatchParent);
            right.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11 * 5, WindowHeight);
            divider.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11, WindowHeight);
            ll1.AddView(left);
            ll1.AddView(divider);
            ll1.AddView(right);
            divider.Background = new ColorDrawable(Color.Black);

            Item[] player_inventory = p.GetInvetory();
            Item[] storage_inventory = storage.GetInventory();
            LinearLayout.LayoutParams LayoutParam = new LinearLayout.LayoutParams(left.LayoutParameters.Width / 7 * 5, (int)(left.LayoutParameters.Width / 7 * (4.0 / 3)));
            LayoutParam.LeftMargin = left.LayoutParameters.Width / 7;
            LayoutParam.TopMargin = left.LayoutParameters.Width / 14;
            LinearLayout.LayoutParams ButtonParams = new LinearLayout.LayoutParams((int)(left.LayoutParameters.Width / 7 * (4.0 / 3)), (int)(left.LayoutParameters.Width / 7 * (4.0 / 3)));
            LinearLayout ItemLine = null;
            LinearLayout LeftMain = new LinearLayout(this);
            LeftMain.Orientation = Orientation.Vertical;
            LeftMain.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, ButtonParams.Height * (int)Math.Ceiling(storage_inventory.Length / 2.0) + LayoutParam.TopMargin * (int)Math.Ceiling(storage_inventory.Length / 2.0));
            LinearLayout RightMain = new LinearLayout(this);
            RightMain.Orientation = Orientation.Vertical;
            RightMain.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, ButtonParams.Height * (int)Math.Ceiling(player_inventory.Length / 2.0) + LayoutParam.TopMargin * (int)Math.Ceiling(player_inventory.Length / 2.0));
            left.AddView(LeftMain);
            right.AddView(RightMain);
            for (int i = 0; i < storage_inventory.Length; i++)
            {
                Button item = new Button(this);
                if (i % 2 == 0)
                {
                    ItemLine = new LinearLayout(this);
                    ItemLine.LayoutParameters = LayoutParam;
                    ButtonParams.LeftMargin = 0;
                    LeftMain.AddView(ItemLine);
                }
                else
                {
                    ButtonParams.LeftMargin = left.LayoutParameters.Width / 7;
                }
                item.LayoutParameters = ButtonParams;
                Bitmap icon = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("inventory_slot_border", "drawable", PackageName)).Copy(Bitmap.Config.Argb8888, true);
                int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                if (storage_inventory[i] != null)
                {
                    Canvas c = new Canvas(icon);
                    Rect src = new Rect((storage_inventory[i].id % MapDraw.spriteSheetColoumnCount) * iconSize, (storage_inventory[i].id / MapDraw.spriteSheetColoumnCount) * iconSize, (storage_inventory[i].id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (storage_inventory[i].id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
                    int rectleft = c.Width / 11;
                    int rectright = c.Width / 11 * 10;
                    int recttop = c.Height / 11;
                    int rectbottom = c.Height / 11 * 10;
                    Rect dst = new Rect(rectleft, recttop, rectright, rectbottom);
                    Bitmap itemIcon = Bitmap.CreateBitmap(spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
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
                item.Background = new BitmapDrawable(Resources, icon);
                item.Click += (object sender, EventArgs e) =>
                {
                    int index = (int)((View)sender).Tag;
                    if (storage_inventory[index] != null)
                    {
                        if (p.GiveItem(storage_inventory[index]))
                        {
                            storage.RemoveItem(index);
                            d.Cancel();
                            Trade(storage, p);
                        }
                    }
                };
                icon.Dispose();
                ItemLine.AddView(item);
            }

            for (int i = 0; i < player_inventory.Length; i++)
            {
                Button item = new Button(this);
                if (i % 2 == 0)
                {
                    ItemLine = new LinearLayout(this);
                    ItemLine.LayoutParameters = LayoutParam;
                    ButtonParams.LeftMargin = 0;
                    RightMain.AddView(ItemLine);
                }
                else
                {
                    ButtonParams.LeftMargin = left.LayoutParameters.Width / 7;
                }
                item.LayoutParameters = ButtonParams;
                Bitmap icon = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("inventory_slot_border", "drawable", PackageName)).Copy(Bitmap.Config.Argb8888, true);
                int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                if (player_inventory[i] != null)
                {
                    Canvas c = new Canvas(icon);
                    Rect src = new Rect((player_inventory[i].id % MapDraw.spriteSheetColoumnCount) * iconSize, (player_inventory[i].id / MapDraw.spriteSheetColoumnCount) * iconSize, (player_inventory[i].id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (player_inventory[i].id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
                    int rectleft = c.Width / 11;
                    int rectright = c.Width / 11 * 10;
                    int recttop = c.Height / 11;
                    int rectbottom = c.Height / 11 * 10;
                    Rect dst = new Rect(rectleft, recttop, rectright, rectbottom);
                    Bitmap itemIcon = Bitmap.CreateBitmap(spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
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
                item.Background = new BitmapDrawable(Resources, icon);
                item.Tag = i;
                item.Click += (object sender, EventArgs e) =>
                {
                    int index = (int)((View)sender).Tag;
                    if (player_inventory[index] != null)
                    {
                        if (storage.AddItem(player_inventory[index]))
                        {
                            p.DropItem(index);
                            d.Cancel();
                            Trade(storage, p);
                        }
                    }
                };
                icon.Dispose();
                ItemLine.AddView(item);
            }

            d.Show();
        }*/

        private void Axe_Click(object sender, EventArgs e)
        {
            recipes[(int)((Button)sender).Tag].Craft(p);
        }
    }
}