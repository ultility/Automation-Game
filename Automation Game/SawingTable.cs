using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation_Game
{
    class SawingTable : Structure
    {
        CraftingRecipe Recipes;
        Context C;
        public SawingTable(Context context, string name = "SawingTable", int id = (int)GameActivity.IDs.CRAFTING_STATION, int size = 1, Item useableItem = null, Item droppedItem = null) : base(name, id, size, useableItem, droppedItem,0)
        {
            C = context;
        }

        public void Use(Player p)
        {

        }
    }
}