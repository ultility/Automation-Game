﻿using System;
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
    [Activity(Label = "GameActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, MainLauncher = true)]
    public class GameActivity : Activity
    {
        FrameLayout frame;
        LinearLayout craftingUI;

        MapDraw map;

        Button displayMap;
        Button displayInventory;
        Button DisplayCraftingUI;

        Dialog inventory;

        Bitmap spriteSheet;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.game_map);
            frame = (FrameLayout)FindViewById(Resource.Id.frame);
            displayInventory = (Button)FindViewById(Resource.Id.openInventory);
            displayMap = (Button)FindViewById(Resource.Id.openMap);
            DisplayCraftingUI = (Button)FindViewById(Resource.Id.craftingMenu);

            map = new MapDraw(this);
            frame.AddView(map);

            displayInventory.Click += DisplayInventory_Click;
            displayMap.Click += DisplayMap_Click;
            DisplayCraftingUI.Click += DisplayCraftingUI_Click;

            spriteSheet = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("sprite_sheet", "drawable", this.PackageName));

        }

        private void DisplayCraftingUI_Click(object sender, EventArgs e)
        {
            if (craftingUI == null) {
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
                Rect dst = new Rect(0,0, c.Width, c.Height);
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
            int size = 0;
            int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
            int windowWidth = inventory.Window.DecorView.Width;
            int windowHeight = inventory.Window.DecorView.Height; 
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (inv[i] != null)
                {
                    itemSlots[i] = new Button(this);
                    size = (int)(iconSize);
                    Bitmap icon = Bitmap.CreateBitmap(spriteSheet, iconSize * (inv[i].id % MapDraw.spriteSheetColoumnCount), iconSize * (inv[i].id / MapDraw.spriteSheetColoumnCount), iconSize, iconSize);
                    BitmapDrawable background = new BitmapDrawable(Resources, icon);
                    itemSlots[i].SetWidth(background.Bitmap.Width);
                    itemSlots[i].SetHeight(background.Bitmap.Height);
                    itemSlots[i].Background = background;
                    int leftoverWidth = inventory.Window.DecorView.Width - (itemSlots[i].Width * 4);
                    int leftoverHeight = inventory.Window.DecorView.Height - (itemSlots[i].Height * 2);
                    itemSlots[i].FocusableInTouchMode = true;
                    if (slotsLayout1.ChildCount < 4)
                    {
                        slotsLayout1.AddView(itemSlots[i]);
                    }
                    else
                    {
                        slotsLayout2.AddView(itemSlots[i]);
                    }
                    itemSlots[i].Click += GameActivity_Click;
                    itemSlots[i].Tag = i;
                }
            }
            inventory.Show();
        }

        private void GameActivity_Click(object sender, EventArgs e)
        {
            Button clicked = (Button)sender;
            if(map.dropItem((int)clicked.Tag))
            {
                clicked.SetBackgroundColor(Android.Graphics.Color.White);
            }
        }
    }
}