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
        float[,] mapNoise;
        Vector2 offset;
        public Terrain[,] terrainMap { get; }
        Bitmap map;

        Context context;
        public MapGenerator(int mapWidth, int mapHeight, Context context)
        {
            Random rng = new Random();
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            scale = .4f;
            seed = rng.Next(1000000);
            octaves = 5;
            persistence = .9f;
            lacunarity = 1.5f;
            offset = new Vector2(153.2f, 12);
            terrainMap = new Terrain[mapWidth, mapHeight];
            this.context = context;
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
            mapWidth = (int)(mapWidth * scale);
            mapHeight = (int)(mapHeight * scale);
            scale = scale * this.scale;
            mapNoise = NoiseMaker.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, seed, offset);
            Bitmap map = Bitmap.CreateBitmap(mapWidth, mapHeight, Bitmap.Config.Argb8888);
            Color c;
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (mapNoise[x, y] < .30)
                    {
                        c = Color.Blue;
                        terrainMap[x, y] = new Terrain("water", 2);
                    }
                    else if (mapNoise[x, y] < .50)
                    {
                        c = Color.SandyBrown;
                        terrainMap[x, y] = new Terrain("sand", 3);
                    }
                    else if (mapNoise[x, y] < .70)
                    {
                        c = Color.SaddleBrown;
                        terrainMap[x, y] = new Terrain("dirt", 0);
                    }
                    else
                    {
                        c = Color.ForestGreen;
                        terrainMap[x, y] = new Terrain("grass", 1);
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
    }
}