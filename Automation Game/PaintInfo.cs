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
using Android.Graphics;

namespace Automation_Game
{
    class PaintInfo
    {
        public Canvas canvas;
        public Bitmap bitmap;
        public int posX;
        public int posY;

        public PaintInfo(Canvas canvas, Bitmap bitmap, int posX, int posY)
        {
            this.canvas = canvas;
            this.bitmap = bitmap;
            this.posX = posX;
            this.posY = posY;
        }
    }
}