using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Automation_Game
{
    [Activity(Label = "MainMenu", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, MainLauncher = true)]
    public class MainMenu : Activity
    {
        Button load;
        Button start;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.startup_menu);
            load = (Button)FindViewById(Resource.Id.last);
            start = (Button)FindViewById(Resource.Id.@new);
            load.Click += Load_Click;
            start.Click += Start_Click;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            Intent t = new Intent(this, typeof(GameActivity));
            t.PutExtra("new", true);
            StartActivity(t);
        }

        private void Load_Click(object sender, EventArgs e)
        {
            Intent t = new Intent(this, typeof(GameActivity));
            t.PutExtra("new", false);
            StartActivity(t);
        }
    }
}