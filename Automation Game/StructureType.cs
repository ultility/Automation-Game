using System.Collections.Generic;

namespace Automation_Game
{
    public class StructureType
    {
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
    }
}