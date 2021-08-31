using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.IO;
using System;


namespace Automation_Game
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme",/* MainLauncher = true,*/ ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class StartUpMenu : AppCompatActivity
    {
        Button start;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.startup_menu);
            start = (Button)FindViewById(Resource.Id.start);
            start.Click += Start_Click;
        }

        private void Start_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(GameActivity)));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}