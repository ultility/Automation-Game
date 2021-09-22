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
    public class Delivery
    {
        public Item item { get; }
        public int amount { get; set; }

        public Delivery(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }

        public Delivery(Byte[] bytes)
        {
            int offset = 0;
            int length = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            item = new Item(bytes.ToList().GetRange(offset, length).ToArray());
            offset += length;
            amount = BitConverter.ToInt32(bytes, offset);
        }

        public Byte[] ToByte()
        {
            List<Byte> bytes = new List<byte>();
            Byte[] item = this.item.ToByte();
            bytes.AddRange(BitConverter.GetBytes(item.Length));
            bytes.AddRange(item);
            bytes.AddRange(BitConverter.GetBytes(amount));
            return bytes.ToArray();
        }
    }
}