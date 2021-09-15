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
                            terrainMap[x, y] = new Terrain("water", 2);
                        }
                    }
                    else if (mapNoise[x, y] < .50)
                    {
                        c = Color.SandyBrown;
                        if (scale == DEFAULT_SCALE)
                        {
                            terrainMap[x, y] = new Terrain("sand", 3);
                        }
                    }
                    else if (mapNoise[x, y] < .70)
                    {
                        c = Color.SaddleBrown;
                        if (scale == DEFAULT_SCALE)
                        {
                            terrainMap[x, y] = new Terrain("dirt", 0);
                        }
                    }
                    else
                    {
                        c = Color.ForestGreen;
                        if (scale == DEFAULT_SCALE)
                        {
                            terrainMap[x, y] = new Terrain("grass", 1);
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
            return bytes.ToArray();
        }

        public MapGenerator( Byte[] bytes)
        {
            mapWidth = BitConverter.ToInt32(bytes, 0);
            mapHeight = BitConverter.ToInt32(bytes, 4);
            seed = BitConverter.ToInt32(bytes, 8);
            octaves = BitConverter.ToInt32(bytes, 12);
            scale = BitConverter.ToSingle(bytes, 16);
            persistence = BitConverter.ToSingle(bytes, 20);
            lacunarity = BitConverter.ToSingle(bytes, 24);
            offset = new Vector2(BitConverter.ToSingle(bytes, 28), BitConverter.ToSingle(bytes, 32));
            terrainMap = new Terrain[mapWidth, mapHeight];
            
            generateMap();

        }
    }
}