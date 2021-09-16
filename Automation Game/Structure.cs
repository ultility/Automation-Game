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

        public int sizePercentage { get; }

        public Item dropItem { get; }

        Item useableTool;

        public virtual bool isBuildable(Terrain t)
        {
            return !t.type.Equals("water");
        }

        public Structure(string name, int id, int size, Item useableTool, Item droppedItem)
        {
            this.name = name;
            this.id = id;
            this.sizePercentage = sizePercentage;
            this.useableTool = useableTool;
            dropItem = droppedItem;
        }

        public bool destory(Player p)
        {
            return p.GetEquippedItem().Equals(useableTool);
        }
    }
}