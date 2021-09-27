namespace Automation_Game
{
    public class ItemType
    {
        public string name { get; }
        public double spawnChance { get; }
        public int id { get; }
        public double sizePercentage { get; }
        public string[] spawnableTerrain { get; }

        public ItemType(string name, double spawnChance, int id, double size, string[] spawnableTerrain)
        {
            this.name = name;
            this.spawnChance = spawnChance;
            this.id = id;
            this.sizePercentage = size;
            this.spawnableTerrain = spawnableTerrain;
        }
    }
}