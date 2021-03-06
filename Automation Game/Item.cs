using System;
using System.Collections.Generic;
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

        public Item(Item other)
        {
            name = other.name;
            id = other.id;
            sizePercentage = other.sizePercentage;
        }

        public Item(ItemType type)
        {
            name = type.name;
            id = type.id;
            sizePercentage = type.sizePercentage;
        }

        public Item(Byte[] bytes)
        {
            int offset = 0;
            int length = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(Int32);
            name = Encoding.Default.GetString(bytes, offset, length);
            offset += length;
            id = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(Int32);
            sizePercentage = BitConverter.ToDouble(bytes, offset);
            offset += sizeof(double);
        }

        public virtual Byte[] ToByte()
        {
            List<Byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(Encoding.ASCII.GetBytes(name).Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(name));
            bytes.AddRange(BitConverter.GetBytes(id));
            bytes.AddRange(BitConverter.GetBytes(sizePercentage));
            return bytes.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is Item item)
            {
                return name.Equals(item.name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name);
        }

        public static int IsItem(int id)
        {
            switch (id)
            {
                case (int)GameActivity.IDs.STONE:
                    return (int)ItemType.ItemTypes.STONE;
                case (int)GameActivity.IDs.STICK:
                    return (int)ItemType.ItemTypes.STICK;
                case (int)GameActivity.IDs.AXE:
                    return (int)ItemType.ItemTypes.AXE;
                case (int)GameActivity.IDs.PICKAXE:
                    return (int)ItemType.ItemTypes.PICKAXE;
                case (int)GameActivity.IDs.SHOVEL:
                    return (int)ItemType.ItemTypes.SHOVEL;
                case (int)GameActivity.IDs.TREE_SEED:
                    return (int)ItemType.ItemTypes.TREE_SEED;
                default:
                    return (int)ItemType.ItemTypes.FALSE;
            }
        }

        public static bool IsTool(int id)
        {
            switch (id)
            {
                case (int)GameActivity.IDs.AXE:
                case (int)GameActivity.IDs.PICKAXE:
                case (int)GameActivity.IDs.SHOVEL:
                    return true;
                default:
                    return false;
            }
        }
    }
}