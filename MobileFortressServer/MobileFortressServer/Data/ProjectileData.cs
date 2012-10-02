using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using MobileFortressServer.Physics;

namespace MobileFortressServer.Data
{
    abstract class ProjectileData
    {
        public static ProjectileData[] ProjectileTable = new ProjectileData[]
        {
        new BulletData(sprite: 0, size: 0.5f, velocity: 150, spread: 3f, power: 25),
        new BulletData(sprite: 0, size: 0.7f, velocity: 140, spread: 2f, power: 40),
        new BulletData(sprite: 0, size: 1f, velocity: 120, spread: 1.5f, power: 100),
        new RocketData(modelID: 21, hitboxRadius: 0.5f, explosionSize: 4, muzzleVel: 75, spread: 3, power: 75,
            fuel: 8, motorAccel: 0, lifetime: 12),
        new RocketData(modelID: 21, hitboxRadius: 0.5f, explosionSize: 6, muzzleVel: 60, spread: 1, power: 170,
            fuel: 10, motorAccel: 0, lifetime: 15, homingRadius: 45, maxRange: 350),
        new RocketData(modelID: 22, hitboxRadius: 0.5f, explosionSize: 20, muzzleVel: 0, spread: 6, power: 500,
            fuel: 0, motorAccel: 0, lifetime: 25)
        };

        public static Random pRandom = new Random();

        public float ForwardOffset = 1f;

        public abstract void Create(Vector3 position, Quaternion orientation, Vector3 inputVelocity, out Vector3 velocity);
        public abstract void Create(Vector3 position, Quaternion orientation, Vector3 inputVelocity, out Vector3 velocity, PhysicsObj target);
        public abstract ProjectileData Copy();
    }
}
