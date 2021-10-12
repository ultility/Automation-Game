namespace Automation_Game
{
    class CraftingRecipe
    {
        public Item Result { get; }
        readonly Delivery[] Recipe;

        public CraftingRecipe(Item result, Delivery[] recipe)
        {
            this.Result = result;
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
                    if (inv[n] != null && inv[n].Equals(Recipe[i].item))
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
                        if (inv[n] != null && inv[n].Equals(Recipe[i].item))
                        {
                            inv[n] = null;
                            amount--;
                            p.DropItem(n, false);
                        }
                    }
                }
                p.SortInventory();
                if (Result is Tool tool)
                {
                    p.GiveItem(new Tool(tool));
                }
                else
                {
                    p.GiveItem(new Item(Result));
                }
                return true;
            }
            return false;
        }
    }
}