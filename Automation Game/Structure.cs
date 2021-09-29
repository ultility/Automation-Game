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

        protected readonly List<Item> dropItems;

        readonly Item useableTool;

        public int Hardness { get; }

        public bool Walkable { get; protected set; }

        public virtual bool IsBuildable(Terrain t)
        {
            return !t.Type.Equals("water");
        }

        public Structure(string name, int id, float size, Item useableTool, IEnumerable<Item> droppedItems, int Hardness, bool Walkable = false)
        {
            this.dropItems = new List<Item>();
            this.dropItems.AddRange(droppedItems);
            this.Name = name;
            this.Id = id;
            SizePercentage = size;
            this.useableTool = useableTool;
            this.Hardness = Hardness;
            this.Walkable = Walkable;
        }

        public Structure(string name, int id, float size, Item useableTool, Item droppedItem, int Hardness, bool Walkable = false)
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
            this.Walkable = Walkable;
        }

        public Structure(StructureType type, bool Walkable = false)
        {
            Name = type.Name;
            Id = type.Id;
            SizePercentage = type.SizePercentage;
            useableTool = type.UseableItem;
            Hardness = type.Hardness;
            this.Walkable = Walkable;
            this.dropItems = new List<Item>();
            dropItems.AddRange(type.DropItem);
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
            if (n < dropItems.Count && n >= 0)
            {
                return dropItems[n];
            }
            return null;
        }

        public virtual Item[] GetDropItems()
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