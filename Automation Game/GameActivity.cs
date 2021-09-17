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

        Dialog inventory;

        Bitmap spriteSheet;

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
                            Map.MapGenerator gen = new Map.MapGenerator(buffer);
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
                            map = new MapDraw(this, gen, GenerateGroundItems(buffer, gen.GetWidth(), gen.GetHeight()), p);
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
            LinearLayout slotsLayout1 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout1);
            LinearLayout slotsLayout2 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout2);
            LinearLayout main = (LinearLayout)inventory.FindViewById(Resource.Id.main);
            slotsLayout1.Invalidate();
            slotsLayout2.Invalidate();
            main.Invalidate();

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
                c.DrawColor(Color.CadetBlue);
                Rect src = new Rect((9 / 6) * spriteSheet.Width, (9 % 6) * spriteSheet.Height, (9 / 6 + 1) * spriteSheet.Width, (9 % 6 + 1) * spriteSheet.Height);
                Rect dst = new Rect(0, 0, c.Width, c.Height);
                c.DrawBitmap(spriteSheet, src, dst, null);
                craftingStation.Background = new BitmapDrawable(Resources, bs);
                craftingUI.AddView(close);
                craftingUI.AddView(craftingStation);
                frame.AddView(craftingUI);
            }
            craftingUI.Visibility = ViewStates.Visible;
            DisplayCraftingUI.Visibility = ViewStates.Invisible;
            displayInventory.Visibility = ViewStates.Invisible;
            map.editMode = true;

        }

        private void Close_Click(object sender, EventArgs e)
        {
            string str = e.ToString();
            craftingUI.Visibility = ViewStates.Invisible;
            DisplayCraftingUI.Visibility = ViewStates.Visible;
            displayInventory.Visibility = ViewStates.Visible;
            map.editMode = false;
        }

        private void DisplayMap_Click(object sender, EventArgs e)
        {
            Dialog minimap = new Dialog(this);
            View background = new View(this);
            background.Background = new BitmapDrawable(Resources, map.DisplayMap((int)(Window.DecorView.Width * 0.8), (int)(Window.DecorView.Height * 0.8)));
            minimap.SetContentView(background);
            minimap.SetCancelable(true);
            minimap.SetTitle("");
            minimap.Show();
            minimap.Window.SetLayout((int)(Window.DecorView.Width * 0.8), (int)(Window.DecorView.Height * 0.8));
            Console.WriteLine("width:" + minimap.Window.Attributes.Width + " height:" + minimap.Window.Attributes.Height);
        }

        private void DisplayInventory_Click(object sender, EventArgs e)
        {
            inventory = new Dialog(this);
            inventory.SetContentView(Resource.Layout.inventory_layout);
            inventory.SetCancelable(true);
            inventory.SetTitle("");
            inventory.Window.SetLayout((int)(Window.DecorView.Width * 0.8), (int)(Window.DecorView.Height * 0.8));
            Item[] inv = map.player.GetInvetory();
            Button[] itemSlots = new Button[inv.Length];
            LinearLayout slotsLayout1 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout1);
            LinearLayout slotsLayout2 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout2);
            LinearLayout main = (LinearLayout)inventory.FindViewById(Resource.Id.main);
            int size = 0;
            int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
            BitmapDrawable background;
            Bitmap icon;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (inv[i] != null)
                {
                    itemSlots[i] = new Button(this);
                    size = (int)(iconSize);
                    icon = Bitmap.CreateBitmap(spriteSheet, iconSize * (inv[i].id % MapDraw.spriteSheetColoumnCount), iconSize * (inv[i].id / MapDraw.spriteSheetColoumnCount), iconSize, iconSize);
                    background = new BitmapDrawable(Resources, icon);
                    itemSlots[i].SetWidth(background.Bitmap.Width);
                    itemSlots[i].SetHeight(background.Bitmap.Height);
                    itemSlots[i].Background = background;
                    if (slotsLayout1.ChildCount < 4)
                    {
                        slotsLayout1.AddView(itemSlots[i]);
                    }
                    else
                    {
                        slotsLayout2.AddView(itemSlots[i]);
                    }
                    itemSlots[i].SetOnTouchListener(new OnTouchEvent(this));
                    itemSlots[i].Tag = i;
                }
            }
            Item equipment = map.player.GetEquippedItem();
            if (equipment != null)
            {
                Button btn = new Button(this);
                size = (int)(iconSize);
                icon = Bitmap.CreateBitmap(spriteSheet, iconSize * (equipment.id % MapDraw.spriteSheetColoumnCount), iconSize * (equipment.id / MapDraw.spriteSheetColoumnCount), iconSize, iconSize);
                background = new BitmapDrawable(Resources, icon);
                btn.SetWidth(background.Bitmap.Width);
                btn.SetHeight(background.Bitmap.Height);
                btn.Background = background;
                btn.SetPadding((int)(Window.DecorView.Width * 0.8) - btn.Width, (int)(Window.DecorView.Height * 0.8) - btn.Height * 3, 0, 0);
            }
            inventory.Show();
        }

        private void Equipped_Click(object sender, EventArgs e)
        {
            if (map.player.DeEquip())
            {
                LinearLayout slotsLayout1 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout1);
                LinearLayout slotsLayout2 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout2);
                LinearLayout main = (LinearLayout)inventory.FindViewById(Resource.Id.main);
                main.RemoveView((View)sender);
                if (slotsLayout1.ChildCount < 4)
                {
                    slotsLayout1.AddView((Button)sender);
                }
                else
                {
                    slotsLayout2.AddView((Button)sender);
                }
                Invalidate();
            }
        }

        public void add_equip()
        {
            inventory.Hide();
            displayInventory.CallOnClick();
        }

        public void Drop(Button clicked)
        {
            int tag = (int)clicked.Tag;
            LinearLayout ll1 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout1);
            LinearLayout ll2 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout2);
            if (map.dropItem(tag))
            {
                ((LinearLayout)clicked.Parent).RemoveView(clicked);
                if (ll2.ChildCount > 0)
                {
                    Button btn = (Button)ll2.GetChildAt(0);
                    ll2.RemoveViewAt(0);
                    ll1.AddView(btn);
                }
            }
            for (int i = 0; i < ll1.ChildCount; i++)
            {
                Button btn = (Button)ll1.GetChildAt(i);
                btn.Tag = i;
            }
            for (int i = 0; i < ll2.ChildCount; i++)
            {
                Button btn = (Button)ll2.GetChildAt(i);

                btn.Tag = i + 4;
            }
        }

        private Item[,] GenerateGroundItems(Byte[] bytes, int width, int height)
        {
            Item[,] items = new Item[width, height];
            List<Byte> byteList = bytes.ToList();
            int offset = 0;
            while (offset < bytes.Length - 1)
            {
                int x = BitConverter.ToInt32(bytes, offset);
                offset += 4;
                int y = BitConverter.ToInt32(bytes, offset);
                offset += 4;
                while (offset < bytes.Length - 1 && BitConverter.ToInt32(bytes, offset) != 0)
                {
                    int length = BitConverter.ToInt32(bytes, offset);
                    offset += 4;
                    items[x, y] = new Item(byteList.GetRange(offset, length).ToArray());
                    offset += length;
                    x++;
                    y++;
                }
                if (BitConverter.ToInt32(bytes, offset) == 0)
                {
                    offset += 4;
                }
            }
            return items;
        }
    }
}