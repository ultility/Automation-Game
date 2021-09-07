using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Automation_Game
{
    public class Player : Moveable
    {
        //string mode;
        Inventory inv;
        public const int INVENTORY_SIZE = 8;
        public int id { get; }

        public Player(int x, int y, Context context, MapDraw parent)
        {
            this.x = x;
            this.y = y;
            //mode = "awake";
            inv = new Inventory(INVENTORY_SIZE);
            id = 4;
            this.parent = parent;
        }

        public override void MoveTo(int targetX, int targetY)
        {
            MovementPacket packet = new MovementPacket(this, targetX, targetY);
            if (movementThread != null && movementThread.IsAlive)
            {
                movementThread.Abort();
            }
            movementThread = new Thread(new ParameterizedThreadStart(MoveTo));
            movementThread.Start(packet);
        }

        private void MoveTo(Object obj)
        {
            MovementPacket packet = (MovementPacket)obj;
            Random rng = new Random();
            int dx = 0;
            int dy = 0;
            if (packet.moving.GetX() > packet.targetX)
            {
                dx = -1;
            }
            else
            {
                dx = 1;
            }
            if (packet.moving.GetY() > packet.targetY)
            {
                dy = -1;
            }
            else
            {
                dy = 1;
            }
            while (packet.moving.GetX() != packet.targetX && packet.moving.GetY() != packet.targetY)
            {
                if (rng.Next(2) == 0)
                {
                    if (!parent.generator.GetTerrain()[packet.moving.GetX() + dx, packet.moving.GetY()].type.Equals("water"))
                    {
                        packet.moving.Move(dx, 0);
                        packet.moving.GetParent().Invalidate();
                        Thread.Sleep(500);
                    }
                    
                }
                else
                {
                    if (!parent.generator.GetTerrain()[packet.moving.GetX(), packet.moving.GetY() + dy].type.Equals("water"))
                    {
                        packet.moving.Move(0, dy);
                        packet.moving.GetParent().Invalidate();
                        Thread.Sleep(500);
                    }
                }
            }
            while (packet.moving.GetX() != packet.targetX)
            {
                packet.moving.Move(dx, 0);
                packet.moving.GetParent().Invalidate();
                Thread.Sleep(500);
            }
            while (packet.moving.GetY() != packet.targetY)
            {
                packet.moving.Move(0, dy);
                packet.moving.GetParent().Invalidate();
                Thread.Sleep(500);
            }
            PickUp();
        }

        public bool PickUp()
        {
            if (!inv.IsFull())
            {
                if (parent.groundItems[x,y] != null)
                {
                    inv.AddItem(parent.groundItems[x, y]);
                    parent.groundItems[x, y] = null;
                    parent.Invalidate();
                    return true;
                }
            }
            return false;
        }

        public Item[] GetInvetory()
        {
            Item[] items = new Item[INVENTORY_SIZE];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = inv.GetItem(i);
            }
            return items;
        }

        public Item dropItem(int index)
        {
            return inv.RemoveItem(index);
        }
    }
}