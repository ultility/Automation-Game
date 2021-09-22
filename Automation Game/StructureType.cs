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
    public class StructureType
    {
        public string name { get; }
        public int id { get; }
        public int sizePercentage { get; }
        public Item dropItem { get; }
        public Item useableItem { get; }
        public double spawnChance { get; }
        public string[] spawnableTerrain { get; }
        public int hardness { get; }

        public StructureType(string name, int id, int size, Item DropItem, Item UseableItem, double spawnChance, string[] SpawnableTerrain, int hardness)
        {
            this.name = name;
            this.id = id;
            this.sizePercentage = size;
            dropItem = DropItem;
            useableItem = UseableItem;
            this.spawnChance = spawnChance;
            this.spawnableTerrain = SpawnableTerrain;
            this.hardness = hardness;
        }
    }
}