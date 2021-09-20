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
    public class Delivery
    {
        public Item item { get; }
        public int amount { get; set; }

        public Delivery(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }
}