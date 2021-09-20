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
    public class Tool : Item
    {
        int durability;
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

        public bool use(int hardness)
        {
            durability -= hardness;
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