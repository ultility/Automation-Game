﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using System;
using Automation_Game.Map;
using System.Timers;
using Android.Graphics.Drawables;
using System.IO;

namespace Automation_Game
{
    [Activity(Label = "MainMenu", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, MainLauncher = true)]
    public class MainMenu : Activity
    {
        Button load;
        Button start;
        Bitmap FullBackground;
        Bitmap background;
        LinearLayout backdrop;
        Rect src;
        Rect dst;
        Timer t;
        double dx;
        double left;
        double right;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.startup_menu);
            load = (Button)FindViewById(Resource.Id.last);
            start = (Button)FindViewById(Resource.Id.start);
            load.Click += Load_Click;
            start.Click += Start_Click;
            int width = Window.WindowManager.CurrentWindowMetrics.Bounds.Width();
            int height = Window.WindowManager.CurrentWindowMetrics.Bounds.Height();
            background = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            backdrop = (LinearLayout)FindViewById(Resource.Id.backdrop);
            Canvas draw = new Canvas(background);
            FullBackground = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("mainmenu_background", "drawable", PackageName));
            src = new Rect(0, 0, draw.Width, draw.Height);
            dst = new Rect(0, 0, draw.Width, draw.Height);
            left = src.Left;
            right = src.Right;
            draw.DrawBitmap(FullBackground, src, dst, null);
            t = new Timer(1000 / 30.0);
            t.Elapsed += T_Elapsed;
            t.Enabled = true;
            dx = Math.Abs(FullBackground.Width - background.Width);
            dx = dx / (60 * 30);
           backdrop.Background = new BitmapDrawable(Resources, background);
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            t.Enabled = false;
            left += dx;
            right += dx;
            src.Left = (int)left;
            src.Right = (int)right;
            if (src.Right >= FullBackground.Width || src.Left <= 0)
            {
                dx = -dx;
            }
            Canvas draw = new Canvas(background);
            draw.DrawBitmap(FullBackground, src, dst, null);
            backdrop.Background = new BitmapDrawable(Resources, background);
            t.Enabled = true;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            this.t.Stop();
            Intent t = new Intent(this, typeof(GameActivity));
            t.PutExtra("create", true);
            StartActivity(t);
        }

        private void Load_Click(object sender, EventArgs e)
        {
            this.t.Stop();
            Intent t = new Intent(this, typeof(GameActivity));
            t.PutExtra("create", false);
            StartActivity(t);
        }
    }
}