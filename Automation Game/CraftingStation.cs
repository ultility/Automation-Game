using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System;

namespace Automation_Game
{
    class CraftingStation : Structure, IUseable
    {
        readonly CraftingRecipe[] recipes;
        readonly Context c;
        Player p;
        Dialog d;
        public CraftingStation(Context context, string name = "CraftingStation", int id = (int)GameActivity.IDs.CRAFTING_STATION, int size = 1, Item useableItem = null, Item droppedItem = null) : base(name, id, size, useableItem, droppedItem, 0)
        {
            recipes = new CraftingRecipe[3];
            Delivery[] axeRecipe = new Delivery[2];
            axeRecipe[0] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.STICK), 1);
            axeRecipe[1] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.STONE), 1);
            recipes[0] = new CraftingRecipe((Tool)ItemType.Create((int)ItemType.ItemTypes.AXE), axeRecipe);
            recipes[1] = new CraftingRecipe((Tool)ItemType.Create((int)ItemType.ItemTypes.PICKAXE), axeRecipe);
            recipes[2] = new CraftingRecipe((Tool)ItemType.Create((int)ItemType.ItemTypes.SHOVEL), axeRecipe);
            c = context;
            d = new Dialog(c);
        }

        public void Use(Player p)
        {
            this.p = p;
            ((Activity)c).RunOnUiThread(DisplayMenu);
        }
        private void DisplayMenu()
        {
            LinearLayout ll1 = new LinearLayout(c);
            d.SetContentView(ll1);
            Window Window = ((GameActivity)c).Window;
            int WindowWidth = (int)(Window.DecorView.Width * 0.8);
            int WindowHeight = (int)(Window.DecorView.Height * 0.8);
            d.Window.SetLayout(WindowWidth, WindowHeight);
            ScrollView left = new ScrollView(c, Orientation.Vertical);
            ScrollView right = new ScrollView(c, Orientation.Vertical);
            ImageView divider = new ImageView(c);
            left.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11 * 5, ViewGroup.LayoutParams.MatchParent);
            right.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11 * 5, WindowHeight);
            divider.LayoutParameters = new LinearLayout.LayoutParams(WindowWidth / 11, WindowHeight);
            ll1.AddView(left);
            ll1.AddView(divider);
            ll1.AddView(right);
            divider.Background = new ColorDrawable(Color.Black);

            Item[] player_inventory = p.GetInvetory();
            LinearLayout.LayoutParams LayoutParam = new LinearLayout.LayoutParams(left.LayoutParameters.Width / 7 * 5, (int)(left.LayoutParameters.Width / 7 * (4.0 / 3)));
            LayoutParam.LeftMargin = left.LayoutParameters.Width / 7;
            LayoutParam.TopMargin = left.LayoutParameters.Width / 14;
            LinearLayout.LayoutParams ButtonParams = new LinearLayout.LayoutParams((int)(left.LayoutParameters.Width / 7 * (4.0 / 3)), (int)(left.LayoutParameters.Width / 7 * (4.0 / 3)));
            LinearLayout ItemLine = null;
            LinearLayout LeftMain = new LinearLayout(c);
            LeftMain.Orientation = Orientation.Vertical;
            LeftMain.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, ButtonParams.Height * (int)Math.Ceiling(recipes.Length / 2.0) + LayoutParam.TopMargin * (int)Math.Ceiling(recipes.Length / 2.0));
            LinearLayout RightMain = new LinearLayout(c);
            RightMain.Orientation = Orientation.Vertical;
            RightMain.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, ButtonParams.Height * (int)Math.Ceiling(player_inventory.Length / 2.0) + LayoutParam.TopMargin * (int)Math.Ceiling(player_inventory.Length / 2.0));
            left.AddView(LeftMain);
            right.AddView(RightMain);
            Android.Content.Res.Resources Resources = c.Resources;
            for (int i = 0; i < recipes.Length; i++)
            {
                Button item = new Button(c);
                if (i % 2 == 0)
                {
                    ItemLine = new LinearLayout(c);
                    ItemLine.LayoutParameters = LayoutParam;
                    ButtonParams.LeftMargin = 0;
                    LeftMain.AddView(ItemLine);
                }
                else
                {
                    ButtonParams.LeftMargin = left.LayoutParameters.Width / 7;
                }
                item.LayoutParameters = ButtonParams;
                Bitmap icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.inventory_slot_border).Copy(Bitmap.Config.Argb8888, true);
                int iconSize = GameActivity.spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                if (recipes[i] != null)
                {
                    Canvas c = new Canvas(icon);
                    Rect src = new Rect((recipes[i].Result.id % MapDraw.spriteSheetColoumnCount) * iconSize, (recipes[i].Result.id / MapDraw.spriteSheetColoumnCount) * iconSize, (recipes[i].Result.id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (recipes[i].Result.id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
                    int rectleft = c.Width / 11;
                    int rectright = c.Width / 11 * 10;
                    int recttop = c.Height / 11;
                    int rectbottom = c.Height / 11 * 10;
                    Rect dst = new Rect(rectleft, recttop, rectright, rectbottom);
                    Bitmap itemIcon = Bitmap.CreateBitmap(GameActivity.spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
                    Bitmap scaled = Bitmap.CreateScaledBitmap(itemIcon, rectright - rectleft, rectbottom - recttop, false);
                    c.DrawBitmap(scaled, null, dst, null);
                }
                item.Background = new BitmapDrawable(Resources, icon);
                item.Tag = i;
                item.Click += Axe_Click;
                icon.Dispose();
                ItemLine.AddView(item);
            }

            for (int i = 0; i < player_inventory.Length; i++)
            {
                Button item = new Button(c);
                if (i % 2 == 0)
                {
                    ItemLine = new LinearLayout(c);
                    ItemLine.LayoutParameters = LayoutParam;
                    ButtonParams.LeftMargin = 0;
                    RightMain.AddView(ItemLine);
                }
                else
                {
                    ButtonParams.LeftMargin = left.LayoutParameters.Width / 7;
                }
                item.LayoutParameters = ButtonParams;
                Bitmap icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.inventory_slot_border).Copy(Bitmap.Config.Argb8888, true);
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
                        c.DrawRect(rectleft, recttop * 9, (float)(rectright * percentage), rectbottom, paint);
                    }
                }
                item.Background = new BitmapDrawable(Resources, icon);
                icon.Dispose();
                ItemLine.AddView(item);
            }

            d.Show();
        }

        private void Axe_Click(object sender, EventArgs e)
        {
            int tool = (int)((Button)sender).Tag;
            if (recipes[tool].Craft(p))
            {
                DisplayMenu();
            }
        }
    }
}