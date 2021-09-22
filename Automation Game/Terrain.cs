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

        Item item;
        int x;
        int y;

        Structure structure;

        GameActivity activity;
        public Terrain(string type, int id, int x, int y, GameActivity activity = null)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.id = id;
            structure = null;
            this.activity = activity;
        }

        public Item GetItem()
        {
            return item;
        }
        public void SetItem(Item item)
        {
            this.item = item;
        }

        public void SetActivity(GameActivity activity)
        {
            this.activity = activity;
        }

        public GameActivity GetActivity()
        {
            return activity;
        }
        
        public Structure GetStructure()
        {
            return structure;
        }

        public string GetStructureName()
        {
            if (structure == null)
            {
                return null;
            }
            return structure.name;
        }

        public int GetStructureId()
        {
            return structure.id;
        }

        public bool BuildStructure(Structure structure)
        {
            if (this.structure == null && structure != null && structure.isBuildable(this) && item == null)
            {
                this.structure = structure;
                return true;
            }
            return false;
        }

        public bool UseStructure(MovementPacket packet)
        {
            if (Math.Abs(packet.moving.GetX() - x) <= 1 && Math.Abs(packet.moving.GetY() - y) <= 1)
            {
                if (structure is CraftingStation cs)
                {
                    if (packet.moving is Player p)
                    {
                        cs.use(p);
                        return true;
                    } 
                }
                else if (structure is StructureBlueprint)
                {
                    if (activity != null)
                    {
                        Handler handle = new Handler(Looper.MainLooper);
                        handle.Post(OpenBlueprint);
                        return true;
                    }
                    
                }
            }
            return false;
        }

        private void OpenBlueprint()
        {
            StructureBlueprint sb = (StructureBlueprint)structure;
            activity.UsedBlueprint = sb;
            activity.DisplayInventory_Click(null, null);
        }


        public bool DestroyStructure(Player p)
        {
            if (structure != null)
            {
                if (structure.destory(p))
                {
                    item = structure.dropItem;
                    structure = null;
                    return true;
                }
            }
            return false;
        }

        public void build(StructureBlueprint sender)
        {
            if (structure is StructureBlueprint sb)
            {
                if (sb == sender)
                {
                    structure = sb.result;
                    activity.map.Invalidate();
                }
            }
        }
    }
}