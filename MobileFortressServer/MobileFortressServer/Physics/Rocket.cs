using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Data;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Constraints.SingleEntity;
using MobileFortressServer.Ships;
using MobileFortressServer.Messages;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Collidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Constraints.TwoEntity.Motors;
using Lidgren.Network;

namespace MobileFortressServer.Physics
{
    class Rocket : PhysicsObj
    {
        RocketData Data;
        public float Damping { get { return 0.01f; } }
        public PhysicsObj Target {get; set;}

        float currentFuel;
        float currentLife;

        bool exploded = false;

        float forwardVel = 0f;

        SingleEntityLinearMotor rocketMotor;
        SingleEntityAngularMotor trackingMotor;

        public Rocket(RocketData data, Vector3 position, Quaternion orientation)
            : base(new Sphere(position, data.HitboxRadius, 5),data.ModelID)
        {
            Data = data;
            float x = (float)(ProjectileData.pRandom.NextDouble() - 0.5f) * data.BulletSpread;
            float y = (float)(ProjectileData.pRandom.NextDouble() - 0.5f) * data.BulletSpread;
            Quaternion random = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(x), MathHelper.ToRadians(y), 0);
            orientation = random * orientation;
            Vector3 velocity = Vector3.Transform(new Vector3(0,0,-Data.MuzzleVel), orientation);
            Entity.LinearVelocity = velocity;
            forwardVel = Data.MuzzleVel;
            Entity.Orientation = orientation;

            currentFuel = Data.Fuel;
            currentLife = Data.Lifetime;

            rocketMotor = new SingleEntityLinearMotor(Entity, Entity.Position);
            rocketMotor.Settings.Mode = MotorMode.VelocityMotor;
            Sector.Redria.Space.Add(rocketMotor);
            trackingMotor = new SingleEntityAngularMotor(Entity);
            trackingMotor.Settings.Mode = MotorMode.Servomechanism;
            trackingMotor.Settings.Servo.MaxCorrectiveVelocity = 1.5f;
            trackingMotor.Settings.Servo.Goal = orientation;
            Sector.Redria.Space.Add(trackingMotor);
            Entity.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
        }

        void Events_InitialCollisionDetected(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            Explode();
        }

        public override void Update(float dt)
        {
            if (currentFuel > 0)
            {
                forwardVel += Data.MotorAccel * dt;
                Vector3 targetVelocity = Vector3.Transform(new Vector3(0, 0, -forwardVel), Entity.Orientation);
                rocketMotor.Settings.VelocityMotor.GoalVelocity = targetVelocity;
                currentFuel -= dt;
                #region Guidance
                if (Target != null)
                {
                    if (!trackingMotor.IsActive)
                        trackingMotor.IsActive = true;
                    Vector3 Direction = Target.Position - this.Entity.Position;
                    Direction.Normalize();
                    Quaternion targetOrientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(Vector3.Zero, Direction, Vector3.Up));
                    trackingMotor.Settings.Servo.Goal = targetOrientation;
                }
                else
                {
                    trackingMotor.IsActive = false;
                }
                #endregion
            }
            else
            {
                rocketMotor.IsActive = false;
            }

            currentLife -= dt;

            if (currentLife <= 0)
            {
                Explode();
            }
        }

        void Explode()
        {
            if (!exploded)
            {
                foreach (ShipObj ship in Sector.Redria.Ships.table)
                {
                    Vector3 outVector = ship.Position - Position;
                    float distance = outVector.Length();
                    if (distance < Data.ExplosionSize)
                    {
                        ship.ExplosionStrike(Data.Power, Data.ExplosionSize, distance);
                    }
                }
                MessageWriter.ExplosionMessage(ID, Data.ExplosionSize);
                if (Target is ShipObj)
                {
                    var EnemyShip = (ShipObj)Target;
                    MessageWriter.MissileLockonMessage(EnemyShip.Client.Owner, 0, LockonStatus.MissileDestroyed);
                }
                Sector.Redria.Objects.Remove(this);
                Sector.Redria.Space.Remove(rocketMotor);
                Sector.Redria.Space.Remove(trackingMotor);
                Sector.Redria.Space.Remove(this.Entity);
                exploded = true;
            }
        }
    }
}