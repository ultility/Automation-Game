using Android.App;
using Android.Content;
using Android.Graphics;
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
    public class ProccessBar : View
    {
        Bitmap empty { get; set; }
        Bitmap filled { get; set; }
        int progress { get { return progress; } 
            set { 
                progress = value;
                if (progress > 100)
                {
                    progress = 100;
                }
                else if (progress < 0)
                {
                    progress = 0;
                }
                Invalidate();
            } }
        public ProccessBar(Context context) : base(context)
        {
            progress = 0;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            canvas.DrawBitmap(empty, 0, 0, null);
            canvas.DrawBitmap(filled, filled.Width * (100 - progress) / 100, filled.Height * (100 - progress) / 100, null);
        }
    }
}