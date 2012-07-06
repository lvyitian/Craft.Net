using System;
using Craft.Net.Server.Worlds.Generation;
using Craft.Net.Server.Blocks;
using System.Collections.Generic;
using Craft.Net.Server.Events;

namespace Craft.Net.Server.Worlds
{
    public class World
    {
        public string LevelType
        {
            get
            {
                return WorldGenerator.LevelType;
            }
        }

        public Dictionary<Vector3, Region> Regions;
        public EntityManager EntityManager;
        public string Name;
        public Vector3 SpawnPoint;
        public GameMode GameMode;
        public Difficulty Difficulty;
        public IWorldGenerator WorldGenerator;
        public long Seed;

        public event EventHandler<BlockChangedEventArgs> OnBlockChanged;

        public World(IWorldGenerator WorldGenerator)
        {
            EntityManager = new EntityManager(this);
            Name = "world";
            GameMode = GameMode.Creative;
            Difficulty = Difficulty.Peaceful;
            this.WorldGenerator = WorldGenerator;
            SpawnPoint = WorldGenerator.SpawnPoint;
            Seed = MinecraftServer.Random.Next();
            Regions = new Dictionary<Vector3, Region>();
        }

        public World(IWorldGenerator WorldGenerator, long Seed) : this(WorldGenerator)
        {
            this.Seed = Seed;
        }

        public Chunk GetChunk(Vector3 position)
        {
            // TODO: I really hate this code
            int X = (int)position.X;
            int Z = (int)position.Z;
            bool negX = X < 0;
            bool negZ = Z < 0;
            X = negX ? -X : X;
            Z = negZ ? -Z : Z;
            // abs(n)/width+1
            X = (X / Region.Width) + (negX ? 1 : 0);
            Z = (Z / Region.Depth) + (negZ ? 1 : 0);
            X = negX ? -X : X;
            Z = negZ ? -Z : Z;
            Vector3 region = new Vector3(X, 0, Z);
            if (!Regions.ContainsKey(region))
                Regions.Add(region, new Region(region, WorldGenerator));
            // Create relative coordinates
            X = (int)position.X;
            Z = (int)position.Z;
            X = negX ? -X : X;
            Z = negZ ? -Z : Z;
            X = X % Region.Width;
            Z = Z % Region.Depth;
            X = negX ? Region.Width - X : X;
            Z = negZ ? Region.Depth - Z : Z;
            return Regions[region].GetChunk(new Vector3(X, 0, Z));
        }

        public Block GetBlock(Vector3 position)
        {
            int X = (int)position.X;
            int Z = (int)position.Z;
            bool negX = X < 0;
            bool negZ = Z < 0;
            X = negX ? -X : X;
            Z = negZ ? -Z : Z;
            // abs(n)/width+1
            X = (X / Region.Width) + (negX ? 1 : 0);
            Z = (Z / Region.Depth) + (negZ ? 1 : 0);
            X = negX ? -X : X;
            Z = negZ ? -Z : Z;
            Vector3 region = new Vector3(X, 0, Z);
            if (!Regions.ContainsKey(region))
                Regions.Add(region, new Region(region, WorldGenerator));
            
            Vector3 relativePosition = position;
            if (negX)
                relativePosition.X = -relativePosition.X;
            if (negZ)
                relativePosition.Z = -relativePosition.Z;
            relativePosition.X = (int)(relativePosition.X) % Region.Width;
            relativePosition.Z = (int)(relativePosition.Z) % Region.Depth;
            if (negX)
                relativePosition.X = (Region.Width * Chunk.Width) - relativePosition.X;
            if (negZ)
                relativePosition.Z = (Region.Depth * Chunk.Depth) - relativePosition.Z;

            return Regions[region].GetBlock(relativePosition);
        }

        public void SetBlock(Vector3 position, Block value)
        {
            int X = (int)position.X;
            int Z = (int)position.Z;
            bool negX = X < 0;
            bool negZ = Z < 0;
            X = negX ? -X : X;
            Z = negZ ? -Z : Z;
            // abs(n)/width+1
            X = (X / Region.Width) + (negX ? 1 : 0);
            Z = (Z / Region.Depth) + (negZ ? 1 : 0);
            X = negX ? -X : X;
            Z = negZ ? -Z : Z;
            Vector3 region = new Vector3(X, 0, Z);
            if (!Regions.ContainsKey(region))
                Regions.Add(region, new Region(region, WorldGenerator));
            
            Vector3 relativePosition = position;
            if (negX)
                relativePosition.X = -relativePosition.X;
            if (negZ)
                relativePosition.Z = -relativePosition.Z;
            relativePosition.X = (int)(relativePosition.X) % Region.Width;
            relativePosition.Z = (int)(relativePosition.Z) % Region.Depth;
            if (negX)
                relativePosition.X = (Region.Width * Chunk.Width) - relativePosition.X;
            if (negZ)
                relativePosition.Z = (Region.Depth * Chunk.Depth) - relativePosition.Z;
            
            Regions[region].SetBlock(relativePosition, value);

            if (OnBlockChanged != null)
                OnBlockChanged(this, new BlockChangedEventArgs(this, position, value));
        }
    }
}
