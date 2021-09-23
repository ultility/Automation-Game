using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;

namespace Automation_Game
{
    [Activity(Label = "GameActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class GameActivity : Activity
    {
        FrameLayout frame;
        LinearLayout craftingUI;

        public MapDraw map { get; set; }

        Button displayMap;
        Button displayInventory;
        Button DisplayCraftingUI;
        Button[] slots;

        Dialog inventory;

        Bitmap spriteSheet;
        Bitmap minimapBitmap;

        OnTouchEvent press;
        bool IsMinimapShown;

        public StructureBlueprint UsedBlueprint;

        protected override void OnStop()
        {
            map.save();
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
            slots = new Button[9];
            IsMinimapShown = false;
            if (Intent.GetBooleanExtra("create", true))
            {
                map = new MapDraw(this);
            }
            else
            {
                try
                {
                    int offset = 0;
                    using (Stream stream = OpenFileInput("world.txt"))
                    {
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
                            Player p = new Player(buffer, gen.GetWidth() / 2, gen.GetHeight() / 2);
                            offset += buffer.Length;
                            stream.Read(readSize, 0, 4);
                            offset += 4;
                            buffer = new Byte[BitConverter.ToInt32(readSize)];
                            stream.Read(buffer, 0, buffer.Length);
                            map = new MapDraw(this, gen, p);
                        }
                        catch (Java.IO.IOException e)
                        {
                            e.PrintStackTrace();
                            map = new MapDraw(this);
                        }
                    }
                }
                catch (Java.IO.FileNotFoundException e)
                {
                    e.PrintStackTrace();
                    map = new MapDraw(this);
                }
            }
            frame.AddView(map);
            displayInventory.Click += DisplayInventory_Click;
            displayMap.Click += DisplayMap_Click;
            DisplayCraftingUI.Click += DisplayCraftingUI_Click;

            spriteSheet = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("sprite_sheet", "drawable", this.PackageName));

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
                Rect src = new Rect((9 % 6) * spriteSheet.Width / 6, (9 / 6) * spriteSheet.Height / 2, (9 % 6 + 1) * spriteSheet.Width / 6, (9 / 6 + 1) * spriteSheet.Height / 2);
                Rect dst = new Rect(0, 0, c.Width, c.Height);
                Paint p = new Paint();
                Color color = Color.CadetBlue;
                c.DrawColor(color);
                PorterDuffColorFilter cf = new PorterDuffColorFilter(color, PorterDuff.Mode.Multiply);
                p.SetColorFilter(cf);
                c.DrawBitmap(spriteSheet, src, dst, p);
                craftingStation.Background = new BitmapDrawable(Resources, bs);
                craftingStation.Click += CraftingStation_Click;
                craftingUI.AddView(close);
                craftingUI.AddView(craftingStation);
                frame.AddView(craftingUI);
            }
            craftingUI.Visibility = ViewStates.Visible;
            DisplayCraftingUI.Visibility = ViewStates.Invisible;
            displayInventory.Visibility = ViewStates.Invisible;
            map.editMode = true;

        }

        private void CraftingStation_Click(object sender, EventArgs e)
        {
            Structure s = new CraftingStation(this);
            Delivery[] d = new Delivery[1];
            d[0] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].name, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].id, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].sizePercentage), 1);
            map.CurrentlyBuilding = new StructureBlueprint(s, d, null);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            string str = e.ToString();
            craftingUI.Visibility = ViewStates.Invisible;
            DisplayCraftingUI.Visibility = ViewStates.Visible;
            displayInventory.Visibility = ViewStates.Visible;
            map.editMode = false;
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
                    minimapBitmap = await Task.Run(() => map.DisplayMap(size, size));
                }
                Bitmap mm = minimapBitmap.Copy(minimapBitmap.GetConfig(), true);
                Canvas c = new Canvas(mm);
                int spriteSheetSignleWidth = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                Player player = map.player;
                int centerX = (int)(player.GetX() / 100 * c.Width);
                int centerY = (int)(player.GetY() / 100 * c.Height);
                int width = 3 * c.Width / map.generator.GetWidth();
                int height = 3 * c.Height / map.generator.GetHeight();
                int length = Math.Min(width, height);
                if (Window.DecorView.Width > Window.DecorView.Height)
                {
                    width = length / (Window.DecorView.Width / Window.DecorView.Height);
                }
                else
                {
                    height = length / (Window.DecorView.Height / Window.DecorView.Width);
                }
                Paint p = new Paint();
                p.Color = Color.Red;
                c.DrawOval(centerX - width / 2, centerY - height / 2, centerX + width / 2, centerY + height / 2, p);
                background.Background = new BitmapDrawable(Resources, mm);
                minimap.SetContentView(background);
                minimap.SetCancelable(true);
                minimap.CancelEvent += Minimap_CancelEvent;
                minimap.SetTitle("");
                minimap.Show();
                minimap.Window.SetLayout((int)(Window.DecorView.Width * 0.8), (int)(Window.DecorView.Height * 0.8));
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
            UsedBlueprint = null;
            Item[] inv = map.player.GetInvetory();
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
                }
                slots[i].Background = new BitmapDrawable(Resources, icon);

            }
            Item equipment = map.player.GetEquippedItem();

            icon = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("inventory_slot_border", "drawable", PackageName)).Copy(Bitmap.Config.Argb8888, true);
            if (equipment != null)
            {
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
            }
            slots[8].Background = new BitmapDrawable(Resources, icon);
            inventory.Show();
        }

        private void DeEquip(object sender, EventArgs e)
        {
            map.player.DeEquip();
            Invalidate();
        }
    }
}