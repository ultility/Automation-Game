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
using System.Threading;

namespace Automation_Game
{
    public abstract class Moveable : IMoveable
    {
        protected int x;
        protected int y;
        protected MapDraw parent;
        protected Thread movementThread;
        public abstract void MoveTo(int targetX, int targetY);

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

        public void Move(int dx, int dy)
        {
            x += dx;
            y += dy;
        }

        public MapDraw GetParent()
        {
            return parent;
        }
    }
}