﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.Data
{
    class RocketData : ProjectileData
    {
        public ushort ModelID;
        public float HitboxRadius;
        public float ExplosionSize;
        public float MuzzleVel;
        public float BulletSpread;
        public float Power;

        public bool IsHoming;

        public float HomingRadius;
        public float MaxRange;

        public float Fuel;
        public float MotorAccel;
        public float Lifetime;

        public RocketData(ushort modelID, float hitboxRadius, float explosionSize,
            float muzzleVel, float spread, float power, float fuel, float motorAccel, float lifetime)
        {
            Initialize(modelID, hitboxRadius, explosionSize, muzzleVel, spread, power, fuel, motorAccel, lifetime);
        }

        public RocketData(ushort modelID, float hitboxRadius, float explosionSize,
            float muzzleVel, float spread, float power, float fuel, float motorAccel, float lifetime,
            float homingRadius, float maxRange)
        {
            Initialize(modelID, hitboxRadius, explosionSize, muzzleVel, spread, power, fuel, motorAccel, lifetime);
            IsHoming = true;
            HomingRadius = homingRadius;
            MaxRange = maxRange;
        }

        public void Initialize(ushort modelID, float hitboxRadius, float explosionSize,
            float muzzleVel, float spread, float power, float fuel, float motorAccel, float lifetime)
        {
            ModelID = modelID; HitboxRadius = hitboxRadius; ExplosionSize = explosionSize;
            MuzzleVel = muzzleVel; BulletSpread = spread; Power = power; Fuel = fuel; Lifetime = lifetime;
            MotorAccel = motorAccel;
        }


        public override ProjectileData Copy()
        {
            if (IsHoming)
                return new RocketData(ModelID, HitboxRadius, ExplosionSize, MuzzleVel, BulletSpread,
                Power, Fuel, MotorAccel, Lifetime, HomingRadius, MaxRange);
            else
                return new RocketData(ModelID, HitboxRadius, ExplosionSize, MuzzleVel, BulletSpread,
                Power, Fuel, MotorAccel, Lifetime);
        }
        public override void Create(Vector3 position, Quaternion orientation, Vector3 velocity)
        {
            throw new NotImplementedException();
        }
        public override void Create(Vector3 position, Quaternion orientation, Vector3 velocity, BEPUphysics.Entities.Entity target)
        {
            throw new NotImplementedException();
        }
    }
}
