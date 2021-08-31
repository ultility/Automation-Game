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
    class MovementPacket
    {
        public Moveable moving { get;}
        public int targetX;
        public int targetY;

        public MovementPacket(Moveable moving, int targetX, int targetY)
        {
            this.moving = moving;
            this.targetX = targetX;
            this.targetY = targetY;
        }
    }
}