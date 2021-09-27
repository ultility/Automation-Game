using Android.OS;
using System;
using System.Collections.Generic;

namespace Automation_Game
{
    public class Terrain
    {
        public string Type { get; }
        public static int Size { get; } = 70;
        public int Id { get; }

        readonly List<Item> items;
        readonly int x;
        readonly int y;

        Structure structure;

        GameActivity activity;
        public Terrain(string type, int id, int x, int y, GameActivity activity = null)
        {
            this.x = x;
            this.y = y;
            this.Type = type;
            this.Id = id;
            structure = null;
            this.activity = activity;
            items = new List<Item>();
        }

        public Item GetItem()
        {
            if (items.Count > 0)
            {
                return items[^1];
            }
            return null;
        }

        public Item Pickup()
        {
            Item i = items[^1];
            items.RemoveAt(items.Count - 1);
            return i;
        }
        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void SetActivity(GameActivity activity)
        {
            this.activity = activity;
        }

        public GameActivity GetActivity()
        {
            return activity;
        }

        public Structure GetStructure()
        {
            return structure;
        }

        public string GetStructureName()
        {
            if (structure == null)
            {
                return null;
            }
            return structure.Name;
        }

        public int GetStructureId()
        {
            return structure.Id;
        }

        public bool BuildStructure(Structure structure)
        {
            if (this.structure == null && structure != null && structure.IsBuildable(this) && items.Count == 0)
            {
                this.structure = structure;
                return true;
            }
            return false;
        }

        public bool UseStructure(MovementPacket packet)
        {
            if (Math.Abs(packet.moving.GetX() - x) <= 1 && Math.Abs(packet.moving.GetY() - y) <= 1)
            {
                if (structure is CraftingStation cs)
                {
                    if (packet.moving is Player p)
                    {
                        cs.Use(p);
                        return true;
                    }
                }
                else if (structure is StructureBlueprint blueprint)
                {
                    if (activity != null)
                    {
                        Handler handle = new Handler(Looper.MainLooper);
                        handle.Post(() =>
                        {
                            activity.UsedBlueprint = blueprint;
                            activity.DisplayInventory_Click(null, null);
                        });
                        return true;
                    }

                }
            }
            return false;
        }


        public bool DestroyStructure(Player p)
        {
            if (Math.Abs(p.GetX() - x) <= 1 && Math.Abs(p.GetY() - y) <= 1)
            {
                if (structure != null)
                {
                    if (structure.Destory(p))
                    {
                        if (structure.Name.Equals("Rock"))
                        {
                            if (!p.IsInventoryFull())
                            {
                                p.GiveItem(structure.GetDropItem(0));
                                return true;
                            }
                        }
                        else
                        {
                            items.AddRange(structure.GetDropItems());
                            structure = null;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Build(StructureBlueprint sender)
        {
            if (structure is StructureBlueprint sb)
            {
                if (sb == sender)
                {
                    structure = sb.Result;
                    activity.Map.Invalidate();
                }
            }
        }
    }
}