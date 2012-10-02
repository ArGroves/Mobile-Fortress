using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressServer.Data.Heightmaps;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;
using BEPUphysics.Collidables;
using MobileFortressServer.Physics;

namespace MobileFortressServer.Data
{
    class Heightmap
    {
        const float SeaLevel = 0.0f;
        const float lateralScale = 40;
        const float verticalScale = 32;
        float[,] map;
        int mapsize;
        public int Power;
        public Random Generator;
        public Terrain Terrain;

        public int Seed { get; private set; }
        public float Roughness { get; private set; }

        List<DSSquare> squares = new List<DSSquare>(1);

        public float[,] Map
        {
            get
            {
                return map;
            }
        }

        public Heightmap(int seed, int power, float roughness)
        {
            Power = power;
            Generator = new Random(seed);
            mapsize = (int)Math.Pow(2,power) + 1;
            Roughness = roughness;
            map = new float[mapsize,mapsize];
            GenerateMap();
        }

        void GenerateMap()
        {
            var mapsize = map.GetLength(0) - 1;
            map[0, 0] = SeaLevel;
            map[mapsize, 0] = SeaLevel;
            map[0, mapsize] = SeaLevel;
            map[mapsize, mapsize] = SeaLevel;
            squares.Add(new DSSquare(
                heightmap: this,
                nw: new Point(0,0),
                ne: new Point(mapsize, 0),
                sw: new Point(0, mapsize),
                se: new Point(mapsize, mapsize),
                displacement: Roughness
                ));
            float squareSize = mapsize;
            var newSquares = new List<DSSquare>(4);
            while (squareSize > 1)
            {
                foreach (DSSquare square in squares)
                {
                    square.SetMid(ref map);
                }
                foreach (DSSquare square in squares)
                {
                    square.SetEdges(ref map);
                }
                foreach (DSSquare square in squares)
                {
                    newSquares.AddRange(square.Subdivide());
                }
                squareSize /= 2;
                squares = newSquares;
                newSquares = new List<DSSquare>(4);
            }

            CreateTerrain();

            //Final map modifiers
            //Now handled by additive sea level.
            /*for (int x = 0; x <= mapsize; x++)
                for (int y = 0; y <= mapsize; y++)
                {
                    map[x, y] = MathHelper.Max(map[x, y], SeaLevel); //Clamp to sea level.
                }
             */
        }

        public void FlattenArea(Rectangle area)
        {
            float flat = SeaLevel;
            int x, y;
            for (x = area.X; x <= area.X+area.Width; x++)
                for (y = area.Y; y <= area.Y+area.Height; y++)
                    flat = Math.Max(flat, map[x, y]);
            for (x = area.X; x <= area.X+area.Width; x++)
                for (y = area.Y; y <= area.Y+area.Height; y++)
                    map[x, y] = flat;
        }

        public float GetRandomFloat()
        {
            return (float)Generator.NextDouble() - 0.5f;
        }

        public void DebugPrint()
        {
            Console.WriteLine("Heightmap:");
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    Console.Write(map[x, y] + " ");
                }
                Console.WriteLine();
            }
        }

        void CreateTerrain()
        {

            float peakValue = 0;

            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(0); y++)
                    if (map[x, y] > peakValue) peakValue = map[x, y];

            float half = (map.GetLength(0) * lateralScale) / 2;

            AffineTransform transform = new AffineTransform(
                new Vector3(lateralScale, verticalScale, -lateralScale),
                Quaternion.Identity,
                new Vector3(-half, 0, half));

            Terrain = new Terrain(map, transform);
            Sector.Redria.Space.Add(Terrain);
        }
    }
}
