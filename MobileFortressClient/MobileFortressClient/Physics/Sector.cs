using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using Microsoft.Xna.Framework;
using MobileFortressClient.Managers;
using MobileFortressClient.Ships;
using MobileFortressClient.Particles;
using MobileFortressClient.MobileObjects;

namespace MobileFortressClient.Physics
{
    class Sector //Mostly used to hold space.
    {
        public static Sector Redria = new Sector();

        public Space Space { get; private set; }

        public TerrainManager Terrain { get; private set; }
        public EffectManager ClientObjects { get; private set; }
        public ShipManager Ships { get; private set; }
        public MobileObjectManager Objects { get; private set; }

        public void Initialize()
        {
            Space = new Space();
            Space.ForceUpdater.Gravity = new Vector3(0, -9.8f, 0);
            Terrain = new TerrainManager();
            ClientObjects = new EffectManager();
            Ships = new ShipManager();
            Objects = new MobileObjectManager();
        }

        public void Update(float dt)
        {
            ClientObjects.Process(dt);
            Ships.Process(dt);
            Terrain.Process(dt);
            Objects.Process(dt);
            Space.Update(dt);
        }

        public void Add(PhysicsObj obj)
        {
            Space.Add(obj.Entity);
            if (obj is ShipObj)
                Ships.Add((ShipObj)obj);
            else if (obj is MobileObj)
                Objects.Add((MobileObj)obj);
            else
                Terrain.Add(obj);
        }
        public void Add(Particle particle)
        {
            ClientObjects.Add(particle);
        }
    }
}
