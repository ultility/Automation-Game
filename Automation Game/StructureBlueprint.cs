using System.Collections.Generic;
using System.Linq;

namespace Automation_Game
{
    public class StructureBlueprint : Structure
    {
        public Structure Result { get; }
        public readonly List<Delivery> Recipe;
        Terrain Parent;
        public StructureBlueprint(Structure structure, Delivery[] recipe, Terrain parent) : base(structure.Name + "blueprint", structure.Id, structure.SizePercentage, null, (Item)null, 0)
        {
            Result = structure;
            this.Recipe = recipe.ToList();
            this.Parent = parent;
            Walkable = true;
        }

        public void SetTerrain(Terrain t)
        {
            Parent = t;
        }

        public bool AddItem(Item item)
        {
            for (int i = 0; i < Recipe.Count; i++)
            {
                if (Recipe[i].item.Equals(item))
                {
                    Recipe[i].amount--;
                    if (Recipe[i].amount == 0)
                    {
                        Recipe.RemoveAt(i);
                        if (Recipe.Count == 0)
                        {
                            Parent.Build(this);
                            Parent.GetActivity().Hide_Inventory();
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override bool Destory(Player p)
        {
            return true;
        }
    }
}