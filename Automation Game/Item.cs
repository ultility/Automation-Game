using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation_Game
{
    public class Item
    {
        public string name { get; }
        public int id { get; }

        public double sizePercentage { get; }

        public bool isEquipable { get; }

        public Item(string name, int id, double sizePercentage, bool isEquipable = false)
        {
            this.name = name;
            this.id = id;
            this.sizePercentage = sizePercentage;
            this.isEquipable = isEquipable;
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
            isEquipable = BitConverter.ToBoolean(bytes, offset);
        }

        public Byte[] ToByte()
        {
            List<Byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(Encoding.ASCII.GetBytes(name).Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(name));
            bytes.AddRange(BitConverter.GetBytes(id));
            bytes.AddRange(BitConverter.GetBytes(sizePercentage));
            bytes.AddRange(BitConverter.GetBytes(isEquipable));
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
    }
}