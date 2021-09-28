using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation_Game.PathFinding;
using Timer = System.Timers.Timer;
namespace Automation_Game
{
    public class Player : Moveable, IUpdateable
    {
        //string mode;
        readonly Inventory inv;
        public const int INVENTORY_SIZE = 8;
        public int Id { get; }
        int dx, dy;
        int i = 10, j = 10, frames = 10;
        List<Point> MovementPath;
        Random rng = new Random();
        bool updating;
        public Player(int x, int y, MapDraw parent)
        {
            this.x = x;
            this.y = y;
            //mode = "awake";
            inv = new Inventory(INVENTORY_SIZE);
            Id = 4;
            this.parent = parent;
            MovementPath = new List<Point>();
            updating = false;
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
            inv.Equip(n);
        }

        public bool DeEquip()
        {
            return inv.DeEquip();
        }

        public bool IsInventoryFull()
        {
            return inv.IsFull();
        }

        public Task Update()
        {
            if (!updating)
            {
                return Task.Run(() =>
                {
                    if (MovementPath.Count > 0)
                    {
                        if (i == frames || j == frames)
                        {
                            if (MovementPath[0].X != x && MovementPath[0].Y != y)
                            {
                                if (rng.Next(2) == 0)
                                {
                                    i = 0;
                                }
                                else
                                {
                                    j = 0;
                                }
                            }
                            else if (i == frames && MovementPath[0].X != x)
                            {
                                i = 0;
                            }
                            else if (j == frames && MovementPath[0].Y != y)
                            {
                                j = 0;
                            }
                        }
                        if (i < frames)
                        {
                            i++;
                            Move((float)dx / frames, 0);
                            if (i == frames)
                            {
                                x = (int)Math.Round(x);
                                UpdateDX(packet.targetX);
                                if (dx == 0 || (MovementPath.Count == 1 && Terrain.IsWalkable(parent.Generator.TerrainMap[MovementPath[0].X, MovementPath[0].Y])))
                                {
                                    TryUsing(parent.Generator.TerrainMap[MovementPath[0].X, MovementPath[0].Y]);
                                }
                                if (x == MovementPath[0].X && y == MovementPath[0].Y)
                                {
                                    MovementPath.RemoveAt(0);
                                }
                            }
                        }
                        else if (j < frames)
                        {
                            j++;
                            Move(0, (float)dy / frames);
                            if (j == frames)
                            {
                                y = (int)Math.Round(y);
                                UpdateDY(packet.targetY);
                                if (dy == 0 || (MovementPath.Count == 1 && Terrain.IsWalkable(parent.Generator.TerrainMap[MovementPath[0].X, MovementPath[0].Y])))
                                {
                                    TryUsing(parent.Generator.TerrainMap[MovementPath[0].X, MovementPath[0].Y]);
                                }
                                if (x == MovementPath[0].X && y == MovementPath[0].Y)
                                {
                                    MovementPath.RemoveAt(0);
                                }
                            }
                        }
                    }
                });
            }
            return Task.CompletedTask;
        }

        public override void MoveTo(int targetX, int targetY)
        {
            updating = true;
            if (packet == null)
            {
                packet = new MovementPacket(this, (int)x, (int)y);
            }
            if (packet.targetX != targetX || packet.targetY != targetY)
            {
                packet.targetX = targetX;
                packet.targetY = targetY;
                MovementPath = AStar.GetPath((int)x,(int)y,targetX,targetY, parent.Generator.TerrainMap);
                int oldDX = dx;
                int oldDY = dy;
                UpdateDX(targetX);
                UpdateDY(targetY);
                if (dx != oldDX && i == frames)
                {
                    i = -i;
                }
                if (dy != oldDY && j == frames)
                {
                    j = -j;
                }
            }
            updating = false;
        }

        private void TryUsing(Terrain t)
        {

            if (!t.UseStructure(packet))
            {
                if (t.GetStructure() != null)
                {
                    int Hardness = t.GetStructure().Hardness;
                    if (t.DestroyStructure(this))
                    {
                        if (GetEquippedItem() != null && GetEquippedItem().Use(Hardness))
                        {
                            inv.RemoveItem(-1, false);
                        }
                    }
                }
            }

        }


        private void UpdateDX(int targetX)
        {
            if (x < targetX)
            {
                dx = 1;
            }
            else if (x > targetX)
            {
                dx = -1;
            }
            else
            {
                dx = 0;
            }
        }

        private void UpdateDY(int targetY)
        {
            if (y < targetY)
            {
                dy = 1;
            }
            else if (y > targetY)
            {
                dy = -1;
            }
            else
            {
                dy = 0;
            }
        }

        private void Update(ref int dx, ref int dy, MovementPacket packet)
        {
            if (packet.moving.GetX() > packet.targetX)
            {
                dx = -1;
            }
            else if (packet.moving.GetX() < packet.targetX)
            {
                dx = 1;
            }
            else
            {
                dx = 0;
            }
            if (packet.moving.GetY() > packet.targetY)
            {
                dy = -1;
            }
            else if (packet.moving.GetY() < packet.targetY)
            {
                dy = 1;
            }
            else
            {
                dy = 0;
            }
        }

        public bool PickUp()
        {
            if (!inv.IsFull())
            {
                if (parent.Generator.TerrainMap[(int)x, (int)y].GetItem() != null)
                {
                    inv.AddItem(parent.Generator.TerrainMap[(int)x, (int)y].Pickup());
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
            inv.Sort();
        }

        public Item DropItem(int index, bool sort = true)
        {
            return inv.RemoveItem(index, sort);
        }

        public Player(Byte[] bytes)
        {
            int offset = 0;
            int length = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            Byte[] inventory = new Byte[length];
            Array.Copy(bytes, 4, inventory, 0, length);
            offset += length;
            inv = new Inventory(inventory);
            this.x = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            this.y = BitConverter.ToSingle(bytes, offset);
            Id = 4;
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