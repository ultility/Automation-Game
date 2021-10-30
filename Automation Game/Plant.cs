using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Automation_Game
{
    public class Plant : Structure, IUpdateable
    {
        public int GrowthStage { get; private set; }
        int MaxGrowthStage;
        float GrowthChance;


        public Plant(float GrowthChance, int MaxGrowthStage, string name, int id, float size, Item useableTool, IEnumerable<Item> droppedItems, int Hardness, bool Walkable = false, int GrowthStage = 0) : base(name, id, size, useableTool, droppedItems, Hardness, Walkable)
        {
            this.GrowthStage = GrowthStage;
            this.MaxGrowthStage = MaxGrowthStage;
            this.GrowthChance = GrowthChance;
            if (this.GrowthStage > this.MaxGrowthStage)
            {
                this.GrowthStage = this.MaxGrowthStage;
            }
            else if(this.GrowthStage < 0)
            {
                this.GrowthStage = 0;
            }
        }

        public Plant(float GrowthChance, int MaxGrowthStage, StructureType type, int GrowthStage = 0) : base(type)
        {
            this.GrowthStage = GrowthStage;
            this.MaxGrowthStage = MaxGrowthStage;
            this.GrowthChance = GrowthChance;
            if (this.GrowthStage > this.MaxGrowthStage)
            {
                this.GrowthStage = this.MaxGrowthStage;
            }
            else if (this.GrowthStage < 0)
            {
                this.GrowthStage = 0;
            }
        }

        public override Item[] GetDropItems()
        {
            Random rng = new Random();
            List<Item> drops = new List<Item>(dropItems);
            for (int i = 0; i < dropItems.Count; i++)
            {
                if (dropItems[i].name.Contains("seed", StringComparison.OrdinalIgnoreCase) && rng.Next(3) == 0)
                {
                    drops.Add(new Item(dropItems[i]));
                }
            }
            return drops.ToArray();
        }

        public bool IsFullyGrown()
        {
            return GrowthStage == MaxGrowthStage;
        }

        public Task Update()
        {
            if (GrowthStage < MaxGrowthStage)
            {
                return Task.Run(() =>
                {
                    if (new Random().NextDouble() < GrowthChance)
                    {
                        GrowthStage++;
                        if (GrowthStage == MaxGrowthStage)
                        {
                            Walkable = false;
                        }
                    }
                });
            }
            
            return Task.CompletedTask;
        }
    }
}