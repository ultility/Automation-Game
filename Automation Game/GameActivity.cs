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
    public class GameActivity : Activity, IMenuItemOnMenuItemClickListener
    {
        FrameLayout frame;
        LinearLayout craftingUI;
        Intent MusicControl;

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

        public static bool debug = true;
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
            WOOD_LOG,
            SAWING_TABLE,
            WOOD_FLOOR,
            WOOD_WALL,
            AMOUNT
        };

        protected override void OnPause()
        {
            SendBroadcast(MusicControl);
            Console.WriteLine("stopped music");
            Map.Save();
            base.OnPause();
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
                        Map = MapSaveManager.Restore(buffer, this);
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
            MusicControl = new Intent("music");
            MusicControl.PutExtra("music", 1);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SendBroadcast(MusicControl);
            Console.WriteLine("started game music");
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

        public void Invalidate()
        {
            if (inventory != null && inventory.IsShowing)
            {
                inventory.Hide();
                displayInventory.CallOnClick();
            }

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
                ScrollView Structures = new ScrollView(this, Orientation.Horizontal);
                Structures.LayoutParameters = new LinearLayout.LayoutParams(5 * width / 8, height);
                Button close = new Button(this);
                close.SetWidth(width / 8);
                close.SetHeight(height);
                close.SetBackgroundColor(Color.Red);
                close.Background.Alpha = 150;
                close.Click += Close_Click;
                Button craftingStation = GetCraftingUIButton((int)IDs.CRAFTING_STATION, width / 8, height);
                craftingStation.Click += CraftingStation_Click;
                Button StorageBox = GetCraftingUIButton((int)IDs.STORAGE_BOX, width / 8, height);
                StorageBox.Click += StorageBox_Click;
                Button SawingTable = GetCraftingUIButton((int)IDs.SAWING_TABLE, width / 8, height);
                SawingTable.Click += SawingTable_Click;
                Button WoodWall = GetCraftingUIButton((int)IDs.WOOD_WALL, width / 8, height);
                WoodWall.Click += WoodWall_Click;
                Button WoodFloor = GetCraftingUIButton((int)IDs.WOOD_FLOOR, width / 8, height);
                WoodFloor.Click += WoodFloor_Click;
                craftingUI.AddView(close);
                craftingUI.AddView(Structures);
                Structures.AddView(craftingStation);
                Structures.AddView(StorageBox);
                Structures.AddView(SawingTable);
                Structures.AddView(WoodWall);
                Structures.AddView(WoodFloor);
                Button turn90Right = new Button(this);
                turn90Right.LayoutParameters = new ViewGroup.LayoutParams(width / 8, height);
                turn90Right.Background = new BitmapDrawable(Resources, BitmapFactory.DecodeResource(Resources, Resource.Drawable.turn_right));
                craftingUI.AddView(turn90Right);
                turn90Right.Click += Turn90Right_Click;
                frame.AddView(craftingUI);
            }
            craftingUI.Visibility = ViewStates.Visible;
            DisplayCraftingUI.Visibility = ViewStates.Invisible;
            displayInventory.Visibility = ViewStates.Invisible;
            Map.editMode = true;

        }

        private void Turn90Right_Click(object sender, EventArgs e)
        {
            Structure s = Map.Generator.TerrainMap[(int)Map.LastPoint.X, (int)Map.LastPoint.Y].GetStructure();
            if (s != null)
            {
                s.Rotation += 90;
                Map.Invalidate();
            }
        }

        private void WoodFloor_Click(object sender, EventArgs e)
        {
            Structure s = StructureType.Create((int)StructureType.StructureTypes.WOOD_FLOOR);
            Delivery[] d = new Delivery[1];
            d[0] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.LOG), 2);
            Map.CurrentlyBuilding = new StructureBlueprint(s, d, null);
        }

        private void WoodWall_Click(object sender, EventArgs e)
        {
            Structure s = StructureType.Create((int)StructureType.StructureTypes.WOOD_WALL);
            Delivery[] d = new Delivery[1];
            d[0] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.LOG), 2);
            Map.CurrentlyBuilding = new StructureBlueprint(s, d, null);
        }

        private void SawingTable_Click(object sender, EventArgs e)
        {
            Structure s = new SawingTable(this);
            Delivery[] d = new Delivery[2];
            d[0] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.LOG), 2);
            d[1] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.STONE), 1);
            Map.CurrentlyBuilding = new StructureBlueprint(s, d, null);
        }

        private Button GetCraftingUIButton(int id, int width, int height)
        {
            Button btn = new Button(this);
            btn.SetWidth(width / 8);
            btn.SetHeight(height);
            Bitmap bs = Bitmap.CreateBitmap(width / 8, height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bs);
            Rect src = new Rect((id % MapDraw.spriteSheetColoumnCount) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, (id / MapDraw.spriteSheetColoumnCount) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, (id % MapDraw.spriteSheetColoumnCount + 1) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount, (id / MapDraw.spriteSheetColoumnCount + 1) * spriteSheet.Width / MapDraw.spriteSheetColoumnCount);
            Rect dst = new Rect(0, 0, c.Width, c.Height);
            Paint p = new Paint();
            Color color = Color.CadetBlue;
            c.DrawColor(color);
            PorterDuffColorFilter cf = new PorterDuffColorFilter(color, PorterDuff.Mode.Multiply);
            p.SetColorFilter(cf);
            c.DrawBitmap(spriteSheet, src, dst, p);
            btn.Background = new BitmapDrawable(Resources, bs);
            return btn;
        }

        private void StorageBox_Click(object sender, EventArgs e)
        {
            Structure s = new StorageChest(this);
            Delivery[] d = new Delivery[1];
            d[0] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.STICK), 4);
            Map.CurrentlyBuilding = new StructureBlueprint(s, d, null);
        }

        private void CraftingStation_Click(object sender, EventArgs e)
        {
            Structure s = new CraftingStation(this);
            Delivery[] d = new Delivery[1];
            d[0] = new Delivery(ItemType.Create((int)ItemType.ItemTypes.STICK), 1);
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
                
                slots[1] = (Button)inventory.FindViewById(Resource.Id.slot2);
                
                slots[2] = (Button)inventory.FindViewById(Resource.Id.slot3);
                
                slots[3] = (Button)inventory.FindViewById(Resource.Id.slot4);
                
                slots[4] = (Button)inventory.FindViewById(Resource.Id.slot5);
                
                slots[5] = (Button)inventory.FindViewById(Resource.Id.slot6);
                
                slots[6] = (Button)inventory.FindViewById(Resource.Id.slot7);
                slots[7] = (Button)inventory.FindViewById(Resource.Id.slot8);
                
                slots[8] = (Button)inventory.FindViewById(Resource.Id.slotEquip);
                slots[8].Click += DeEquip;
                Button debug = (Button)inventory.FindViewById(Resource.Id.debug);
                if (!GameActivity.debug)
                {
                    debug.Visibility = ViewStates.Gone;
                }
                else
                {
                    debug.Click += Debug_Click;
                }
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
                if (inv[i] != null)
                {
                    slots[i].Tag = 0;
                    icon = GetIcon(inv[i].id, this);
                    if (inv[i] is Tool tool)
                    {
                        slots[i].Tag = 1;
                        slots[i].SetTextColor(Color.Argb(0, 0, 0, 0));
                        slots[i].Text = "" + i;
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
                    slots[i].Click += ((Object o, EventArgs e) => { OpenContextMenu(slots[i]); });
                }
                else
                {
                    icon = GetIcon(-1, this);
                }
                slots[i].Background = new BitmapDrawable(Resources, icon);
                icon.Dispose();
            }
            Item equipment = Map.Player.GetEquippedItem();
            icon = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("inventory_slot_border", "drawable", PackageName)).Copy(Bitmap.Config.Argb8888, true);
            if (equipment != null)
            {
                slots[^1].Tag = -1;
                slots[^1].Click += ((Object o, EventArgs e) => { OpenContextMenu(slots[^1]); });
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
                c.DrawRect(left, top * 9, (float)(right * percentage), bottom, p);
            }
            slots[^1].Background = new BitmapDrawable(Resources, icon);
            icon.Dispose();
            inventory.Show();
        }

        public void Debug_Click(object sender, EventArgs e)
        {
            LinearLayout.LayoutParams lines = (LinearLayout.LayoutParams)inventory.FindViewById(Resource.Id.slotsLayout1).LayoutParameters;
            LinearLayout.LayoutParams buttons = (LinearLayout.LayoutParams)inventory.FindViewById(Resource.Id.slot2).LayoutParameters;
            LinearLayout.LayoutParams firstButton = (LinearLayout.LayoutParams)inventory.FindViewById(Resource.Id.slot1).LayoutParameters;

            Dialog d = new Dialog(this);
            ScrollView main = new ScrollView(this, Orientation.Vertical);
            main.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            d.SetContentView(main);
            LinearLayout l1 = null;
            for (int i = 0, y = 0; i < (int)IDs.AMOUNT; i++)
            {
                if (IsItem(i))
                {
                    Button Item = new Button(this);
                    if (y % 4 == 0)
                    {
                        l1 = new LinearLayout(this);
                        l1.LayoutParameters = lines;
                        main.AddView(l1);
                        Item.LayoutParameters = firstButton;
                    }
                    else
                    {
                        Item.LayoutParameters = buttons;
                    }
                    Item.Tag = i;
                    Item.Background = new BitmapDrawable(Resources, GetIcon((int)Item.Tag, this));
                    Item.Click += GiveItem_Click;
                    l1.AddView(Item);
                    y++;
                }
            }
            d.Show();
            d.CancelEvent += Debug_CancelEvent;
        }

        private void Debug_CancelEvent(object sender, EventArgs e)
        {
            displayInventory.CallOnClick();
        }

        private bool IsItem(int id)
        {
            bool IsItem = false;
            switch (id)
            {
                case (int)IDs.STONE:
                case (int)IDs.STICK:
                case (int)IDs.PICKAXE:
                case (int)IDs.SHOVEL:
                case (int)IDs.TREE_SEED:
                case (int)IDs.AXE:
                case (int)IDs.WOOD_LOG:
                    IsItem = true;
                    break;
            }
            return IsItem;
        }

        private void GiveItem_Click(object sender, EventArgs e)
        {
            int id = (int)((View)sender).Tag;
            switch (id)
            {
                case (int)IDs.STONE:
                    Map.Player.GiveItem(ItemType.Create((int)ItemType.ItemTypes.STONE));
                    break;
                case (int)IDs.STICK:
                    Map.Player.GiveItem(ItemType.Create((int)ItemType.ItemTypes.STICK));
                    break;
                case (int)IDs.PICKAXE:
                    Map.Player.GiveItem(ItemType.Create((int)ItemType.ItemTypes.PICKAXE));
                    break;
                case (int)IDs.SHOVEL:
                    Map.Player.GiveItem(ItemType.Create((int)ItemType.ItemTypes.SHOVEL));
                    break;
                case (int)IDs.TREE_SEED:
                    Map.Player.GiveItem(ItemType.Create((int)ItemType.ItemTypes.TREE_SEED));
                    break;
                case (int)IDs.AXE:
                    Map.Player.GiveItem((Tool)ItemType.Create((int)ItemType.ItemTypes.AXE));
                    break;
                case (int)IDs.WOOD_LOG:
                    Map.Player.GiveItem(ItemType.Create((int)ItemType.ItemTypes.LOG));
                    break;
            }
        }

        public static Bitmap GetIcon(int id, Context C)
        {
            int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
            Bitmap icon = BitmapFactory.DecodeResource(C.Resources, C.Resources.GetIdentifier("inventory_slot_border", "drawable", C.PackageName)).Copy(Bitmap.Config.Argb8888, true);
            if (id < 0)
            {
                return icon;
            }
            Canvas c = new Canvas(icon);
            Rect src = new Rect((id % MapDraw.spriteSheetColoumnCount) * iconSize, (id / MapDraw.spriteSheetColoumnCount) * iconSize, (id % MapDraw.spriteSheetColoumnCount + 1) * iconSize, (id / MapDraw.spriteSheetColoumnCount + 1) * iconSize);
            int left = c.Width / 11;
            int right = c.Width / 11 * 10;
            int top = c.Height / 11;
            int bottom = c.Height / 11 * 10;
            Rect dst = new Rect(left, top, right, bottom);
            Bitmap item = Bitmap.CreateBitmap(spriteSheet, src.Left, src.Top, src.Right - src.Left, src.Bottom - src.Top);
            Bitmap scaled = Bitmap.CreateScaledBitmap(item, right - left, bottom - top, false);
            c.DrawBitmap(scaled, null, dst, null);
            return icon;
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

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            TextView view = (TextView)v;
            menu.Add(0,(int)v.Tag, 0, "Drop");
            int type = (int)v.Tag;
            if (type > 0)
            {
                menu.Add(0, int.Parse(view.Text), 0, "Equip");
            }
            else if (type < 0)
            {
                menu.Add(0, int.Parse(view.Text), 0, "De-Equip");
            }
            for (int i = 0; i < menu.Size(); i++)
            {
                menu.GetItem(i).SetOnMenuItemClickListener(this);
            }
        }
        public override bool OnContextItemSelected(IMenuItem item)
        {
            string title = item.TitleFormatted.ToString();
            if (title.Equals("Drop"))
            {
                Item drop = Map.Player.DropItem(item.ItemId);
                if (drop != null)
                {
                    Map.Generator.SetItemPointer((int)Map.Player.GetX(), (int)Map.Player.GetY(), drop);
                }
                Map.Invalidate();
            }
            else if (title.Equals("Equip"))
            {
                Map.Player.Equip(item.ItemId);
            }
            else if (title.Equals("De-Equip"))
            {
                Map.Player.DeEquip();
            }
            Invalidate();

            return true;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            return OnContextItemSelected(item);
        }
    }
}