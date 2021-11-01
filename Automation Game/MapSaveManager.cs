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
    public class MapSaveManager
    {
        public static List<Byte> SaveStructure(Structure s)
        {
            List<Byte> bytes = new List<byte>();
            Byte[] temp = BitConverter.GetBytes(s.Id);
            bytes.AddRange(temp);
            temp = BitConverter.GetBytes(s.Rotation);
            bytes.AddRange(temp);
            if (s is SawingTable st)
            {
                temp = SaveItem(st.InputItem).ToArray();
                AddArray(bytes, temp);
                temp = SaveItem(st.OutputItem).ToArray();
                AddArray(bytes, temp);
            }
            else if (s is StorageChest sc)
            {
                Item[] inventory = sc.GetInventory();
                temp = BitConverter.GetBytes(inventory.Length);
                bytes.AddRange(temp);
                for (int i = 0; i < inventory.Length && inventory[i] != null; i++)
                {
                    temp = SaveItem(inventory[i]).ToArray();
                    AddArray(bytes, temp);
                }
            }
            else if (s is StructureBlueprint sb)
            {
                for (int i = 0; i < sb.Recipe.Count; i++)
                {
                    temp = SaveItem(sb.Recipe[i].item).ToArray();
                    AddArray(bytes, temp);
                    temp = BitConverter.GetBytes(sb.Recipe[i].amount);
                    AddArray(bytes, temp);
                }
            }
            bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count));
            return bytes;
        }

        public static List<Byte> SaveItem(Item i)
        {
            List<Byte> bytes = new List<byte>();
            Byte[] temp = BitConverter.GetBytes(i.id);
            AddArray(bytes, temp);
            if (i is Tool t)
            {
                temp = BitConverter.GetBytes(t.durability);
                AddArray(bytes, temp);
            }
            bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count));
            return bytes;
        }

        private static void AddArray(List<Byte> bytes, Byte[] arr)
        {
            bytes.AddRange(BitConverter.GetBytes(arr.Length));
            bytes.AddRange(arr);
        }

        public static Object Restore(IEnumerable<Byte> bytes)
        {
            Object restored = null;
            int offset = 0;
            int id = BitConverter.ToInt32(bytes.ToArray(), offset);
            offset += 4;

            if (Item.IsItem(id) != -1)
            {
                restored = RestoreItem(bytes, offset, id);
            }
            else if (Structure.IsStructure(id) != -1)
            {
                restored = RestoreStructure(bytes, offset, id);
            }
            return restored;
        }

        private static Item RestoreItem(IEnumerable<Byte> bytes, int offset, int id)
        {
            Item item = ItemType.Create(Item.IsItem(id));
            return item;
        }
        private static Structure RestoreStructure(IEnumerable<Byte> bytes, int offset, int id)
        {
            Structure structure = StructureType.Create(Structure.IsStructure(id));
            return structure;
        }
    }
}