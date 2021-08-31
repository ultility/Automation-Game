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
    class ItemType
    {
        public string name { get; }
        public double spawnChance { get; }
        public int id { get; }
        public double sizePercentage { get; }
        public string[] spawnableTerrain { get; }

        public ItemType(string name, double spawnChance, int id, double size, string[] spawnableTerrain)
        {
            this.name = name;
            this.spawnChance = spawnChance;
            this.id = id;
            this.sizePercentage = size;
            this.spawnableTerrain = spawnableTerrain;
        }
    }
}