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
    public class Structure
    {
        public string name { get; }
        public int id { get; }

        public double sizePercentage { get; }

        Item useableTool;

        public Structure(string name, int id, double sizePercentage, Item useableTool)
        {
            this.name = name;
            this.id = id;
            this.sizePercentage = sizePercentage;
            this.useableTool = useableTool;
        }

        public bool destory(Player p)
        {
            
            return false;
        }
    }
}