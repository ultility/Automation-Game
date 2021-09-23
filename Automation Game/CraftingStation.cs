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
    class CraftingStation : Structure
    {
        CraftingRecipe[] recipes;
        Context c;
        Player p;
        public CraftingStation(Context context, string name = "CraftingStation", int id = 9, int size = 1, Item useableItem = null, Item droppedItem = null) : base(name,id,size,useableItem,droppedItem, 0)
        {
            recipes = new CraftingRecipe[3];
            Delivery[] axeRecipe = new Delivery[2];
            axeRecipe[0] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].name, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].id, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].sizePercentage), 1);
            axeRecipe[1] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].name, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].id, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].sizePercentage), 1);
            recipes[0] = new CraftingRecipe(new Tool("Axe", 10, 20),axeRecipe);
            recipes[1] = new CraftingRecipe(new Tool("Pickaxe", 11, 20), axeRecipe);
            recipes[2] = new CraftingRecipe(new Tool("Shovel", 12, 20), axeRecipe);
            c = context;
        }

        public void use(Player p)
        {
            Handler handle = new Handler(Looper.MainLooper);
            this.p = p;
            handle.Post(new Action(DisplayMenu));
        }

        private void DisplayMenu()
        {
            AlertDialog d;
            AlertDialog.Builder build = new AlertDialog.Builder(c);
            build.SetCancelable(true);
            LinearLayout l1 = new LinearLayout(c);
            l1.LayoutParameters = new LinearLayout.LayoutParams(((Activity)c).Window.DecorView.Width * 8 / 10, ((Activity)c).Window.DecorView.Height * 8 / 10);
            build.SetView(l1);
            for (int i = 0; i < recipes.Length; i++)
            {
                Button tool = new Button(c);
                Bitmap spriteSheet = BitmapFactory.DecodeResource(c.Resources, c.Resources.GetIdentifier("sprite_sheet", "drawable", c.PackageName));
                int iconSize = spriteSheet.Width / MapDraw.spriteSheetColoumnCount;
                Bitmap icon = Bitmap.CreateBitmap(spriteSheet, iconSize * (recipes[i].result.id % MapDraw.spriteSheetColoumnCount), iconSize * (recipes[i].result.id / MapDraw.spriteSheetColoumnCount), iconSize, iconSize);
                tool.Background = new BitmapDrawable(c.Resources, icon);
                tool.Click += Axe_Click;
                tool.Tag = i;
                l1.AddView(tool);
            }
            d = build.Create();
            d.Show();
            d.Window.SetLayout(((Activity)c).Window.DecorView.Width * 8 / 10, ((Activity)c).Window.DecorView.Height * 8 / 10);
        }

        private void Axe_Click(object sender, EventArgs e)
        {
            recipes[(int)((Button)sender).Tag].Craft(p);
        }
    }
}