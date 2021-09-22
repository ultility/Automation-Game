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

        public void SetParent(MapDraw p)
        {
            this.parent = p;
        }

        public bool GiveItem(Item item)
        {
            return inv.AddItem(item);
        }

        public void Equip(int n)
        {
            inv.equip(n);
        }

        public bool DeEquip()
        {
            return inv.deequip();
        }

        public override void MoveTo(int targetX, int targetY)
        {
            if (packet == null) {
                packet = new MovementPacket(this, targetX, targetY);
            }
            else
            {
                packet.targetX = targetX;
                packet.targetY = targetY;
            }
            if (movementThread == null || !movementThread.IsAlive)
            {
                movementThread = new Thread(new ParameterizedThreadStart(MoveTo));
                movementThread.Start(packet);
            }
        }

        private void MoveTo(Object obj)
        {
            bool canMoveX = true;
            bool canMoveY = true;
            MovementPacket packet = (MovementPacket)obj;
            Random rng = new Random();
            int dx = 0;
            int dy = 0;
            update(ref  dx, ref dy, packet);
            Terrain step;
            while ((int)packet.moving.GetX() != packet.targetX && (int)packet.moving.GetY() != packet.targetY)
            {
                update(ref dx, ref dy, packet);
                if (rng.Next(2) == 0)
                {
                    step = parent.generator.GetTerrain()[(int)packet.moving.GetX() + dx, (int)packet.moving.GetY()];
                    if (!step.type.Equals("water") && step.GetStructure() == null)
                    {
                        canMoveY = true;
                        canMoveX = true;
                        for (int i = 0; i < 10; i++)
                        {
                            packet.moving.Move((float)dx / 10, 0);
                            packet.moving.GetParent().Invalidate();
                            Thread.Sleep(500 / 10);
                        }
                    }
                    else
                    {
                        canMoveX = false;
                        if (!(canMoveX || canMoveY))
                        {

                            if (parent.generator.terrainMap[packet.targetX, packet.targetY].GetStructure() != null)
                            {
                                parent.generator.terrainMap[packet.targetX, packet.targetY].UseStructure(packet);
                                parent.generator.terrainMap[packet.targetX, packet.targetY].DestroyStructure(this);
                            }
                            Thread.CurrentThread.Abort();
                        }
                    }
                }
                else
                {
                    step = parent.generator.GetTerrain()[(int)packet.moving.GetX(), (int)packet.moving.GetY() + dy];
                    if (!step.type.Equals("water") && step.GetStructure() == null)
                    {
                        canMoveY = true;
                        canMoveX = true;
                        for (int i = 0; i < 10; i++)
                        {
                            packet.moving.Move(0, (float)dy / 10);
                            packet.moving.GetParent().Invalidate();
                            Thread.Sleep(500 / 10);
                        }
                    }
                    else
                    {
                        canMoveY = false;
                        if (!(canMoveX || canMoveY))
                        {

                            if (parent.generator.terrainMap[packet.targetX, packet.targetY].GetStructure() != null)
                            {
                                parent.generator.terrainMap[packet.targetX, packet.targetY].UseStructure(packet);
                                parent.generator.terrainMap[packet.targetX, packet.targetY].DestroyStructure(this);
                            }
                            Thread.CurrentThread.Abort();
                        }
                    }
                }
            }
            while ((int)packet.moving.GetX() != packet.targetX)
            {
                update(ref dx, ref dy, packet);
                canMoveY = false;
                step = parent.generator.GetTerrain()[(int)packet.moving.GetX() + dx, (int)packet.moving.GetY()];
                if (!step.type.Equals("water") && step.GetStructure() == null)
                {
                    canMoveX = true;
                    for (int i = 0; i < 10; i++)
                    {
                        packet.moving.Move((float)dx / 10, 0);
                        packet.moving.GetParent().Invalidate();
                        Thread.Sleep(500 / 10);
                    }
                }
                else
                {
                    canMoveX = false;
                    if (!(canMoveX || canMoveY))
                    {

                        if (parent.generator.terrainMap[packet.targetX, packet.targetY].GetStructure() != null)
                        {
                            parent.generator.terrainMap[packet.targetX, packet.targetY].UseStructure(packet);
                            int hardness = parent.generator.terrainMap[packet.targetX, packet.targetY].GetStructure().hardness;
                            if (parent.generator.terrainMap[packet.targetX, packet.targetY].DestroyStructure(this))
                            {
                                if (GetEquippedItem().use(hardness))
                                {
                                    inv.RemoveItem(-1, false);
                                }
                            }
                        }
                        Thread.CurrentThread.Abort();
                    }
                }
            }
            while ((int)packet.moving.GetY() != packet.targetY)
            {
                update(ref dx, ref dy, packet);
                canMoveX = false;
                step = parent.generator.GetTerrain()[(int)packet.moving.GetX(), (int)packet.moving.GetY() + dy];
                if (!step.type.Equals("water") && step.GetStructure() == null)
                {
                    canMoveY = true;
                    for (int i = 0; i < 10; i++)
                    {
                        packet.moving.Move(0, (float)dy / 10);
                        packet.moving.GetParent().Invalidate();
                        Thread.Sleep(500 / 10);
                    }
                }
                else
                {
                    canMoveY = false;
                    if (!(canMoveX || canMoveY))
                    {

                        if (parent.generator.terrainMap[packet.targetX, packet.targetY].GetStructure() != null)
                        {
                            parent.generator.terrainMap[packet.targetX, packet.targetY].UseStructure(packet);
                            parent.generator.terrainMap[packet.targetX, packet.targetY].DestroyStructure(this);
                        }
                        Thread.CurrentThread.Abort();
                    }
                }
            }
            PickUp();

        }

        private void update(ref int dx, ref int dy, MovementPacket packet)
        {
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
        }

        public bool PickUp()
        {
            if (!inv.IsFull())
            {
                if (parent.generator.terrainMap[(int)x, (int)y].GetItem() != null)
                {
                    inv.AddItem(parent.generator.terrainMap[(int)x, (int)y].GetItem());
                    parent.generator.terrainMap[(int)x, (int)y].SetItem(null);
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

        public Tool GetEquippedItem()
        {
            return (Tool)inv.GetItem(-1);
        }

        public void SortInventory()
        {
            inv.sort();
        }

        public Item dropItem(int index, bool sort = true)
        {
            return inv.RemoveItem(index, sort);
        }

        public Player(Byte[] bytes, int x, int y)
        {
            int offset = 0;
            int length = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            Byte[] inventory = new Byte[length];
            Array.Copy(bytes, 4, inventory, 0, length);
            offset += length;
            inv = new Inventory(inventory);
            this.x = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            this.y = BitConverter.ToInt32(bytes, offset);
            id = 4;
        }

        public Byte[] ToByte()
        {
            List<Byte> bytes = new List<byte>();
            Byte[] inv = this.inv.ToByte();
            bytes.AddRange(BitConverter.GetBytes(inv.Length));
            bytes.AddRange(inv);
            bytes.AddRange(BitConverter.GetBytes(x));
            bytes.AddRange(BitConverter.GetBytes(y));
            return bytes.ToArray();
        }
    }
}