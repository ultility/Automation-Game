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
    interface IRotateable
    {
        private int rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public int Rotation
        {
            get; set;
        }
    }
}