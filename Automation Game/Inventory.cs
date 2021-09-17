using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation_Game
{
    class Inventory
    {
        Item[] items;
        Tool equipped;
        int size;
        int itemsStored;

        public Inventory(int inventorySize)
        {
            size = inventorySize;
            items = new Item[size];
            equipped = null;
            itemsStored = 0;
        }

        public bool AddItem(Item item)
        {
            if (itemsStored != size)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null)
                    {
                        items[i] = item;
                        itemsStored++;
                        return true;
                    }
                }
            }
            return false;
        }

        public void sort()
        {
            for (int k = 0; k < items.Length; k++)
            {
                if (items[k] == null)
                {
                    for (int i = k; i < items.Length - 1; i++)
                    {
                        items[i] = items[i + 1];
                    }
                    items[items.Length - 1] = null;
                }
            }
        }

        public Item RemoveItem(int index, bool sort)
        {
            Item item = items[index];
            items[index] = null;
            if (item != null)
            {
                itemsStored--;
                if (sort)
                {
                    this.sort();
                }
            }
            return item;
        }

        public bool IsFull()
        {
            return itemsStored == size;
        }

        public Item GetItem(int n)
        {
            if (n < 0)
            {
                return equipped;
            }
            return items[n];
        }

        public bool equip(int n)
        {
            if (items[n] != null)
            {
                Item item = items[n];
                if (item is Tool tool)
                {
                    if (equipped == null)
                    {
                        equipped = tool;
                        items[n] = null;
                    }
                    else
                    {
                        Item removed = equipped;
                        equipped = tool;
                        items[n] = equipped;
                    }

                    return true;
                }
            }
            return false;
        }
        public bool deequip()
        {
            if (AddItem(equipped))
            {
                equipped = null;
                return true;
            }
            return false;
        }

        public Inventory(Byte[] bytes)
        {
            
            int offset = 0;
            int i = 0;
            size = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(int);
            items = new Item[size];
            while (offset < bytes.Length - 1 && BitConverter.ToInt32(bytes, offset) != 0)
            {
                int length = BitConverter.ToInt32(bytes, offset);
                offset += sizeof(int);
                Byte[] item = new Byte[length];
                Array.Copy(bytes, offset, item, 0, length);
                items[i] = new Item(item);
                offset += length;
                i++;
            }
            if (offset < bytes.Length - 1)
            {
                offset += sizeof(int);
                Byte[] item = new Byte[bytes.Length - offset];
                Array.Copy(bytes, offset, item, 0, item.Length);
                equipped = new Tool(item);
            }
        }
        public Byte[] ToByte()
        {
            List<Byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(size));
            for (int i = 0; i < itemsStored; i++)
            {
                Byte[] item = items[i].ToByte();
                bytes.AddRange(BitConverter.GetBytes(item.Length));
                bytes.AddRange(item);
            }
            if (equipped != null)
            {
                bytes.AddRange(BitConverter.GetBytes(0));
                bytes.AddRange(equipped.ToByte());
            }
            return bytes.ToArray();
        }
    }
}