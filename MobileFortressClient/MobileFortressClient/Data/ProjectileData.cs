using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;

namespace MobileFortressClient.Data
{
    abstract class ProjectileData
    {
        public static ProjectileData[] ProjectileTable = new ProjectileData[]
        {
        new BulletData(0,0.5f, 100, 2, 25),
        new BulletData(0,0.7f, 80, 2.4f, 40),
        new BulletData(0,1.4f, 75, 3.8f, 75),
        new RocketData(modelID: 0, hitboxRadius: 0.5f, explosionSize: 5, muzzleVel: 50, spread: 2, power: 60,
            fuel: 3, motorAccel: 5, lifetime: 6),
        new RocketData(modelID: 0, hitboxRadius: 0.1f, explosionSize: 6, muzzleVel: 50, spread: 1, power: 170,
            fuel: 6, motorAccel: 8, lifetime: 10, homingRadius: 45, maxRange: 350),
        new RocketData(modelID: 1, hitboxRadius: 0.5f, explosionSize: 30, muzzleVel: 20, spread: 6, power: 500,
            fuel: 10, motorAccel: 1, lifetime: 15)
        };

        public static Random pRandom = new Random();

        public abstract void Create(Vector3 position, Quaternion orientation, Vector3 velocity);
        public abstract void Create(Vector3 position, Quaternion orientation, Vector3 velocity, Entity target);
        public abstract ProjectileData Copy();
    }
}
