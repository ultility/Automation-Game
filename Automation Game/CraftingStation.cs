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
    class CraftingStation : Structure
    {
        CraftingRecipe[] recipes;
        Context c;
        public CraftingStation(Context context, string name = "CraftingStation", int id = 9, int size = 1, Item useableItem = null, Item droppedItem = null) : base(name,id,size,useableItem,droppedItem)
        {
            recipes = null;
            c = context;
        }

        public void use(Player p)
        {
            Handler handle = new Handler(Looper.MainLooper);
            handle.Post(new Action(DisplayMenu));
            
        }

        private void DisplayMenu()
        {
            AlertDialog d;
            AlertDialog.Builder build = new AlertDialog.Builder(c);
            build.SetCancelable(true);
            d = build.Create();
            d.Show();
            d.Window.SetLayout(((Activity)c).Window.DecorView.Width * 8 / 10, ((Activity)c).Window.DecorView.Height * 8 / 10);
        }
    }
}