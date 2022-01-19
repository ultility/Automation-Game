using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation_Game
{
    public class OrientationListener : OrientationEventListener
    {
        public event EventHandler OrientationChanged;
        public OrientationListener(Context c) : base(c)
        {

        }

        public override void OnOrientationChanged(int orientation)
        {
                EventHandler handler = OrientationChanged;
                OrientationEventArgs args;
                if (orientation  >= 315 && orientation <= 135)
                {
                    args = new OrientationEventArgs(Android.Content.PM.ScreenOrientation.Landscape);
                }
                else
                {
                    args = new OrientationEventArgs(Android.Content.PM.ScreenOrientation.ReverseLandscape);
                }
                handler.Invoke(this, args);
            
        }

    }

    class OrientationEventArgs : EventArgs
    {
        public Android.Content.PM.ScreenOrientation Orientation { get; private set; }

        public OrientationEventArgs(Android.Content.PM.ScreenOrientation orientation)
        {
            this.Orientation = orientation;
        }
    }
}