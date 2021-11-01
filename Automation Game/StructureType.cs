using Android.Content;
using System.Collections.Generic;

namespace Automation_Game
{
    public class StructureType
    {
        public enum StructureTypes
        {
            FALSE = -1,
            TREE,
            DIRT_HOLE,
            WOOD_FLOOR,
            WOOD_WALL,
            ROCK,
            CRAFTING_STATION,
            STORAGE_BOX,
            SAWING_TABLE,
            AMOUNT
        };
        public static List<StructureType> structureTypeList = new List<StructureType> { new StructureType("tree", (int)GameActivity.IDs.TREE, 1, new Item[] { ItemType.Create((int)ItemType.ItemTypes.LOG), ItemType.Create((int)ItemType.ItemTypes.TREE_SEED)}, (Tool)ItemType.Create((int)ItemType.ItemTypes.AXE), 0.08, new string[] { "dirt" }, 1),
                                                                                        new StructureType("Dirt Hole", (int)GameActivity.IDs.DIRT_HOLE, 1, (Item)null, ItemType.Create((int)ItemType.ItemTypes.TREE_SEED), 0, new string[]{ }, 0, true),
                                                                                        new StructureType("Wood Floor", (int)GameActivity.IDs.WOOD_FLOOR, 1, (Item)null, (Item)null, 0, new string[]{ }, 0, true),
                                                                                        new StructureType("Wood Wall", (int)GameActivity.IDs.WOOD_WALL, 1, (Item)null, (Item)null, 0, new string[]{ }, 0),
                                                                                        new StructureType("Rock", (int)GameActivity.IDs.ROCK, .85f, ItemType.Create((int)ItemType.ItemTypes.STONE), (Tool)ItemType.Create((int)ItemType.ItemTypes.PICKAXE), 0, new string[] {}, 1, false),
                                                                                        new StructureType("Crafting Station", (int)GameActivity.IDs.CRAFTING_STATION, 1, (Item)null, null, 0, new string[] { }, 0, false),
                                                                                        new StructureType("Storage Box", (int)GameActivity.IDs.STORAGE_BOX, 1, (Item)null, null, 0, new string[] { }, 0, false),
                                                                                        new StructureType("Sawing Table", (int)GameActivity.IDs.SAWING_TABLE, 1, (Item)null, null, 0, new string[] { }, 0, false),
        };
        public string Name { get; }
        public int Id { get; }
        public float SizePercentage { get; }
        public List<Item> DropItem { get; }
        public Item UseableItem { get; }
        public double SpawnChance { get; }
        public string[] SpawnableTerrain { get; }
        public int Hardness { get; }
        public bool Walkable { get; }

        public StructureType(string name, int id, float size, Item DropItem, Item UseableItem, double spawnChance, string[] SpawnableTerrain, int Hardness, bool Walkable = true)
        {
            this.Name = name;
            this.Id = id;
            this.SizePercentage = size;
            this.DropItem = new List<Item>
            {
                DropItem
            };
            this.UseableItem = UseableItem;
            this.SpawnChance = spawnChance;
            this.SpawnableTerrain = SpawnableTerrain;
            this.Hardness = Hardness;
            this.Walkable = Walkable;
        }

        public StructureType(string name, int id, float size, IEnumerable<Item> DropItem, Item UseableItem, double spawnChance, string[] SpawnableTerrain, int Hardness, bool Walkable = false)
        {
            this.Name = name;
            this.Id = id;
            this.SizePercentage = size;
            this.DropItem = new List<Item>();
            this.DropItem.AddRange(DropItem);
            this.UseableItem = UseableItem;
            this.SpawnChance = spawnChance;
            this.SpawnableTerrain = SpawnableTerrain;
            this.Hardness = Hardness;
            this.Walkable = Walkable;
        }

        public Structure Create(Context c = null)
        {
            if (Id == (int)StructureTypes.CRAFTING_STATION)
            {
                return new CraftingStation(c);
            }
            else if (Id == (int)StructureTypes.STORAGE_BOX)
            {
                return new StorageChest(c);
            }
            else if (Id == (int)StructureTypes.SAWING_TABLE)
            {
                return new SawingTable(c);
            }
            return new Structure(Name, Id, SizePercentage, UseableItem, DropItem, Hardness, Walkable);
        }

        public static Structure Create(int id)
        {
            if (id < (int)StructureTypes.AMOUNT)
            {
                return structureTypeList[id].Create();
            }
            return null;
        }
    }
}