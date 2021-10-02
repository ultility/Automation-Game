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
    public class StorageChest : Structure
    {
        Inventory inv;
        Context context;
        public StorageChest(Context c) : base("Chest", (int)GameActivity.IDs.STORAGE_BOX, 1, new Item("Axe", (int)GameActivity.IDs.AXE, 1), (Item)null, 1)
        {
            inv = new Inventory(16);
            context = c;
        }

        public bool AddItem(Item item)
        {
            return inv.AddItem(item);
        }

        public Item RemoveItem(int n)
        {
            return inv.RemoveItem(n, true);
        }

        public Item[] GetInventory()
        {
            Item[] inventory = new Item[inv.size];
            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = inv.GetItem(i);
            }
            return inventory;
        }
    }
}