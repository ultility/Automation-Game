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
    class CraftingRecipe
    {
        Item result { get; }
        Delivery[] Recipe;

        public CraftingRecipe(Item result, Delivery[] recipe)
        {
            this.result = result;
            Recipe = recipe;
        }

        public bool Craft(Player p)
        {
            Item[] inv = p.GetInvetory();
            bool hasRequirments = true;
            for (int i = 0; i < Recipe.Length && hasRequirments; i++)
            {
                int amount = 0;
                for (int n = 0; n < inv.Length; n++)
                {
                    if (inv[n] == Recipe[i].item)
                    {
                        amount++;
                    }
                }
                if (amount < Recipe[i].amount)
                {
                    hasRequirments = false;
                }
            }
            if (hasRequirments)
            {
                for (int i = 0; i < Recipe.Length && hasRequirments; i++)
                {
                    int amount = Recipe[i].amount;
                    for (int n = 0; n < inv.Length && amount > 0; n++)
                    {
                        if (inv[n] == Recipe[i].item)
                        {
                            inv[n] = null;
                            amount--;
                            p.dropItem(n, false);
                        }
                    }
                }
            }
            p.SortInventory();
            p.GiveItem(result);
            return false;
            
        }
    }
}