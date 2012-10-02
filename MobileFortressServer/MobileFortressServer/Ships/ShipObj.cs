using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Physics;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;
using MobileFortressServer.Data;
using MobileFortressServer.Messages;
using Lidgren.Network;
using MobileFortressClient.Messages;

namespace MobileFortressServer.Ships
{
    class ShipObj : PhysicsObj
    {
        public ControlMechanism Controls = new ControlMechanism();

        SingleEntityLinearMotor Thrusters;
        SingleEntityAngularMotor ControlSurfaces;

        public ShipData Data;

        public Soul Client;

        ShipObj prevTarget;
        ShipObj Target;

        bool Locked = false;

        public float ClampedYaw
        {
            get
            {
                float shipYaw = (float)Math.Acos(Vector3.Dot(Entity.WorldTransform.Forward, Vector3.Forward));
                return MathHelper.Clamp(Controls.Yaw, shipYaw + MathHelper.PiOver4, shipYaw - MathHelper.PiOver4);
            }
        }

        public ShipObj(Vector3 position, Quaternion orientation, ShipData data)
            : base()
        {
            Data = data;
            Data.ComposeHitbox();
            Data.Hitbox.Position = position;
            Data.Hitbox.Orientation = orientation;
            this.SetEntity(Data.Hitbox, 2);

            Health = Data.TotalArmor;
            Thrusters = new SingleEntityLinearMotor(Entity, Position);
            ControlSurfaces = new SingleEntityAngularMotor(Entity);
            ControlSurfaces.Settings.Mode = MotorMode.Servomechanism;

            Thrusters.Settings.Mode = MotorMode.VelocityMotor;
            Thrusters.Settings.VelocityMotor.Softness = 0.002f;

            Sector.Redria.Space.Add(Thrusters);
            Sector.Redria.Space.Add(ControlSurfaces);
            Entity.IsAffectedByGravity = false;
            Network.AddShip();
        }

        void SendMissileLockon(LockonStatus status)
        {
            if (Client != null)
            {
                NetOutgoingMessage msg = Network.Server.CreateMessage();
                if (Target == null)
                {
                    MessageWriter.MissileLockonMessage(Client.Owner, ushort.MaxValue, LockonStatus.NotLocked);
                }
                else
                {
                    MessageWriter.MissileLockonMessage(Client.Owner, Target.ID, status);
                    if (status != LockonStatus.NotLocked)
                    {
                        MessageWriter.MissileLockonMessage(Target.Client.Owner, 0, LockonStatus.EnemyLock);
                    }
                    else
                    {
                        MessageWriter.MissileLockonMessage(Target.Client.Owner, 0, LockonStatus.MissileDestroyed);
                    }
                }
                
            }
        }

        public override void Update(float dt)
        {
            float vVel = 0;
            float hVel = 0;
            float Thrust = Data.Thrust;
            if (Controls.Up) vVel += Data.StrafeVelocity;
            if (Controls.Down) vVel -= Data.StrafeVelocity;
            if (Controls.Left) hVel -= Data.StrafeVelocity;
            if (Controls.Right) hVel += Data.StrafeVelocity;
            Thrusters.Settings.VelocityMotor.GoalVelocity = Vector3.Transform(new Vector3(hVel, vVel, -Thrust), Orientation);

            float minAoA = MathHelper.PiOver2;

            foreach (ShipObj ship in Sector.Redria.Ships.table)
            {
                float AoA = AngleOfAttack(ship.Position, Position, Entity.WorldTransform.Forward);
                if (AoA < minAoA)
                {
                    Target = ship;
                    minAoA = AoA;
                }
            }

            foreach (KeyValuePair<Vector3, WeaponData> kvp in Data.Weapons)
            {
                WeaponData weapon = kvp.Value;
                if (weapon.Cooldown > 0) weapon.Cooldown -= dt;
                else if (weapon.CurrentAmmo <= 0)
                    weapon.CurrentAmmo = weapon.MaxAmmo;
                if (weapon.UseLockon)
                {
                    if (weapon.CurrentAmmo > 0 && Target != null &&
                        minAoA < weapon.LockonRadius)
                    {
                        if (Target != prevTarget)
                        {
                            weapon.CurrentLockonTime = 0;
                        }
                        if (weapon.CurrentLockonTime <= 0)
                        {
                            Locked = false;
                            SendMissileLockon(LockonStatus.Locking);
                        }
                        weapon.CurrentLockonTime += dt;
                        if (weapon.CurrentLockonTime >= weapon.LockonTime && !Locked)
                        {
                            Locked = true;
                            SendMissileLockon(LockonStatus.Locked);
                        }
                    }
                    else if(weapon.CurrentLockonTime > 0)
                    {
                        weapon.CurrentLockonTime = 0;
                        Locked = false;
                        SendMissileLockon(LockonStatus.NotLocked);
                    }
                }
                if (weapon.CurrentAmmo > 0 && weapon.Cooldown <= 0)
                {
                    if ((Controls.leftMouse && weapon.fireGroup == 0) || (Controls.rightMouse && weapon.fireGroup == 1))
                    {
                        if (weapon.UseLockon)
                        {
                            if (weapon.CurrentLockonTime >= weapon.LockonTime)
                            {
                                weapon.CurrentAmmo--;
                                if (weapon.CurrentAmmo > 0)
                                {
                                    weapon.Cooldown = weapon.InverseRoF;
                                }
                                else
                                {
                                    weapon.Cooldown = weapon.ReloadTime;
                                }
                                Vector3 firePos = Vector3.Transform(kvp.Key + Vector3.Forward * weapon.Projectile.ForwardOffset, Entity.Orientation) + Entity.Position;
                                Vector3 fireVel;
                                weapon.Projectile.Create(firePos, Entity.Orientation, Entity.LinearVelocity, out fireVel, Target);
                                MessageWriter.MissileLockonMessage(Target.Client.Owner, 0, LockonStatus.EnemyMissile);
                            }
                        }
                        else
                        {
                            weapon.CurrentAmmo--;
                            if (weapon.CurrentAmmo > 0)
                            {
                                weapon.Cooldown = weapon.InverseRoF;
                            }
                            else
                            {
                                weapon.Cooldown = weapon.ReloadTime;
                            }
                            Vector3 firePos = Vector3.Transform(kvp.Key + Vector3.Forward * weapon.Projectile.ForwardOffset, Entity.Orientation) + Entity.Position;
                            Vector3 fireVel;
                            weapon.Projectile.Create(firePos, Entity.Orientation, Entity.LinearVelocity, out fireVel);
                            if (fireVel != Vector3.Zero)
                            {
                                MessageWriter.FireBulletMessage(weapon.ProjectileID, firePos, Entity.Orientation, fireVel);
                            }
                        }
                    }
                }
            }

            prevTarget = Target;

            Quaternion target = Quaternion.CreateFromRotationMatrix(Matrix.CreateFromYawPitchRoll(Controls.Yaw, Controls.Pitch, Controls.Roll));
            ControlSurfaces.Settings.Servo.Goal = target;
            ControlSurfaces.Settings.Servo.MaxCorrectiveVelocity = Data.Turn;
        }

        public float Health = 650;

        public void BulletStrike(BulletData bullet, float velocity)
        {
            var damage = bullet.Power;// * (velocity / bullet.MuzzleVel);
            TakeDamage(damage);
        }

        public void ExplosionStrike(float power, float radius, float distance)
        {
            var damage = power; //Todo: Make some kind of polynomial distance-damage relation.
            Console.WriteLine("Explosion Damage: " + damage);
            TakeDamage(damage);
        }

        void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Sector.Redria.Ships.Remove(this);
                Sector.Redria.Space.Remove(this.ControlSurfaces);
                Sector.Redria.Space.Remove(this.Thrusters);
                Sector.Redria.Space.Remove(this.Entity);
                MessageWriter.ShipExplosionMessage(this.ID);
            }
        }

        public void SendStatus(Lidgren.Network.NetConnection connection)
        {
            var Ammo = new ushort[Data.WeaponIDs.Length];
            int i = 0;
            foreach (WeaponData weapon in Data.Weapons.Values)
            {
                Ammo[i++] = weapon.CurrentAmmo;
            }
            MessageWriter.StatusMessage(connection, Health, Ammo);
        }
        public static float AngleOfAttack(Vector3 targetPoint, Vector3 position, Vector3 orientation)
        {
            Vector3 difference = targetPoint - position;
            difference.Normalize();
            return (float)Math.Acos(Vector3.Dot(orientation, difference));
        }
    }
}
