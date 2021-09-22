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
        protected float x;
        protected float y;
        protected MapDraw parent;
        protected Thread movementThread;
        protected MovementPacket packet;
        public abstract void MoveTo(int targetX, int targetY);

        public float GetX()
        {
            return x;
        }

        public float GetY()
        {
            return y;
        }

        public void Move(float dx, float dy)
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