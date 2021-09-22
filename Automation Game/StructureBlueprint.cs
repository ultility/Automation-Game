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

namespace Automation_Game
{
    public class StructureBlueprint : Structure
    {
        public Structure result { get; }
        List<Delivery> recipe;
        Terrain parent;
        public StructureBlueprint(Structure structure, Delivery[] recipe, Terrain parent) : base(structure.name + "blueprint", structure.id, structure.sizePercentage, null, null, 0)
        {
            result = structure;
            this.recipe = recipe.ToList();
            this.parent = parent;
        }

        public void SetTerrain(Terrain t)
        {
            parent = t;
        }

        public bool AddItem(Item item)
        {
            for (int i = 0; i < recipe.Count; i++)
            {
                if (recipe[i].item.Equals(item))
                {
                    recipe[i].amount--;
                    if (recipe[i].amount == 0)
                    {
                        recipe.RemoveAt(i);
                        if (recipe.Count == 0)
                        {
                            parent.build(this);
                            parent.GetActivity().Hide_Inventory();
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override bool destory(Player p)
        {
            return true;
        }
    }
}