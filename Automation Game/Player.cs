using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
namespace Automation_Game
{
    public class Player : Moveable
    {
        //string mode;
        readonly Inventory inv;
        public const int INVENTORY_SIZE = 8;
        Thread t;
        public int Id { get; }
        int dx, dy;
        int i, frames = 10;

        public Player(int x, int y, MapDraw parent)
        {
            this.x = x;
            this.y = y;
            //mode = "awake";
            inv = new Inventory(INVENTORY_SIZE);
            Id = 4;
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

        public override void MoveTo(int targetX, int targetY)
        {
            if (packet == null)
            {
                packet = new MovementPacket(this, targetX, targetY);
            }
            else
            {
                packet.targetX = targetX;
                packet.targetY = targetY;
            }
            if (t == null || !t.IsAlive)
            {
                t = new Thread(new ParameterizedThreadStart(MoveTo));
                t.Start(packet);
            }

        }

        private void MoveTo(Object obj)
        {
            bool canMoveX = true;
            bool canMoveY = true;
            MovementPacket packet = (MovementPacket)obj;
            Random rng = new Random();
            dx = UpdateDX(packet);
            dy = UpdateDY(packet);
            Terrain step;
            while (dx != 0 || dy != 0 && (canMoveX || canMoveY))
            {
                if (rng.Next(2) == 0 || (canMoveX && !canMoveY))
                {
                    step = parent.Generator.GetTerrain()[(int)packet.moving.GetX() + dx, (int)packet.moving.GetY()];
                    if (!step.Type.Equals("water") && step.GetStructure() == null)
                    {
                        canMoveY = true;
                        canMoveX = true;
                        i = 0;
                        Timer t = new Timer(50);
                        t.Elapsed += Timer1_Elapsed;
                        t.Start();
                        while (t.Enabled) ;
                        packet.moving.SetX((float)Math.Round(packet.moving.GetX()));
                    }
                    else
                    {
                        canMoveX = false;
                        if (!(canMoveX || canMoveY))
                        {
                            TryUsing(parent.Generator.TerrainMap[packet.targetX, packet.targetY]);
                        }
                    }
                }
                else
                {
                    step = parent.Generator.GetTerrain()[(int)packet.moving.GetX(), (int)packet.moving.GetY() + dy];
                    if (!step.Type.Equals("water") && step.GetStructure() == null)
                    {
                        canMoveY = true;
                        canMoveX = true;
                        i = 0;
                        Timer t = new Timer(50);
                        t.Elapsed += Timer2_Elapsed;
                        t.Start();
                        while (t.Enabled) ;
                        packet.moving.SetY((float)Math.Round(packet.moving.GetY()));
                    }
                    else
                    {
                        canMoveY = false;
                        if (!(canMoveX || canMoveY))
                        {

                            TryUsing(parent.Generator.TerrainMap[packet.targetX, packet.targetY]);
                        }
                    }
                }
                dx = UpdateDX(packet);
                dy = UpdateDY(packet);
            }
            while (dx != 0 && canMoveX)
            {
                canMoveY = false;
                step = parent.Generator.GetTerrain()[(int)packet.moving.GetX() + dx, (int)packet.moving.GetY()];
                if (!step.Type.Equals("water") && step.GetStructure() == null)
                {
                    canMoveX = true;
                    i = 0;
                    Timer t = new Timer(50);
                    t.Elapsed += Timer1_Elapsed;
                    t.Start();
                    while (t.Enabled) ;
                    packet.moving.SetX((float)Math.Round(packet.moving.GetX()));
                }
                else
                {
                    canMoveX = false;
                    if (!(canMoveX || canMoveY))
                    {

                        TryUsing(parent.Generator.TerrainMap[packet.targetX, packet.targetY]);
                    }
                }
                dx = UpdateDX(packet);
                dy = UpdateDY(packet);
            }
            while (dy != 0 && canMoveY)
            {
                canMoveY = false;
                step = parent.Generator.GetTerrain()[(int)packet.moving.GetX(), (int)packet.moving.GetY() + dy];
                if (!step.Type.Equals("water") && step.GetStructure() == null)
                {
                    canMoveX = true;
                    i = 0;
                    Timer t = new Timer(50);
                    t.Elapsed += Timer2_Elapsed;
                    t.Start();
                    while (t.Enabled) ;
                    packet.moving.SetY((float)Math.Round(packet.moving.GetY()));
                }
                else
                {
                    canMoveX = false;
                    if (!(canMoveX || canMoveY))
                    {
                        TryUsing(parent.Generator.TerrainMap[packet.targetX, packet.targetY]);
                    }
                }
                dx = UpdateDX(packet);
                dy = UpdateDY(packet);
            }
            if (dx == 0 && dy == 0)
            {
                PickUp();
            }
        }


        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (i++ < frames)
            {
                Move((float)dx / frames, 0);
                GetParent().Invalidate();
            }
            else
            {
                ((Timer)sender).Stop();
                ((Timer)sender).Close();
                ((Timer)sender).Dispose();
            }
        }
        private void Timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (i++ < frames)
            {
                Move(0, (float)dy / frames);
                GetParent().Invalidate();
            }
            else
            {
                ((Timer)sender).Stop();
                ((Timer)sender).Close();
                ((Timer)sender).Dispose();
            }
        }

        private void TryUsing(Terrain t)
        {
            if (t.GetStructure() != null)
            {
                if (!t.UseStructure(packet))
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


        private int UpdateDX(MovementPacket packet)
        {
            if (packet.moving.GetX() < packet.targetX)
            {
                return 1;
            }
            else if (packet.moving.GetX() > packet.targetX)
            {
                return -1;
            }
            return 0;
        }

        private int UpdateDY(MovementPacket packet)
        {
            if (packet.moving.GetY() < packet.targetY)
            {
                return 1;
            }
            else if (packet.moving.GetY() > packet.targetY)
            {
                return -1;
            }
            return 0;
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