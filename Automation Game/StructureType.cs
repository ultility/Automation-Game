using System.Collections.Generic;

namespace Automation_Game
{
    public class StructureType
    {
        public enum StructureTypes
        {
            TREE,
            DIRT_HOLE,
            WOOD_FLOOR,
            WOOD_WALL,

            AMOUNT
        };
        public static List<StructureType> structureTypeList = new List<StructureType> { new StructureType("tree", (int)GameActivity.IDs.TREE, 1, new Item[] { ItemType.Create((int)ItemType.ItemTypes.LOG), ItemType.Create((int)ItemType.ItemTypes.TREE_SEED)}, (Tool)ItemType.Create((int)ItemType.ItemTypes.AXE), 0.08, new string[] { "dirt" }, 1),
                                                                                        new StructureType("Dirt Hole", (int)GameActivity.IDs.DIRT_HOLE, 1, (Item)null, ItemType.Create((int)ItemType.ItemTypes.TREE_SEED), 0, new string[]{ }, 0, true),
                                                                                        new StructureType("Wood Floor", (int)GameActivity.IDs.WOOD_FLOOR, 1, (Item)null, (Item)null, 0, new string[]{ }, 0, true),
                                                                                        new StructureType("Wood Wall", (int)GameActivity.IDs.WOOD_WALL, 1, (Item)null, (Item)null, 0, new string[]{ }, 0)};
        public string Name { get; }
        public int Id { get; }
        public int SizePercentage { get; }
        public List<Item> DropItem { get; }
        public Item UseableItem { get; }
        public double SpawnChance { get; }
        public string[] SpawnableTerrain { get; }
        public int Hardness { get; }
        public bool Walkable { get; }

        public StructureType(string name, int id, int size, Item DropItem, Item UseableItem, double spawnChance, string[] SpawnableTerrain, int Hardness, bool Walkable = true)
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

        public StructureType(string name, int id, int size, IEnumerable<Item> DropItem, Item UseableItem, double spawnChance, string[] SpawnableTerrain, int Hardness, bool Walkable = false)
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

        public Structure Create()
        {
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