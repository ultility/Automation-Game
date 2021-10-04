using Android.Content;
using Android.Graphics;
using Android.Views;
using Automation_Game.Map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Bitmap = Android.Graphics.Bitmap;

namespace Automation_Game
{
    public class MapDraw : View
    {
        public enum ItemTypes
        {
            STICK,
            STONE,
            TREE_SEED
        };
        public enum StructureTypes
        {
            TREE,
            DIRT_HOLE
        };
        public MapGenerator Generator { get; }
        readonly Context context;

        Vector2 renderDistance;
        Vector2 camera;
        Vector2 LastPoint;
        public Player Player { get; }

        bool tap;

        bool Drawn;

        public const int spriteSheetColoumnCount = 6;

        public bool editMode;
        public StructureBlueprint CurrentlyBuilding;

        public static List<ItemType> itemTypeList = new List<ItemType> { new ItemType("Stick", 0.05, (int)GameActivity.IDs.STICK, 1, new string[] { "dirt" }),
                                                    new ItemType("Stone", 0.05, (int)GameActivity.IDs.STONE, 1, new string[] { "dirt" }),
                                                    new ItemType("Tree Seed", 0,(int)GameActivity.IDs.TREE_SEED,1,new string[] { }) };
        public static List<StructureType> structureTypeList = new List<StructureType> { new StructureType("tree", (int)GameActivity.IDs.TREE, 1, new Item[] { new Item(itemTypeList[(int)ItemTypes.STICK].name, itemTypeList[(int)ItemTypes.STICK].id, itemTypeList[(int)ItemTypes.STICK].sizePercentage), new Item(itemTypeList[(int)ItemTypes.TREE_SEED].name, itemTypeList[(int)ItemTypes.TREE_SEED].id, itemTypeList[(int)ItemTypes.TREE_SEED].sizePercentage) }, new Tool("Axe", (int)GameActivity.IDs.AXE, 20), 0.08, new string[] { "dirt" }, 1),
                                                                                        new StructureType("Dirt Hole", (int)GameActivity.IDs.DIRT_HOLE, 1, (Item)null, new Item(itemTypeList[(int)ItemTypes.TREE_SEED]), 0, new string[]{ }, 0)};
        public MapDraw(Context context) : base(context)
        {
            this.context = context;
            Generator = new MapGenerator(100, 100, (GameActivity)context);
            renderDistance = new Vector2();
            LastPoint = new Vector2();
            Player = new Player(Generator.GetWidth() / 2, Generator.GetHeight() / 2, this);
            ((GameActivity)context).Updateables.Add(Player);
            camera.X = Player.GetX();
            camera.Y = Player.GetY();
            tap = false;
            editMode = false;
            Drawn = false;
            GenerateItems();
            Generator.GetTerrain()[(int)Player.GetX() - 1, (int)Player.GetY()].BuildStructure(new StructureBlueprint(new CraftingStation(context), new Delivery[] { new Delivery(new Item(itemTypeList[(int)ItemTypes.STICK].name, itemTypeList[(int)ItemTypes.STICK].id, itemTypeList[(int)ItemTypes.STICK].sizePercentage), 1) }, Generator.GetTerrain()[(int)Player.GetX() - 1, (int)Player.GetY()]));
            CurrentlyBuilding = null;
        }
        public MapDraw(Context context, MapGenerator gen, Player p) : base(context)
        {
            this.context = context;
            Generator = gen;
            renderDistance = new Vector2();
            Player = p;
            ((GameActivity)context).Updateables.Add(Player);
            p.SetParent(this);
            camera.X = Player.GetX();
            camera.Y = Player.GetY();
            CorrectCamera();
            tap = false;
            editMode = false;
            Drawn = false;
        }

        protected override void OnDraw(Canvas canvas)
        {
            Drawn = false;
            renderDistance.X = (int)Math.Ceiling((double)(canvas.Width / Terrain.Size));
            renderDistance.Y = (int)Math.Ceiling((double)(canvas.Height / Terrain.Size));
            if (camera.X < renderDistance.X / 2)
            {
                camera.X = renderDistance.X / 2;
            }
            else if (camera.X > Generator.GetWidth() - renderDistance.X)
            {
                camera.X = Generator.GetWidth() - renderDistance.X;
            }
            if (camera.Y < renderDistance.Y / 2)
            {
                camera.Y = renderDistance.Y / 2;
            }
            else if (camera.Y > Generator.GetHeight() - renderDistance.Y)
            {
                camera.Y = Generator.GetHeight() - renderDistance.Y;
            }
            Terrain[,] terrain = Generator.GetTerrain();
            Rect src = new Rect();
            RectF dst = new RectF();
            for (int x = (int)(camera.X - renderDistance.X / 2), posX = 0; x <= camera.X + renderDistance.X / 2; x++, posX++)
            {
                for (int y = (int)(camera.Y - renderDistance.Y / 2), posY = 0; y <= camera.Y + renderDistance.Y / 2; y++, posY++)
                {
                    int spriteSheetSignleWidth = GameActivity.spriteSheet.GetScaledWidth(canvas) / spriteSheetColoumnCount;
                    src.Left = spriteSheetSignleWidth * (terrain[x, y].Id % spriteSheetColoumnCount);
                    src.Top = spriteSheetSignleWidth * (terrain[x, y].Id / spriteSheetColoumnCount);
                    src.Right = spriteSheetSignleWidth * (terrain[x, y].Id % spriteSheetColoumnCount) + spriteSheetSignleWidth;
                    src.Bottom = spriteSheetSignleWidth * (terrain[x, y].Id / spriteSheetColoumnCount) + spriteSheetSignleWidth;
                    dst.Left = posX * Terrain.Size;
                    dst.Top = posY * Terrain.Size;
                    dst.Right = posX * Terrain.Size + Terrain.Size;
                    dst.Bottom = posY * Terrain.Size + Terrain.Size;
                    canvas.DrawBitmap(GameActivity.spriteSheet, src, dst, null);
                    Item i;
                    if ((i = terrain[x, y].GetItem()) != null)
                    {
                        src.Left = spriteSheetSignleWidth * (i.id % spriteSheetColoumnCount);
                        src.Top = spriteSheetSignleWidth * (i.id / spriteSheetColoumnCount);
                        src.Right = spriteSheetSignleWidth * (i.id % spriteSheetColoumnCount) + spriteSheetSignleWidth;
                        src.Bottom = spriteSheetSignleWidth * (i.id / spriteSheetColoumnCount) + spriteSheetSignleWidth;
                        dst.Left = posX * (int)(Terrain.Size * i.sizePercentage);
                        dst.Top = posY * (int)(Terrain.Size * i.sizePercentage);
                        dst.Right = (posX + 1) * (int)(Terrain.Size * i.sizePercentage);
                        dst.Bottom = (posY + 1) * (int)(Terrain.Size * i.sizePercentage);
                        canvas.DrawBitmap(GameActivity.spriteSheet, src, dst, null);
                    }
                    else if (terrain[x, y].GetStructure() != null)
                    {
                        int id = terrain[x, y].GetStructureId();
                        float SizePercentage = terrain[x, y].GetStructure().SizePercentage;
                        src.Left = spriteSheetSignleWidth * (id % spriteSheetColoumnCount);
                        src.Top = spriteSheetSignleWidth * (id / spriteSheetColoumnCount);
                        src.Right = spriteSheetSignleWidth * (id % spriteSheetColoumnCount) + spriteSheetSignleWidth;
                        src.Bottom = spriteSheetSignleWidth * (id / spriteSheetColoumnCount) + spriteSheetSignleWidth;
                        dst.Left = posX * Terrain.Size;
                        dst.Top = posY * Terrain.Size;
                        dst.Right = posX * Terrain.Size + Terrain.Size;
                        dst.Bottom = posY * Terrain.Size + Terrain.Size;
                        dst.Left = dst.Left + Math.Abs(1 - SizePercentage) * Terrain.Size / 2;
                        dst.Top = dst.Top + Math.Abs(1 - SizePercentage) * Terrain.Size / 2;
                        dst.Right = dst.Right - Math.Abs(1 - SizePercentage) * Terrain.Size / 2;
                        dst.Bottom = dst.Bottom - Math.Abs(1 - SizePercentage) * Terrain.Size / 2;
                        Paint p = null;
                        if (terrain[x, y].GetStructure() is StructureBlueprint)
                        {
                            p = new Paint
                            {
                                Alpha = 100
                            };
                        }
                        canvas.DrawBitmap(GameActivity.spriteSheet, src, dst, p);
                    }
                }
            }
            if (Player.GetX() >= Math.Floor(camera.X - renderDistance.X / 2) && Player.GetX() <= Math.Ceiling(camera.X + renderDistance.X / 2))
            {
                if (Player.GetY() >= Math.Floor(camera.Y - renderDistance.Y / 2) && Player.GetY() <= Math.Ceiling(camera.Y + renderDistance.Y / 2))
                {
                    int spriteSheetSignleWidth = GameActivity.spriteSheet.GetScaledWidth(canvas) / spriteSheetColoumnCount;
                    float PlayerDrawY = Player.GetY() - (int)(camera.Y - renderDistance.Y / 2);
                    float PlayerDrawX = Player.GetX() - (int)(camera.X - renderDistance.X / 2);
                    src.Left = spriteSheetSignleWidth * (Player.Id % spriteSheetColoumnCount);
                    src.Top = spriteSheetSignleWidth * (Player.Id / spriteSheetColoumnCount);
                    src.Right = spriteSheetSignleWidth * (Player.Id % spriteSheetColoumnCount) + spriteSheetSignleWidth;
                    src.Bottom = spriteSheetSignleWidth * (Player.Id / spriteSheetColoumnCount) + spriteSheetSignleWidth;
                    dst.Left = PlayerDrawX * (int)(Terrain.Size);
                    dst.Top = PlayerDrawY * (int)(Terrain.Size);
                    dst.Right = (PlayerDrawX + 1) * (int)(Terrain.Size);
                    dst.Bottom = (PlayerDrawY + 1) * (int)(Terrain.Size);

                    canvas.DrawBitmap(GameActivity.spriteSheet, src, dst, null);
                    
                }
            }
            src.Dispose();
            dst.Dispose();
            Drawn = true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (editMode)
            {
                if (CurrentlyBuilding != null)
                {
                    if (e.Action == MotionEventActions.Down)
                    {
                        int x = (int)e.GetX() / Terrain.Size;
                        int y = (int)e.GetY() / Terrain.Size;
                        if (x < renderDistance.X / 2)
                        {
                            x--;
                        }
                        if (y < renderDistance.Y / 2)
                        {
                            y--;
                        }
                        x = (int)(x - renderDistance.X / 2);
                        y = (int)(y - renderDistance.Y / 2);
                        x = (int)camera.X + x;
                        y = (int)camera.Y + y;
                        if (Generator.TerrainMap[x, y].BuildStructure(CurrentlyBuilding))
                        {
                            CurrentlyBuilding.SetTerrain(Generator.TerrainMap[x, y]);
                            if (Generator.TerrainMap[(int)LastPoint.X, (int)LastPoint.Y].GetStructure() is StructureBlueprint sb && sb == CurrentlyBuilding)
                            {
                                Generator.TerrainMap[(int)LastPoint.X, (int)LastPoint.Y].DestroyStructure(null);
                            }
                            LastPoint.X = x;
                            LastPoint.Y = y;
                            Invalidate();
                        }
                    }
                    return false;
                }
            }
            if (e.Action == MotionEventActions.Down)
            {
                tap = true;
            }
            else if (e.Action == MotionEventActions.Up)
            {
                if (tap)
                {
                    int rowTilesFromCorner = (int)e.GetX() / Terrain.Size;
                    int coloumnTilesFromCorner = (int)e.GetY() / Terrain.Size;
                    int newX = (int)((camera.X - renderDistance.X / 2) + rowTilesFromCorner);
                    int newY = (int)((camera.Y - renderDistance.Y / 2) + coloumnTilesFromCorner);
                    Player.MoveTo(newX, newY);
                    Invalidate();
                }

            }
            else if (e.Action == MotionEventActions.Move)
            {

                tap = false;
                if (e.HistorySize > 1)
                {
                    float historyx = e.GetHistoricalX(0, 0);
                    float x = e.GetX();
                    float historyy = e.GetHistoricalY(0, 0);
                    float y = e.GetY();
                    camera.X += (historyx - x) / Terrain.Size;
                    camera.Y += (historyy - y) / Terrain.Size;
                }
                CorrectCamera();
                Invalidate();
            }
            ((GameActivity)context).OnTouchEvent(e);
            return true;
        }

        private void CorrectCamera()
        {
            if (camera.X < renderDistance.X / 2)
            {
                camera.X = renderDistance.X / 2;
            }
            else if (camera.X + renderDistance.X / 2 >= Generator.GetTerrain().GetLength(0))
            {
                camera.X = Generator.GetTerrain().GetLength(0) - renderDistance.X - 1;
            }
            if (camera.Y < renderDistance.Y / 2)
            {
                camera.Y = renderDistance.Y / 2;
            }
            else if (camera.Y + renderDistance.Y / 2 >= Generator.GetTerrain().GetLength(1))
            {
                camera.Y = Generator.GetTerrain().GetLength(1) - renderDistance.Y - 1;
            }
        }

        public Bitmap DisplayMap(int width, int height)
        {
            float scale = Math.Min((float)width / Generator.GetWidth(), (float)height / Generator.GetHeight());
            return Generator.GenerateMap(scale, (GameActivity)context);
        }

        public bool IsDrawn()
        {
            return Drawn;
        }

        public void GenerateItems()
        {
            Terrain[,] terrain = Generator.GetTerrain();
            Random rng = new Random();
            for (int x = 0; x < Generator.GetWidth(); x++)
            {
                for (int y = 0; y < Generator.GetHeight(); y++)
                {
                    if (terrain[x, y].GetStructure() == null && terrain[x, y].GetItem() == null)
                    {
                        for (int i = 0; i < itemTypeList.Count && terrain[x, y].GetItem() == null; i++)
                        {
                            bool isSpawnable = false;
                            for (int n = 0; n < itemTypeList[i].spawnableTerrain.Length; n++)
                            {
                                if (terrain[x, y].Type.Equals(itemTypeList[i].spawnableTerrain[n]))
                                {
                                    isSpawnable = true;
                                    break;
                                }
                            }
                            if (isSpawnable && rng.NextDouble() < itemTypeList[i].spawnChance)
                            {
                                terrain[x, y].AddItem(new Item(itemTypeList[i].name, itemTypeList[i].id, itemTypeList[i].sizePercentage));
                            }
                        }
                        for (int i = 0; i < structureTypeList.Count && terrain[x, y].GetItem() == null; i++)
                        {
                            bool isSpawnable = false;
                            for (int n = 0; n < structureTypeList[i].SpawnableTerrain.Length; n++)
                            {
                                if (terrain[x, y].Type.Equals(structureTypeList[i].SpawnableTerrain[n]))
                                {
                                    isSpawnable = true;
                                    break;
                                }
                            }
                            if (isSpawnable && rng.NextDouble() < structureTypeList[i].SpawnChance)
                            {
                                Structure build;
                                if (i == (int)StructureTypes.TREE)
                                {
                                    build = new Plant(1.0f / 60 / 10, 2, structureTypeList[i], false, 2);
                                }
                                else
                                {
                                    build = new Structure(structureTypeList[i]);
                                }
                                terrain[x, y].BuildStructure(build);
                            }
                        }
                    }
                }
            }
        }

        public bool DropItem(int index)
        {
            if (Generator.TerrainMap[(int)Player.GetX(), (int)Player.GetY()].GetItem() == null)
            {
                Item dropped = Player.DropItem(index);
                Generator.TerrainMap[(int)Player.GetX(), (int)Player.GetY()].AddItem(dropped);
                return true;
            }
            return false;
        }

        public void Save()
        {
            try
            {
                using Stream stream = context.OpenFileOutput("world.txt", Android.Content.FileCreationMode.Private);
                try
                {
                    Byte[] generation = Generator.GetBytes();
                    stream.Write(BitConverter.GetBytes(generation.Length));
                    stream.Write(generation);
                    Byte[] Player = this.Player.ToByte();
                    stream.Write(BitConverter.GetBytes(Player.Length));
                    stream.Write(Player);
                    stream.Close();
                }
                catch (Java.IO.IOException e)
                {
                    e.PrintStackTrace();
                }
            }
            catch (Java.IO.FileNotFoundException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}