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
    [Activity(Label = "GameActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, MainLauncher = true)]
    public class GameActivity : Activity
    {
        FrameLayout frame;

        MapDraw map;

        Button displayMap;
        Button displayInventory;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.game_map);
            frame = (FrameLayout)FindViewById(Resource.Id.frame);
            displayInventory = (Button)FindViewById(Resource.Id.openInventory);
            displayMap = (Button)FindViewById(Resource.Id.openMap);

            map = new MapDraw(this);
            frame.AddView(map);

            displayInventory.Click += DisplayInventory_Click;
            displayMap.Click += DisplayMap_Click;
        }

        private void DisplayMap_Click(object sender, EventArgs e)
        {
        }

        private void DisplayInventory_Click(object sender, EventArgs e)
        {
            Dialog inventory = new Dialog(this);
            inventory.SetContentView(Resource.Layout.inventory_layout);
            inventory.SetCancelable(true);
            inventory.SetTitle("");
            Item[] inv = map.player.GetInvetory();
            Button[] itemSlots = new Button[inv.Length];
            LinearLayout slotsLayout1 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout1);
            LinearLayout slotsLayout2 = (LinearLayout)inventory.FindViewById(Resource.Id.slotsLayout2);
            int size = 0;
            Bitmap spritesheet = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("sprite_sheet", "drawable", this.PackageName));
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (inv[i] != null)
                {
                    itemSlots[i] = new Button(this);
                    size = (int)(40 * inv[i].sizePercentage);
                    itemSlots[i].SetWidth(size);
                    itemSlots[i].SetHeight(size);
                    itemSlots[i].SetPadding(20, 20, 0, 0);

                    Bitmap icon = Bitmap.CreateBitmap(spritesheet, 40 * (inv[i].id % MapDraw.spriteSheetColoumnCount), 40 * (inv[i].id / MapDraw.spriteSheetColoumnCount), 40, 40);
                    itemSlots[i].SetBackgroundDrawable(new BitmapDrawable(icon));
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
            inventory.Window.SetLayout(size * 8, size * 4);
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