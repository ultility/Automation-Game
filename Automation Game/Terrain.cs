using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Automation_Game
{
    public class Terrain
    {
        public string type { get; }
        public static int size { get; } = 70;
        public int id { get; }

        Structure structure;
        public Terrain(string type, int id)
        {
            this.type = type;
            this.id = id;
            structure = null;
        }

        public string GetStructure()
        {
            if (structure == null)
            {
                return null;
            }
            return structure.name;
        }

        public bool BuildStructure(Structure structure)
        {
            if (this.structure == null)
            {
                this.structure = structure;
                return true;
            }
            return false;
        }

        public bool DestroyStructure(Player p)
        {
            if (structure != null && structure.destory(p))
            {
                structure = null;
                return true;
            }
            return false;
        }
    }
}