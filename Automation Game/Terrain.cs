using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Path = System.IO.Path;

namespace Automation_Game
{
    public class Terrain
    {
        public string type { get; }
        public static int size { get; set; } = 70;
        public int id { get; }
        public Terrain(string type, int id)
        {
            this.type = type;
            this.id = id;
        }

    }
}