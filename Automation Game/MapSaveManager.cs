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
using Automation_Game.Map;

namespace Automation_Game
{
    public class MapSaveManager
    {
        static private int offset;
        private static MapGenerator RestoreGenerator(IEnumerable<Byte> bytes, GameActivity activity) 
        {
            int MapWidth = BitConverter.ToInt32(bytes.ToArray(), offset);
            offset += 4;
            int MapHeight = BitConverter.ToInt32(bytes.ToArray(), offset);
            offset += 4;
            int seed = BitConverter.ToInt32(bytes.ToArray(), offset);
            return new MapGenerator(MapWidth, MapHeight, seed, activity);
        }

        public  static List<Byte> SaveGenerator(MapGenerator gen)
        {
            List<Byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(gen.GetWidth()));
            bytes.AddRange(BitConverter.GetBytes(gen.GetHeight()));
            bytes.AddRange(BitConverter.GetBytes(gen.GetSeed()));
            return bytes;
        }

        private static Player RestorePlayer(IEnumerable<Byte> bytes)
        {
            int x = BitConverter.ToInt32(bytes.ToArray(), offset);
            offset += 4;
            int y = BitConverter.ToInt32(bytes.ToArray(), offset);
            Player p = new Player(x,y);
            offset += 4;
            while (bytes.Count() > offset)
            {
                int id = BitConverter.ToInt32(bytes.ToArray(), offset);
                offset += 4;
                Item i = RestoreItem(bytes, id);
                if (i is Tool)
                {
                    offset += 4;
                }
            }
            return p;
        }

        public static List<Byte> SavePlayer(Player p)
        {
            List<Byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(p.GetX()));
            bytes.AddRange(BitConverter.GetBytes(p.GetY()));
            Item[] inventory = p.GetInvetory();
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] != null)
                {
                    bytes.AddRange(SaveItem(inventory[i]));
                }
            }
            return bytes;
        }

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

        public static MapDraw Restore(IEnumerable<Byte> bytes, Context context)
        {
            offset = 0;
            MapDraw restoredMap = new MapDraw(context, RestoreGenerator(bytes, (GameActivity)context), RestorePlayer(bytes));
            while (offset < bytes.Count())
            {
                int id = BitConverter.ToInt32(bytes.ToArray(), offset);
                offset += 4;

                if (Item.IsItem(id) != -1)
                {
                    //restored = RestoreItem(bytes, offset, id);
                }
                else if (Structure.IsStructure(id) != -1)
                {
                    //restored = RestoreStructure(bytes, offset, id);
                }
            }
            restoredMap.Player.SetParent(restoredMap);
            return restoredMap;
        }

        public static List<Byte> Save(MapDraw d)
        {
            offset = 0;
            List<Byte> bytes = new List<byte>();
            bytes.AddRange(SaveGenerator(d.Generator));
            bytes.AddRange(SavePlayer(d.Player));
            return bytes;
        } 

        private static Item RestoreItem(IEnumerable<Byte> bytes, int id)
        {
            Item item = ItemType.Create(Item.IsItem(id));
            if (item is Tool t)
            {
                t.durability = BitConverter.ToInt32(bytes.ToArray(), offset);
            }
            return item;
        }
        private static Structure RestoreStructure(IEnumerable<Byte> bytes, int id)
        {
            Structure structure = StructureType.Create(Structure.IsStructure(id));
            return structure;
        }
    }
}