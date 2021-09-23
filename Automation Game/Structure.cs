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

        public float sizePercentage { get; }

        public Item dropItem { get; }

        Item useableTool;

        public int hardness { get; }

        public virtual bool isBuildable(Terrain t)
        {
            return !t.type.Equals("water");
        }

        public Structure(string name, int id, float size, Item useableTool, Item droppedItem, int hardness)
        {
            this.name = name;
            this.id = id;
            sizePercentage = size;
            this.useableTool = useableTool;
            dropItem = droppedItem;
            this.hardness = hardness;
        }

        public virtual bool destory(Player p)
        {
            if (p != null)
            {
                if (p.GetEquippedItem() != null)
                {
                    return p.GetEquippedItem().Equals(useableTool);
                }
            }
            return false;
        }

        public Byte[] ToByte()
        {
            List<Byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(name.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(name));
            return bytes.ToArray();
        }
    }
}