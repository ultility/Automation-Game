using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
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
        int progress;
        public int Progress
        {
            get { return progress; }
            set
            {
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
            }
        }
        public ProccessBar(Context context, Bitmap empty, Bitmap filled) : base(context)
        {
            progress = 0;
            this.empty = empty;
            Background = new BitmapDrawable(context.Resources, filled);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            Rect src = new Rect(empty.Width * progress / 100, 0, empty.Width, empty.Height);
            Rect dst = new Rect(canvas.Width * progress / 100, 0, canvas.Width, canvas.Height);
            canvas.DrawBitmap(empty, src, dst, null);
        }
    }
}