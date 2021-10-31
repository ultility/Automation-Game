using System;
using System.Collections.Generic;
using System.Text;


namespace Automation_Game
{
    public class Structure : IRotateable
    {
        public string Name { get; }
        public int Id { get; }

        public float SizePercentage { get; }

        protected readonly List<Item> dropItems;

        readonly Item useableTool;

        public int Hardness { get; }

        public bool Walkable { get; protected set; }
        private int rotation { get; set; }
        public int Rotation 
        {
            get { return rotation; }
            set
            {
                rotation = value;
                if (rotation < 0)
                {
                    rotation = -rotation;
                    rotation %= 360;
                    rotation = 359 - rotation;
                }
                else if (rotation > 360)
                {
                    rotation %= 360;
                }
            }
        }

        public virtual bool IsBuildable(Terrain t)
        {
            return !t.Type.Equals("water");
        }

        public Structure(string name, int id, float size, Item useableTool, IEnumerable<Item> droppedItems, int Hardness, bool Walkable = false, int Rotation = 0)
        {
            this.dropItems = new List<Item>();
            this.dropItems.AddRange(droppedItems);
            this.Name = name;
            this.Id = id;
            SizePercentage = size;
            this.useableTool = useableTool;
            this.Hardness = Hardness;
            this.Walkable = Walkable;
            this.Rotation = Rotation;
        }

        public Structure(string name, int id, float size, Item useableTool, Item droppedItem, int Hardness, bool Walkable = false, int Rotation = 0)
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
            this.Rotation = Rotation;
        }

        public Structure(StructureType type, int Rotation = 0)
        {
            Name = type.Name;
            Id = type.Id;
            SizePercentage = type.SizePercentage;
            useableTool = type.UseableItem;
            Hardness = type.Hardness;
            this.Walkable = type.Walkable;
            this.dropItems = new List<Item>();
            dropItems.AddRange(type.DropItem);
            this.Rotation = Rotation;
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

        public static bool IsStructure(int id)
        {
            switch (id)
            {
                case (int)GameActivity.IDs.TREE:
                case (int)GameActivity.IDs.ROCK:
                case (int)GameActivity.IDs.DIRT_HOLE:
                case (int)GameActivity.IDs.CRAFTING_STATION:
                case (int)GameActivity.IDs.STORAGE_BOX:
                case (int)GameActivity.IDs.SAWING_TABLE:
                case (int)GameActivity.IDs.WOOD_FLOOR:
                case (int)GameActivity.IDs.WOOD_WALL:
                    return true;
                default:
                    return false;
            }
        }
    }
}