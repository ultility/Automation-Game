using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Numerics;

namespace Automation_Game.Map
{
    public class MapGenerator
    {
        int mapWidth;
        int mapHeight;
        int seed;
        int octaves;
        float scale;
        float persistence;
        float lacunarity;
        Vector2 offset;
        public Terrain[,] terrainMap { get; }
        Bitmap map;



        const float DEFAULT_SCALE = 0.4f;
        const int DEFAULT_OCTAVES = 5;
        const float DEFAULT_PERSISTENCE = 0.9f;
        const float DEFAULT_LACUNARITY = 1.5f;
        public MapGenerator(int mapWidth, int mapHeight)
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
            terrainMap = new Terrain[mapWidth, mapHeight];

            generateMap();

        }

        public MapGenerator(int mapWidth, int mapHeight, int seed)
        {
            Random rng = new Random();
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            scale = .4f;
            this.seed = seed;
            octaves = 5;
            persistence = .9f;
            lacunarity = 1.5f;
            offset = new Vector2(153.2f, 12);
            terrainMap = new Terrain[mapWidth, mapHeight];
            map = generateMap();
        }

        public Bitmap generateMap()
        {
            return generateMap(1);
        }

        public Bitmap generateMap(float scale)
        {

            int mapWidth = (int)(this.mapWidth * scale);
            int mapHeight = (int)(this.mapHeight * scale);
            scale = scale * this.scale;
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
                            terrainMap[x, y] = new Terrain("water", 2, x, y);
                        }
                    }
                    else if (mapNoise[x, y] < .50)
                    {
                        c = Color.SandyBrown;
                        if (scale == DEFAULT_SCALE)
                        {
                            terrainMap[x, y] = new Terrain("sand", 3, x, y);
                        }
                    }
                    else if (mapNoise[x, y] < .70)
                    {
                        c = Color.SaddleBrown;
                        if (scale == DEFAULT_SCALE)
                        {
                            terrainMap[x, y] = new Terrain("dirt", 0, x, y);
                        }
                    }
                    else
                    {
                        c = Color.ForestGreen;
                        if (scale == DEFAULT_SCALE)
                        {
                            terrainMap[x, y] = new Terrain("grass", 1, x, y);
                        }
                    }
                    map.SetPixel(x, y, c);
                }
            }
            return map;
        }

        public Bitmap GetMap()
        {
            return map;
        }

        public Terrain[,] GetTerrain()
        {
            return terrainMap;
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
                    if (terrainMap[x, y].GetItem() != null)
                    {
                        bytes.AddRange(BitConverter.GetBytes(terrainMap[x, y].GetItem().ToByte().Length));
                        bytes.AddRange(terrainMap[x, y].GetItem().ToByte());
                    }
                    else
                    {
                        bytes.AddRange(BitConverter.GetBytes(0));
                        if (terrainMap[x, y].GetStructure() != null)
                        {
                            bytes.AddRange(terrainMap[x, y].GetStructure().ToByte());
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

        public void SetItemPointer(Item item)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    terrainMap[x, y].SetItem(item);
                }
            }
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
            this.offset = new Vector2();
            this.offset.X = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(float);
            this.offset.Y = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(float);
            terrainMap = new Terrain[mapWidth, mapHeight];
            generateMap();
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    int length = BitConverter.ToInt32(bytes, offset);
                    offset += 4;
                    if (length > 0)
                    {
                        terrainMap[x, y].SetItem(new Item(bytes.ToList<Byte>().GetRange(offset, length).ToArray()));
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
                            switch (name)
                            {
                                case "CraftingStation":
                                    structure = new CraftingStation(context);
                                    break;
                                default:
                                    structure = null;
                                    break;
                            }
                            terrainMap[x, y].BuildStructure(structure);
                            offset += length;
                        }
                    }

                }
            }
        }
    }
}