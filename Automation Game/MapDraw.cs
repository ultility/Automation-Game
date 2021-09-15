﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;

using Automation_Game.Map;
using Android.Graphics;
using Bitmap = Android.Graphics.Bitmap;
using System.Numerics;
using System.Threading;
using System.Reflection;

namespace Automation_Game
{

    public class MapDraw : View
    {
        public MapGenerator generator { get; }
        Context context;

        Vector2 renderDistance;
        Vector2 camera;

        Vector2 check;

        public Player player { get; }

        bool tap;

        bool Drawn;

        public const int spriteSheetColoumnCount = 6;

        public bool editMode;
        public Item[,] groundItems { get; set; }

        List<ItemType> itemTypeList;
        public MapDraw(Context context) : base(context)
        {
            this.context = context;
            generator = new MapGenerator(100, 100);
            renderDistance = new Vector2();
            player = new Player(generator.GetWidth() / 2, generator.GetHeight() / 2, context, this);
            camera.X = player.GetX();
            camera.Y = player.GetY();
            tap = false;
            editMode = false;
            Drawn = false;
            itemTypeList = new List<ItemType>();
            itemTypeList.Add(new ItemType("Stick", 0.05, 7, 1, new string[] { "dirt"}));
            itemTypeList.Add(new ItemType("Stone", 0.05, 6, 1, new string[] { "dirt" }));
            generateItems();
        }
        public MapDraw(Context context, MapGenerator gen, Item[,] GroundItems, Player p) : base(context)
        {
            this.context = context;
            generator = gen;
            renderDistance = new Vector2();
            player = p;
            p.SetParent(this);
            camera.X = player.GetX();
            camera.Y = player.GetY();
            correctCamera();
            tap = false;
            editMode = false;
            Drawn = false;
            itemTypeList = new List<ItemType>();
            itemTypeList.Add(new ItemType("Stick", 0.05, 7, 1, new string[] { "dirt" }));
            itemTypeList.Add(new ItemType("Stone", 0.05, 6, 1, new string[] { "dirt" }));
            groundItems = GroundItems;
        }

        protected override void OnDraw(Canvas canvas)
        {
            Drawn = false;
            int size = Terrain.size;
            renderDistance.X = (int)Math.Ceiling((double)(canvas.Width / Terrain.size));
            renderDistance.Y = (int)Math.Ceiling((double)(canvas.Height / Terrain.size));
            if (camera.X == 0)
            {
                camera.X = renderDistance.X / 2;
            }
            if (camera.Y == 0)
            {
                camera.Y = renderDistance.Y / 2;
            }
            Terrain[,] terrain = generator.GetTerrain();
            Bitmap spritesheet = BitmapFactory.DecodeResource(Resources, Resources.GetIdentifier("sprite_sheet", "drawable", context.PackageName));
            for (int x = (int)(camera.X - renderDistance.X / 2), posX = 0; x <= camera.X + renderDistance.X / 2; x++, posX++)
            {
                for (int y = (int)(camera.Y - renderDistance.Y / 2), posY = 0; y <= camera.Y + renderDistance.Y / 2; y++, posY++)
                {
                    Terrain tera = terrain[x, y];
                    int spriteSheetSignleWidth = spritesheet.GetScaledWidth(canvas) / spriteSheetColoumnCount; 
                    Rect src = new Rect(spriteSheetSignleWidth * (terrain[x, y].id % spriteSheetColoumnCount), spriteSheetSignleWidth * (terrain[x,y].id / spriteSheetColoumnCount), spriteSheetSignleWidth * (terrain[x, y].id % spriteSheetColoumnCount) + spriteSheetSignleWidth, spriteSheetSignleWidth * (terrain[x, y].id / spriteSheetColoumnCount) + spriteSheetSignleWidth);
                    Rect dst = new Rect(posX * Terrain.size, posY * Terrain.size, posX * Terrain.size + Terrain.size, posY * Terrain.size + Terrain.size);
                    canvas.DrawBitmap(spritesheet,src, dst, null);
                    if (groundItems[x, y] != null)
                    {
                        src = new Rect(spriteSheetSignleWidth * (groundItems[x,y].id % spriteSheetColoumnCount), spriteSheetSignleWidth * (groundItems[x, y].id) / spriteSheetColoumnCount, spriteSheetSignleWidth * (groundItems[x, y].id % spriteSheetColoumnCount) + spriteSheetSignleWidth, spriteSheetSignleWidth * (groundItems[x, y].id / spriteSheetColoumnCount) + spriteSheetSignleWidth);
                        dst = new Rect(posX * (int)(Terrain.size * groundItems[x, y].sizePercentage), posY * (int)(Terrain.size * groundItems[x, y].sizePercentage), (posX + 1) * (int)(Terrain.size * groundItems[x,y].sizePercentage), (posY + 1) * (int)(Terrain.size * groundItems[x, y].sizePercentage));
                        canvas.DrawBitmap(spritesheet, src, dst, null);
                    }
                }
            }
            if (player.GetX() >= camera.X - renderDistance.X / 2 && player.GetX() <= camera.X + renderDistance.X / 2)
            {
                if (player.GetY() >= camera.Y - renderDistance.Y / 2 && player.GetY() <= camera.Y + renderDistance.Y / 2)
                {
                    int spriteSheetSignleWidth = spritesheet.GetScaledWidth(canvas) / spriteSheetColoumnCount;
                    int playerDrawY = player.GetY() - (int)(camera.Y - renderDistance.Y / 2);
                    int playerDrawX = player.GetX() - (int)(camera.X - renderDistance.X / 2);
                    Rect src = new Rect(spriteSheetSignleWidth * (player.id % spriteSheetColoumnCount), spriteSheetSignleWidth * (player.id / spriteSheetColoumnCount), spriteSheetSignleWidth * (player.id % spriteSheetColoumnCount) + spriteSheetSignleWidth, spriteSheetSignleWidth * (player.id / spriteSheetColoumnCount) + spriteSheetSignleWidth);
                    Rect dst = new Rect(playerDrawX * (int)(Terrain.size), playerDrawY * (int)(Terrain.size), (playerDrawX + 1) * (int)(Terrain.size), (playerDrawY + 1) * (int)(Terrain.size));
                    
                    canvas.DrawBitmap(spritesheet, src, dst, null);

                }
            }

            Drawn = true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (editMode)
            {
                return false;
            }
            if (e.Action == MotionEventActions.Down)
            {
                tap = true;
            }
            else if (e.Action == MotionEventActions.Up)
            {
                if (tap)
                {
                    int rowTilesFromCorner = (int)e.GetX() / Terrain.size;
                    int coloumnTilesFromCorner = (int)e.GetY() / Terrain.size;
                    int newX = (int)((camera.X - renderDistance.X / 2) + rowTilesFromCorner);
                    int newY = (int)((camera.Y - renderDistance.Y / 2) + coloumnTilesFromCorner);
                    player.MoveTo(newX, newY);
                    Invalidate();
                }

            }
            else if (e.Action == MotionEventActions.Move)
            {

                tap = false;
                if (e.HistorySize != 0)
                {
                    camera.X += (e.GetHistoricalX(0) - e.GetX()) / Terrain.size;
                    camera.Y += (e.GetHistoricalY(0) - e.GetY()) / Terrain.size;
                }
                correctCamera();
            }
            return true;
        }

        private void correctCamera()
        {
            int x = (int)camera.X;
            int y = (int)camera.Y;
            if (camera.X < renderDistance.X / 2)
            {
                camera.X = renderDistance.X / 2;
            }
            else if (camera.X + renderDistance.X / 2 >= generator.GetTerrain().GetLength(0))
            {
                camera.X = generator.GetTerrain().GetLength(0) - renderDistance.X - 1;
            }
            if (camera.Y < renderDistance.Y / 2)
            {
                camera.Y = renderDistance.Y / 2;
            }
            else if (camera.Y + renderDistance.Y / 2 >= generator.GetTerrain().GetLength(1))
            {
                camera.Y = generator.GetTerrain().GetLength(1) - renderDistance.Y - 1;
            }
            if ((int)camera.X != (int)check.X)
            {
                check.X = camera.X;
            }
            if ((int)camera.Y != (int)check.Y)
            {
                check.Y = camera.Y;
            }
            if (!((int)camera.X == x && (int)camera.Y == y))
            {
                Invalidate();
            }
        }

        public Bitmap DisplayMap(int width, int height)
        {
            float scale = Math.Min(width / generator.GetWidth(), height / generator.GetHeight());
            return Bitmap.CreateScaledBitmap(generator.generateMap(scale), (int)((width - generator.GetWidth() * scale) / 2), (int)((height - generator.GetHeight() * scale) / 2), false);
        }

        public bool IsDrawn()
        {
            return Drawn;
        }

        public void generateItems()
        {
            Terrain[,] terrain = generator.GetTerrain();
            groundItems = new Item[generator.GetWidth(), generator.GetHeight()];
            Random rng = new Random();
            for (int x = 0; x < generator.GetWidth(); x++)
            {
                for (int y = 0; y < generator.GetHeight(); y++)
                {
                    for (int i = 0; i < itemTypeList.Count && groundItems[x, y] == null; i++)
                    {
                        bool isSpawnable = false;
                        for (int n = 0; n < itemTypeList[i].spawnableTerrain.Length; n++)
                        {
                            if (terrain[x, y].type.Equals(itemTypeList[i].spawnableTerrain[n]))
                            {
                                isSpawnable = true;
                                break;
                            }
                        }
                        if (isSpawnable && rng.NextDouble() < itemTypeList[i].spawnChance)
                        {
                            groundItems[x, y] = new Item(itemTypeList[i].name, itemTypeList[i].id, itemTypeList[i].sizePercentage);
                        }
                    }
                }
            }
        }

        public bool dropItem(int index)
        {
            if (groundItems[player.GetX(), player.GetY()] == null)
            {
                Item dropped = player.dropItem(index);
                groundItems[player.GetX(), player.GetY()] = dropped;
                return true;
            }
            return false;
        }

        public void save()
        {
            try
            {
                using (Stream stream = context.OpenFileOutput("world.txt", Android.Content.FileCreationMode.Private))
                {
                    try
                    {
                        Byte[] generation = generator.GetBytes();
                        stream.Write(BitConverter.GetBytes(generation.Length));
                        stream.Write(generation);
                        Byte[] player = this.player.ToByte();
                        stream.Write(BitConverter.GetBytes(player.Length));
                        stream.Write(player);
                        Byte[] items = GroundItemsToByte();
                        stream.Write(BitConverter.GetBytes(items.Length));
                        stream.Write(items);
                        stream.Close();
                    }
                    catch (Java.IO.IOException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }
            catch(Java.IO.FileNotFoundException e)
                    {
                e.PrintStackTrace();
            }
        }

        private Byte[] GroundItemsToByte()
        {
            List<Byte> bytes = new List<byte>();
            bool coordinates = true;
            int length1 = groundItems.GetLength(0);
            int length2 = groundItems.GetLength(1);
            for (int x = 0; x < groundItems.GetLength(0); x++)
            {
                for (int y = 0; y < groundItems.GetLength(1); y++)
                {
                    if (groundItems[x,y] != null)
                    {
                        if (coordinates)
                        {
                            bytes.AddRange(BitConverter.GetBytes(x));
                            bytes.AddRange(BitConverter.GetBytes(y));
                            
                        }
                        Byte[] item = groundItems[x, y].ToByte();
                        bytes.AddRange(BitConverter.GetBytes(item.Length));
                        bytes.AddRange(item);
                        coordinates = false;
                    }
                    else
                    {
                        if (bytes.Count != 0 && BitConverter.ToInt32(bytes.GetRange(bytes.Count - 5, 4).ToArray()) != 0) {
                            coordinates = true;
                            bytes.AddRange(BitConverter.GetBytes(0));
                        }
                    }
                }
            }
            return bytes.ToArray();
        }
    }
}