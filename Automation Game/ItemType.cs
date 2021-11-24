using System.Collections.Generic;

namespace Automation_Game
{
    public class ItemType
    {
        public enum ItemTypes
        {
            FALSE = -1,
            STICK,
            STONE,
            TREE_SEED,
            LOG,
            AXE,
            PICKAXE,
            SHOVEL,

            AMOUNT
        };
        public static List<ItemType> itemTypeList = new List<ItemType> { new ItemType("Stick", 0.05, (int)GameActivity.IDs.STICK, 1, new string[] { "dirt" }),
                                                    new ItemType("Stone", 0.05, (int)GameActivity.IDs.STONE, 1, new string[] { "dirt" }),
                                                    new ItemType("Tree Seed", 0,(int)GameActivity.IDs.TREE_SEED,1,new string[] { }),
                                                    new ItemType("Log", 0, (int)GameActivity.IDs.WOOD_LOG, 1, new string[]{ }),
                                                    new ItemType("Axe", 0, (int)GameActivity.IDs.AXE, 1, new string[]{ }, 20),
                                                    new ItemType("Pickaxe", 0, (int)GameActivity.IDs.PICKAXE, 1, new string[]{ }, 20),
                                                    new ItemType("Shovel", 0, (int)GameActivity.IDs.SHOVEL, 1, new string[]{ }, 20),};
        public string name { get; }
        public double spawnChance { get; }
        public int id { get; }
        public double sizePercentage { get; }
        public string[] spawnableTerrain { get; }
        public int durability { get; }

        public ItemType(string name, double spawnChance, int id, double size, string[] spawnableTerrain, int durability = 0)
        {
            this.name = name;
            this.spawnChance = spawnChance;
            this.id = id;
            this.sizePercentage = size;
            this.spawnableTerrain = spawnableTerrain;
            this.durability = durability;
        }

        public Item Create()
        {
            if (durability == 0)
            {
                return new Item(name, id, sizePercentage);
            }
            else
            {
                return new Tool(name, id, durability);
            }
        }

        public static Item Create(int id)
        {
            if (id < (int)ItemTypes.AMOUNT && id >= 0)
            {
                return itemTypeList[id].Create();
            }
            return null;
        }
    }
}