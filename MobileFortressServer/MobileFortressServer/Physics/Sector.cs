using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using MobileFortressServer.Ships;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using MobileFortressServer.Managers;
using MobileFortressServer.Messages;

namespace MobileFortressServer.Physics
{
    class Sector //Mostly used to hold space.
    {
        public static Sector Redria = new Sector();

        public Space Space { get; private set; }

        public TerrainManager Terrain { get; private set; }
        public BulletManager Bullets { get; private set; }
        public ShipManager Ships { get; private set; }
        public MobileObjectManager Objects { get; private set; }

        public void Initialize()
        {
            Space = new Space();
            Space.ForceUpdater.Gravity = new Vector3(0, -9.8f, 0);
            Terrain = new TerrainManager();
            Terrain.Initialize();
            Bullets = new BulletManager();
            Ships = new ShipManager();
            Objects = new MobileObjectManager();
        }

        public void Update(float dt)
        {
            Bullets.Process(dt);
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
            else if (obj is Rocket)
            {
                Objects.Add(obj);
                MessageWriter.ClientEntityCreationMessage(null, NetEntityType.Missile, obj.ID, obj.Position, obj.Orientation, obj.resource_index);
            }
            else if (obj is Character)
            {

            }
            else
                Terrain.Add(obj);
        }
        public void Add(Bullet particle)
        {
            Bullets.Add(particle);
        }

        public void SendUpdate(NetConnection connection)
        {
            Ships.SendUpdate(connection);
            Objects.SendUpdate(connection);
        }
    }
}
