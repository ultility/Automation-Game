using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation_Game
{
    public class Item
    {
        public string name { get; }
        public int id { get; }

        public double sizePercentage { get; }

        public Item(string name, int id, double sizePercentage)
        {
            this.name = name;
            this.id = id;
            this.sizePercentage = sizePercentage;
        }
    }
}