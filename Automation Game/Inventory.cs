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
        Item equipped;
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

        public Item RemoveItem(int index)
        {
            Item item = items[index];
            items[index] = null;
            if (item != null)
            {
                itemsStored--;
            }
            return item;
        }

        public bool IsFull()
        {
            return itemsStored == size;
        }

        public Item GetItem(int n)
        {
            return items[n];
        }

        public bool equip(int n)
        {
            if (items[n] != null && items[n].isEquipable)
            {
                if (equipped == null)
                {
                    equipped = items[n];
                    items[n] = null;
                }
                else
                {
                    Item removed = equipped;
                    equipped = items[n];
                    items[n] = equipped;
                }
                return true;
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
    }
}