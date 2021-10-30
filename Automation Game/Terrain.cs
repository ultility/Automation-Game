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
            if (structure != null)
            {
                if (structure is Plant p)
                {
                    return p.Id + p.GrowthStage;
                }
            }
            return structure.Id;
        }

        public bool BuildStructure(Structure structure)
        {
            if (this.structure == null)
            {
                if (structure != null && structure.IsBuildable(this) && items.Count == 0)
                this.structure = structure;
                return true;
            }
            else if (this.structure.Name.Equals(MapDraw.structureTypeList[(int)MapDraw.StructureTypes.DIRT_HOLE].Name) && 
                     structure.Name.Equals(MapDraw.structureTypeList[(int)MapDraw.StructureTypes.TREE].Name))
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
                if (structure is IUseable useable)
                {
                    if (packet.moving is Player p)
                    {
                        useable.Use(p);
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
                else if (structure ==  null)
                {
                    if (packet.moving is Player p)
                    {
                        if (p.GetEquippedItem() != null && p.GetEquippedItem().name.Equals("Shovel"))
                        {
                            if (Type.Equals("dirt") && items.Count == 0)
                            {
                                structure = new Structure(MapDraw.structureTypeList[(int)MapDraw.StructureTypes.DIRT_HOLE]);
                                p.GetEquippedItem().Use(1);
                                return true;
                            }
                        }
                    }
                }
                else if (structure.Name.Equals(MapDraw.structureTypeList[(int)MapDraw.StructureTypes.DIRT_HOLE].Name))
                {
                    if (activity != null)
                    {
                        Handler handle = new Handler(Looper.MainLooper);
                        handle.Post(() =>
                        {
                            activity.hole = structure;
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
            if (p != null)
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
                                if (structure is Plant plant)
                                {
                                    if (plant.IsFullyGrown())
                                    {
                                        items.AddRange(structure.GetDropItems());
                                        structure = null;
                                    }
                                    else
                                    {
                                        structure = null;
                                    }
                                }
                                else
                                {
                                    items.AddRange(structure.GetDropItems());
                                    structure = null;
                                }
                                return true;
                            }
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
                    if (structure is IUpdateable updateable)
                    {
                        activity.Updateables.Add(updateable);
                    }
                    activity.Map.Invalidate();
                }
            }
        }

        public static bool IsWalkable(Terrain t)
        {
            return !t.Type.Equals("water") && (t.structure == null || t.structure.Walkable);
        }
    }
}