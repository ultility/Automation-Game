using Android.Content;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Automation_Game.PathFinding;

namespace Automation_Game.Map
{
    public class MapGenerator
    {
        readonly int mapWidth;
        readonly int mapHeight;
        readonly int seed;
        readonly int octaves;
        readonly float scale;
        readonly float persistence;
        readonly float lacunarity;
        Vector2 offset;
        public Terrain[,] TerrainMap { get; }



        const float DEFAULT_SCALE = 0.4f;
        const int DEFAULT_OCTAVES = 5;
        const float DEFAULT_PERSISTENCE = 0.9f;
        const float DEFAULT_LACUNARITY = 1.5f;

        public MapGenerator(int mapWidth, int mapHeight, GameActivity activity)
        {
            Random rng = new Random();
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            scale = DEFAULT_SCALE;
            seed = rng.Next(1000000);
            octaves = DEFAULT_OCTAVES;
            persistence = DEFAULT_PERSISTENCE;
            lacunarity = DEFAULT_LACUNARITY;
            offset = new Vector2(153.2f, 12);
            TerrainMap = new Terrain[mapWidth, mapHeight];

            GenerateMap(activity);

        }

        public MapGenerator(int mapWidth, int mapHeight, int seed, GameActivity activity)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            scale = .4f;
            this.seed = seed;
            octaves = 5;
            persistence = .9f;
            lacunarity = 1.5f;
            offset = new Vector2(153.2f, 12);
            TerrainMap = new Terrain[mapWidth, mapHeight];
            GenerateMap(activity);
        }

        public Bitmap GenerateMap(GameActivity activity)
        {
            return GenerateMap(1, activity);
        }

        public Bitmap GenerateMap(float scale, GameActivity activity)
        {

            int mapWidth = (int)(this.mapWidth * scale);
            int mapHeight = (int)(this.mapHeight * scale);
            scale *= this.scale;
            float[,] mapNoise = NoiseMaker.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, seed, offset);
            Bitmap map = Bitmap.CreateBitmap(mapWidth, mapHeight, Bitmap.Config.Argb8888);
            Color c;
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (mapNoise[x, y] < .30)
                    {
                        c = Color.Blue;
                        if (scale == DEFAULT_SCALE)
                        {
                            TerrainMap[x, y] = new Terrain("water", 2, x, y);
                        }
                    }
                    else if (mapNoise[x, y] < .50)
                    {
                        c = Color.SandyBrown;
                        if (scale == DEFAULT_SCALE)
                        {
                            TerrainMap[x, y] = new Terrain("sand", 3, x, y);
                        }
                    }
                    else if (mapNoise[x, y] < .70)
                    {
                        c = Color.SaddleBrown;
                        if (scale == DEFAULT_SCALE)
                        {
                            TerrainMap[x, y] = new Terrain("dirt", 0, x, y);
                            if (mapNoise[x, y] > .63 && mapNoise[x, y] < .64)
                            {
                                TerrainMap[x, y].BuildStructure(new Structure("Rock", 13, .85f, new Tool("Pickaxe", 11, 0), new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].name, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].id, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STONE].sizePercentage), 2));
                            }   
                        }
                    }
                    else
                    {
                        c = Color.ForestGreen;
                        if (scale == DEFAULT_SCALE)
                        {
                            TerrainMap[x, y] = new Terrain("grass", 1, x, y);
                        }
                    }
                    if (scale == DEFAULT_SCALE)
                    {
                        TerrainMap[x, y].SetActivity(activity);
                    }
                    map.SetPixel(x, y, c);
                }
            }
            if (scale == DEFAULT_SCALE)
            {
                bool ImpossibleMap = true;
                int CenterX = mapWidth / 2;
                int CenterY = mapHeight / 2;
                for (int i = -2; i < 2 && ImpossibleMap; i++)
                {
                    if (AStar.GetPath(CenterX, CenterY, CenterX + i, CenterY, TerrainMap).Count > 0)
                    {
                        ImpossibleMap = false;
                    }
                }
                for (int j = -2; j < 2 && ImpossibleMap; j++)
                {
                    if (AStar.GetPath(CenterX, CenterY, CenterX, CenterY + j, TerrainMap).Count > 0)
                    {
                        ImpossibleMap = false;
                    }
                }
                if (ImpossibleMap)
                {
                    return GenerateMap(activity);
                }
            }
            return map;
        }


        public Terrain[,] GetTerrain()
        {
            return TerrainMap;
        }

        public int GetWidth()
        {
            return mapWidth;
        }

        public int GetHeight()
        {
            return mapHeight;
        }

        public float GetScale()
        {
            return scale;
        }

        public Byte[] GetBytes()
        {
            List<Byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(mapWidth));
            bytes.AddRange(BitConverter.GetBytes(mapHeight));
            bytes.AddRange(BitConverter.GetBytes(seed));
            bytes.AddRange(BitConverter.GetBytes(octaves));
            bytes.AddRange(BitConverter.GetBytes(scale));
            bytes.AddRange(BitConverter.GetBytes(persistence));
            bytes.AddRange(BitConverter.GetBytes(lacunarity));
            bytes.AddRange(BitConverter.GetBytes(offset.X));
            bytes.AddRange(BitConverter.GetBytes(offset.Y));
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    if (TerrainMap[x, y].GetItem() != null)
                    {
                        bytes.AddRange(BitConverter.GetBytes(TerrainMap[x, y].GetItem().ToByte().Length));
                        bytes.AddRange(TerrainMap[x, y].GetItem().ToByte());
                    }
                    else
                    {
                        bytes.AddRange(BitConverter.GetBytes(0));
                        if (TerrainMap[x, y].GetStructure() != null)
                        {
                            bytes.AddRange(TerrainMap[x, y].GetStructure().ToByte());
                        }
                        else
                        {
                            bytes.AddRange(BitConverter.GetBytes(0));
                        }
                    }

                }
            }
            return bytes.ToArray();
        }

        public void SetItemPointer(int x, int y, Item item)
        {
            TerrainMap[x, y].AddItem(item);
        }

        public MapGenerator(Byte[] bytes, Context context)
        {
            int offset = 0;
            mapWidth = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(int);
            mapHeight = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(int);
            seed = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(int);
            octaves = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(int);
            scale = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(float);
            persistence = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(float);
            lacunarity = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(float);
            this.offset = new Vector2
            {
                X = BitConverter.ToSingle(bytes, offset)
            };
            offset += sizeof(float);
            this.offset.Y = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(float);
            TerrainMap = new Terrain[mapWidth, mapHeight];
            GenerateMap((GameActivity)context);
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    int length = BitConverter.ToInt32(bytes, offset);
                    offset += 4;
                    if (length > 0)
                    {
                        Byte[] temp = bytes.ToList<Byte>().GetRange(offset, length).ToArray();
                        if (Tool.IsTool(temp))
                        {
                            TerrainMap[x, y].AddItem(new Tool(temp));
                        }
                        else
                        {
                            TerrainMap[x, y].AddItem(new Item(temp));
                        }
                        offset += length;
                    }
                    else
                    {
                        length = BitConverter.ToInt32(bytes, offset);
                        offset += 4;
                        if (length > 0)
                        {
                            Structure structure;
                            string name = Encoding.Default.GetString(bytes, offset, length);
                            Delivery[] deliveries = null;
                            bool isblueprint = false;
                            if (name.Contains("blueprint"))
                            {
                                isblueprint = true;
                                name = name.Replace("blueprint", "");
                            }
                            switch (name)
                            {
                                case "CraftingStation":
                                    structure = new CraftingStation(context);
                                    deliveries = new Delivery[1];
                                    deliveries[0] = new Delivery(new Item(MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].name, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].id, MapDraw.itemTypeList[(int)MapDraw.ItemTypes.STICK].sizePercentage), 1);
                                    break;
                                case "tree":
                                    StructureType st = MapDraw.structureTypeList[(int)MapDraw.StructureTypes.TREE];
                                    structure = new Structure(st.Name, st.Id, st.SizePercentage, st.UseableItem, st.DropItem, st.Hardness);
                                    break;
                                default:
                                    structure = null;
                                    break;
                            }
                            if (isblueprint)
                            {
                                StructureBlueprint blueprint = new StructureBlueprint(structure, deliveries, TerrainMap[x, y]);
                                TerrainMap[x, y].BuildStructure(blueprint);
                            }
                            else
                            {
                                TerrainMap[x, y].BuildStructure(structure);
                            }
                            offset += length;
                        }
                    }

                }
            }
        }
    }
}