using System;
using System.Collections.Generic;

namespace Automation_Game
{
    public class Tool : Item
    {
        public int durability { get; set; }
        public Tool(string name, int id, int durability) : base(name, id, 1)
        {
            this.durability = durability;
        }

        public Tool(Tool other) : base(other.name, other.id, 1)
        {
            this.durability = other.durability;
        }

        public Tool(Byte[] bytes) : base(bytes)
        {
            durability = BitConverter.ToInt32(bytes, bytes.Length - 5);
        }

        public static bool IsTool(Byte[] bytes)
        {
            int offset = 0;
            int length = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(Int32) + length;
            offset += sizeof(Int32);
            offset += sizeof(double);

            if (bytes.Length - offset == sizeof(int))
            {
                return true;
            }
            return false;
        }

        public bool Use(int Hardness)
        {
            durability -= Hardness;
            return durability <= 0;
        }

        public override Byte[] ToByte()
        {
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(base.ToByte());
            bytes.AddRange(BitConverter.GetBytes(durability));
            return bytes.ToArray();
        }
    }
}