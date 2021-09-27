using System;
using System.Collections.Generic;
using System.Text;


namespace Automation_Game
{
    public class Structure
    {
        public string Name { get; }
        public int Id { get; }

        public float SizePercentage { get; }

        readonly List<Item> dropItems;

        readonly Item useableTool;

        public int Hardness { get; }

        public virtual bool IsBuildable(Terrain t)
        {
            return !t.Type.Equals("water");
        }

        public Structure(string name, int id, float size, Item useableTool, IEnumerable<Item> droppedItems, int Hardness)
        {
            this.dropItems = new List<Item>();
            this.dropItems.AddRange(droppedItems);
            this.Name = name;
            this.Id = id;
            SizePercentage = size;
            this.useableTool = useableTool;
            this.Hardness = Hardness;
        }

        public Structure(string name, int id, float size, Item useableTool, Item droppedItem, int Hardness)
        {
            this.dropItems = new List<Item>
            {
                droppedItem
            };
            this.Name = name;
            this.Id = id;
            SizePercentage = size;
            this.useableTool = useableTool;
            this.Hardness = Hardness;
        }

        public virtual bool Destory(Player p)
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

        public Item GetDropItem(int n)
        {
            if (n < dropItems.Count && n > 0)
            {
                return dropItems[n];
            }
            return null;
        }

        public Item[] GetDropItems()
        {
            return dropItems.ToArray();
        }

        public Byte[] ToByte()
        {
            List<Byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Name.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(Name));
            return bytes.ToArray();
        }
    }
}