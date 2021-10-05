using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Automation_Game
{
    [Activity(Label = "GameActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class GameActivity : Activity
    {
        FrameLayout frame;
        LinearLayout craftingUI;

        public MapDraw Map { get; set; }

        Button displayMap;
        Button displayInventory;
        Button DisplayCraftingUI;
        Button[] slots;

        Dialog inventory;

        public static Bitmap spriteSheet { get; private set; }
        Bitmap minimapBitmap;

        OnTouchEvent press;
        bool IsMinimapShown;

        public StructureBlueprint UsedBlueprint;
        public Structure hole;

        Timer timer;
        const int MAX_FPS = 60;
        public List<IUpdateable> Updateables;

        bool updating;

        public enum IDs
        {
            DIRT,
            GRASS,
            WATER,
            SAND,
            PLAYER_WAKE,
            PLAYER_ASLEEP,
            STONE,
            STICK,
            TREE,
            PICKAXE = TREE + 3,
            SHOVEL,
            ROCK,
            TREE_SEED,
            DIRT_HOLE,
            CRAFTING_STATION,
            AXE,
            STORAGE_BOX,
        };

        protected override void OnStop()
        {
            //Map.Save();
            base.OnStop();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.game_map);
            frame = (FrameLayout)FindViewById(Resource.Id.frame);
            displayInventory = (Button)FindViewById(Resource.Id.openInventory);
            displayMap = (Button)FindViewById(Resource.Id.openMap);
            DisplayCraftingUI = (Button)FindViewById(Resource.Id.craftingMenu);
            displayInventory.FocusChange += FocusChange;
            displayMap.FocusChange += FocusChange;
            slots = new Button[9];
            IsMinimapShown = false;
            timer = new Timer(1000 / MAX_FPS);
            timer.Elapsed += Tick;
            Updateables = new List<IUpdateable>();
            updating = false;
            if (Intent.GetBooleanExtra("create", true))
            {
                Map = new MapDraw(this);
            }
            else
            {
                try
                {
                    int offset = 0;
                    using Stream stream = OpenFileInput("world.txt");
                    try
                    {
                        Byte[] readSize = new Byte[4];
                        stream.Read(readSize, offset, 4);
                        offset += 4;
                        Byte[] buffer = new Byte[BitConverter.ToInt32(readSize)];
                        stream.Read(buffer, 0, buffer.Length);
                        offset += buffer.Length;
                        Map.MapGenerator gen = new Map.MapGenerator(buffer, this);
                        stream.Read(readSize, 0, 4);
                        offset += 4;
                        buffer = new Byte[BitConverter.ToInt32(readSize)];
                        stream.Read(buffer, 0, buffer.Length);
                        Player p = new Player(buffer);
                        offset += buffer.Length;
                        stream.Read(readSize, 0, 4);
                        offset += 4;
                        buffer = new Byte[BitConverter.ToInt32(readSize)];
                        stream.Read(buffer, 0, buffer.Length);
                        Map = new MapDraw(this, gen, p);
                    }
                    catch (Java.IO.IOException e)
                    {
                        e.PrintStackTrace();
                        Map = new MapDraw(this);
                    }
                }
                catch (Java.IO.FileNotFoundException e)
                {
                    e.PrintStackTrace();
                    Map = new MapDraw(this);
                }
            }
            frame.AddView(Map);
            displayInventory.Click += DisplayInventory_Click;
            displayMap.Click += DisplayMap_Click;
            DisplayCraftingUI.Click += DisplayCraftingUI_Click;

            spriteSheet = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("sprite_sheet", "drawable", this.PackageName));
            timer.Start();
        }

        public void StartClock()
        {
            timer.Start();
        }

        public void StopClock()
        {
            timer.Stop();
        }

        private async void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!updating)
            {
                updating = true;
                List<Task> tasks = new List<Task>();
                foreach (IUpdateable updateable in Updateables)
                {
                    tasks.Add(updateable.Update());
                }
                await Task.WhenAll(tasks);
                for (int i = Updateables.Count - 1; i >= 0; i--)
                {
                    if (Updateables[i] is Plant p)
                    {
                        if (p.IsFullyGrown())
                        {
                            Updateables.RemoveAt(i);
                        }
                    }
                }
                Map.Invalidate();
                updating = false;
            }
        }

        private void FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                Console.WriteLine("has focus\n\n");
            }
        }

        public void Invalidate()
        {
            if (inventory != null && inventory.IsShowing)
            {
                inventory.Hide();
                displayInventory.CallOnClick();
            }

        }

        public void Trade(StorageChest storage, Player p)
        {
            Dialog d = new Dialog(this);
            LinearLayout ll1 = new LinearLayout(this);
            d.SetContentView(ll1);
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
        }

        private void Item_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DisplayCraftingUI_Click(object sender, EventArgs e)
        {
            if (craftingUI == null)
            {
                int width = Window.DecorView.Width;
                int height = (int)(displayInventory.Height);
                craftingUI = new LinearLayout(this);
                LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(width, height);
                craftingUI.LayoutParameters = param;
                craftingUI.SetBackgroundColor(Color.White);
                craftingUI.SetY(frame.Height - craftingUI.LayoutParameters.Height);
                Button close = new Button(this);
                close.SetWidth(width / 8);
                close.SetHeight(height);
                close.SetBackgroundColor(Color.Red);
                close.Background.Alpha = 150;
                close.Click += Close_Click;
                Button craftingStation = new Button(this);
                craftingStation.SetWidth(width / 8);
                craftingStation.SetHeight(height);
                Bitmap bs = Bitmap.CreateBitmap(width / 8, height, Bitmap.Config.Argb8888);
                Canvas c = new Canvas(bs);
                Rect src = new Rect(((int)IDs.CRAFTING_STATION % MapDraw.spriteSheetColoumnCount) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, ((int)IDs.CRAFTING_STATION / MapDraw.spriteSheetColoumnCount) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, ((int)IDs.CRAFTING_STATION % MapDraw.spriteSheetColoumnCount + 1) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, ((int)IDs.CRAFTING_STATION / MapDraw.spriteSheetColoumnCount + 1) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount);
                Rect dst = new Rect(0, 0, c.Width, c.Height);
                Paint p = new Paint();
                Color color = Color.CadetBlue;
                c.DrawColor(color);
                PorterDuffColorFilter cf = new PorterDuffColorFilter(color, PorterDuff.Mode.Multiply);
                p.SetColorFilter(cf);
                c.DrawBitmap(spriteSheet, src, dst, p);
                craftingStation.Background = new BitmapDrawable(Resources, bs);
                craftingStation.Click += CraftingStation_Click;
                bs = Bitmap.CreateBitmap(width / 8, height, Bitmap.Config.Argb8888);
                Button StorageBox = new Button(this);
                c = new Canvas(bs);
                c.DrawColor(color);
                src = new Rect(((int)IDs.STORAGE_BOX % MapDraw.spriteSheetColoumnCount) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, ((int)IDs.STORAGE_BOX / MapDraw.spriteSheetColoumnCount) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, ((int)IDs.STORAGE_BOX % MapDraw.spriteSheetColoumnCount + 1) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, ((int)IDs.STORAGE_BOX / MapDraw.spriteSheetColoumnCount + 1) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount);
                c.DrawBitmap(spriteSheet, src, dst, p);
                StorageBox.Background = new BitmapDrawable(Resources, bs);
                StorageBox.Click += StorageBox_Click;
                StorageBox.SetWidth(width / 8);
                craftingStation.SetHeight(height);
                craftingUI.AddView(close);
                craftingUI.AddView(craftingStation);
                craftingUI.AddView(StorageBox);
                frame.AddView(craftingUI);
            }
            craftingUI.Visibility = ViewStates.Visible;
            DisplayCraftingUI.Visibility = ViewStates.Invisible;
            displayInventory.Visibility = ViewStates.Invisible;
            Map.editMode = true;

        }

        private void StorageBox_Click(object sender, EventArgs e)
        {
            Structure s = new StorageChest(this);
            Delivery[] d = new Delivery[1];
            d[0] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK]), 4);
            Map.CurrentlyBuilding = new StructureBlueprint(s, d, null);
        }

        private void CraftingStation_Click(object sender, EventArgs e)
        {
            Structure s = new CraftingStation(this);
            Delivery[] d = new Delivery[1];
            d[0] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK]), 1);
            Map.CurrentlyBuilding = new StructureBlueprint(s, d, null);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            craftingUI.Visibility = ViewStates.Invisible;
            DisplayCraftingUI.Visibility = ViewStates.Visible;
            displayInventory.Visibility = ViewStates.Visible;
            Map.editMode = false;
        }

        private async void DisplayMap_Click(object sender, EventArgs e)
        {
            if (!IsMinimapShown)
            {
                IsMinimapShown = true;
                Dialog minimap = new Dialog(this);
                View background = new View(this);
                int size = (int)Math.Min(Window.DecorView.Width * 0.8, Window.DecorView.Height * 0.8);
                if (minimapBitmap == null)
                {
                    minimapBitmap = await Task.Run(() => Map.DisplayMap(size, size));
                }
                Bitmap mm = minimapBitmap.Copy(minimapBitmap.GetConfig(), true);
                Canvas c = new Canvas(mm);
                int spriteSheetSignleWidth = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                Player Player = Map.Player;
                int centerX = (int)(Player.GetX() / 100 * c.Width);
                int centerY = (int)(Player.GetY() / 100 * c.Height);
                int width = 3 * c.Width / Map.Generator.GetWidth();
                int height = 3 * c.Height / Map.Generator.GetHeight();
                int length = Math.Min(width, height);
                if (Window.DecorView.Width > Window.DecorView.Height)
                {
                    width = length / (Window.DecorView.Width / Window.DecorView.Height);
                }
                else
                {
                    height = length / (Window.DecorView.Height / Window.DecorView.Width);
                }
                Paint p = new Paint
                {
                    Color = Color.Red
                };
                c.DrawOval(centerX - width / 2, centerY - height / 2, centerX + width / 2, centerY + height / 2, p);
                background.Background = new BitmapDrawable(Resources, mm);
                minimap.SetContentView(background);
                minimap.SetCancelable(true);
                minimap.CancelEvent += Minimap_CancelEvent;
                minimap.SetTitle("");
                RunOnUiThread(() =>
                {
                    minimap.Show();
                    minimap.Window.SetLayout((int)(Window.DecorView.Width * 0.8), (int)(Window.DecorView.Height * 0.8));
                });

            }
        }

        private void Minimap_CancelEvent(object sender, EventArgs e)
        {
            IsMinimapShown = false;
        }

        public void Hide_Inventory()
        {
            if (inventory != null && inventory.IsShowing)
            {
                inventory.Hide();
            }
        }

        public void DisplayInventory_Click(object sender, EventArgs e)
        {
            if (inventory == null)
            {
                inventory = new Dialog(this);
                inventory.SetContentView(Resource.Layout.inventory_layout);
                inventory.SetCancelable(true);
                inventory.SetTitle("");
                press = new OnTouchEvent(this);
                inventory.Window.SetLayout((int)(Window.DecorView.Width * 0.8), (int)(Window.DecorView.Height * 0.8));
                slots[0] = (Button)inventory.FindViewById(Resource.Id.slot1);
                slots[0].SetOnTouchListener(press);
                slots[1] = (Button)inventory.FindViewById(Resource.Id.slot2);
                slots[1].SetOnTouchListener(press);
                slots[2] = (Button)inventory.FindViewById(Resource.Id.slot3);
                slots[2].SetOnTouchListener(press);
                slots[3] = (Button)inventory.FindViewById(Resource.Id.slot4);
                slots[3].SetOnTouchListener(press);
                slots[4] = (Button)inventory.FindViewById(Resource.Id.slot5);
                slots[4].SetOnTouchListener(press);
                slots[5] = (Button)inventory.FindViewById(Resource.Id.slot6);
                slots[5].SetOnTouchListener(press);
                slots[6] = (Button)inventory.FindViewById(Resource.Id.slot7);
                slots[6].SetOnTouchListener(press);
                slots[7] = (Button)inventory.FindViewById(Resource.Id.slot8);
                slots[7].SetOnTouchListener(press);
                slots[8] = (Button)inventory.FindViewById(Resource.Id.slotEquip);
                slots[8].Click += DeEquip;
            }
            press.SetBlueprint(UsedBlueprint);
            press.SetStructure(hole);
            UsedBlueprint = null;
            hole = null;
            Item[] inv = Map.Player.GetInvetory();
            int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
            Bitmap icon;
            for (int i = 0; i < inv.Length; i++)
            {
                icon = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("inventory_slot_border", "drawable", PackageName)).Copy(Bitmap.Config.Argb8888, true);
                if (inv[i] != null)
                {
                    Canvas c = new Canvas(icon);
                    Rect src = new Rect((inv[i].id % MapDraw.spriteSheetColoumnCount) * iconSize, (inv[i].id / MapDraw.spriteSheetColoumnCount) * iconSize, (inv[i].id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (inv[i].id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
                    int left = c.Width / 11;
                    int right = c.Width / 11 * 10;
                    int top = c.Height / 11;
                    int bottom = c.Height / 11 * 10;
                    Rect dst = new Rect(left, top, right, bottom);
                    Bitmap item = Bitmap.CreateBitmap(spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
                    Bitmap scaled = Bitmap.CreateScaledBitmap(item, right - left, bottom - top, false);
                    c.DrawBitmap(scaled, null, dst, null);
                    if (inv[i] is Tool tool)
                    {
                        Paint p = new Paint();
                        p.SetStyle(Paint.Style.FillAndStroke);
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
                        c.DrawRect(left, top * 9, (float)(right * percentage), bottom, p);
                    }
                }
                slots[i].Background = new BitmapDrawable(Resources, icon);
                icon.Dispose();
            }
            Item equipment = Map.Player.GetEquippedItem();
            icon = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("inventory_slot_border", "drawable", PackageName)).Copy(Bitmap.Config.Argb8888, true);
            if (equipment != null)
            {
                Tool tool = (Tool)equipment;
                Canvas c = new Canvas(icon);
                Rect src = new Rect((equipment.id % MapDraw.spriteSheetColoumnCount) * iconSize, (equipment.id / MapDraw.spriteSheetColoumnCount) * iconSize, (equipment.id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (equipment.id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
                int left = c.Width / 11;
                int right = c.Width / 11 * 10;
                int top = c.Height / 11;
                int bottom = c.Height / 11 * 10;
                Rect dst = new Rect(left, top, right, bottom);
                Bitmap item = Bitmap.CreateBitmap(spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
                Bitmap scaled = Bitmap.CreateScaledBitmap(item, right - left, bottom - top, false);
                c.DrawBitmap(scaled, null, dst, null);
                Paint p = new Paint();
                p.SetStyle(Paint.Style.FillAndStroke);
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
                c.DrawRect(left, top * 9, (float)(right * percentage), bottom, p);
            }
            slots[8].Background = new BitmapDrawable(Resources, icon);
            icon.Dispose();
            inventory.Show();
        }

        private void DeEquip(object sender, EventArgs e)
        {
            Map.Player.DeEquip();
            Invalidate();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                RelativeLayout rl = (RelativeLayout)FindViewById(Resource.Id.relativeLayout1);
                rl.BringChildToFront(displayInventory);
                rl.BringChildToFront(displayMap);
                rl.BringChildToFront(DisplayCraftingUI);

            }
            return base.OnTouchEvent(e);
        }
    }
}