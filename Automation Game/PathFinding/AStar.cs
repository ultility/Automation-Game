using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Automation_Game.PathFinding
{
    class AStar
    {
        public static List<Point> GetPath(int StartX, int StartY, int TargetX, int TargetY, Terrain[,] TerrainMap)
        {
            List<Point> path = new List<Point>();
            var start = new Tile();
            start.Y = StartY;
            start.X = StartX;


            var finish = new Tile();
            finish.Y = TargetY;
            finish.X = TargetX;

            start.SetDistance(finish.X, finish.Y);

            var activeTiles = new List<Tile>();
            activeTiles.Add(start);
            var visitedTiles = new List<Tile>();

            while (activeTiles.Any())
            {
                var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                if (checkTile.X == finish.X && checkTile.Y == finish.Y)
                {
                    //We found the destination and we can be sure (Because the the OrderBy above)
                    //That it's the most low cost option. 
                    var tile = checkTile;
                    Console.WriteLine("Retracing steps backwards...");
                    while (true)
                    {
                        if (Terrain.IsWalkable(TerrainMap[tile.X,tile.Y]))
                        {
                            path.Add(new Point(tile.X, tile.Y));
                        }
                        tile = tile.Parent;
                        if (tile == null)
                        {
                            path.Reverse();
                            return path;
                        }
                    }
                }

                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                var walkableTiles = GetWalkableTiles(TerrainMap, checkTile, finish);

                foreach (var walkableTile in walkableTiles)
                {
                    //We have already visited this tile so we don't need to do so again!
                    if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                        continue;

                    //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                    if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    {
                        var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                        if (existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }
                    else
                    {
                        //We've never seen this tile before so add it to the list. 
                        activeTiles.Add(walkableTile);
                    }
                }
            }

            Console.WriteLine("No Path Found!");
            return path;
        }

        private static List<Tile> GetWalkableTiles(Terrain[,] terrains, Tile currentTile, Tile targetTile)
        {
            var possibleTiles = new List<Tile>()
            {
                new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
                new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

            possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

            var maxX = terrains.GetLength(0);
            var maxY = terrains.GetLength(1);

            var end = possibleTiles.Where(tile => tile.X >= 0 && tile.X <= maxX);
            var endView = end.ToList();
            end = end.Where(tile => tile.Y >= 0 && tile.Y <= maxY);
            endView = end.ToList();
            end = end.Where(tile => Terrain.IsWalkable(terrains[tile.X,tile.Y]) || (tile.X == targetTile.X &&  tile.Y == targetTile.Y));
            endView = end.ToList();
            return endView;
        }

    }
}