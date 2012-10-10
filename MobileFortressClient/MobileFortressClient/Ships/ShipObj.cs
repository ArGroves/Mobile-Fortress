using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Constraints.SingleEntity;
using Microsoft.Xna.Framework;
using MobileFortressClient.Physics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Constraints.TwoEntity.Motors;
using MobileFortressClient.Data;
using Microsoft.Xna.Framework.Audio;
using MobileFortressClient.Particles;
using Microsoft.Xna.Framework.Graphics;
using MobileFortressClient.Managers;
using MobileFortressClient.Menus;

namespace MobileFortressClient.Ships
{
    class ShipObj : PhysicsObj
    {

        SingleEntityLinearMotor Thrusters;
        SingleEntityAngularMotor ControlSurfaces;

        public AudioEmitter Audio = new AudioEmitter();

        public SoundEffectInstance engineNoise;

        ShipData Data;

        //bool engineParticle = false;

        public float ArmorLeft(int cH)
        {
            MobileFortressClient.statusLine = "Armor: " + cH + "/" + Data.TotalArmor;
            return (float)cH / Data.TotalArmor;
        }

        public ShipObj(MobileFortressClient game,Vector3 position, Quaternion orientation, ShipData data)
            : base(game,position,orientation,2)
        {
            Data = data;
            Data.ComposeHitbox();
            Data.Hitbox.Position = position;
            Sector.Redria.Space.Add(Data.Hitbox);
            this.Entity = Data.Hitbox;

            //Thrusters = new SingleEntityLinearMotor(Entity, Position);
            //ControlSurfaces = new SingleEntityAngularMotor(Entity);
            //ControlSurfaces.Settings.Mode = MotorMode.Servomechanism;
            //ControlSurfaces.Settings.Servo.MaxCorrectiveVelocity = 2;

            //Thrusters.Settings.Mode = MotorMode.VelocityMotor;
            //Thrusters.Settings.VelocityMotor.Softness = 0.002f;

            //Sector.Redria.Space.Add(Thrusters);
            //Sector.Redria.Space.Add(ControlSurfaces);
            Entity.IsAffectedByGravity = false;

            engineNoise = Resources.Sounds.Engine.CreateInstance();
            engineNoise.Apply3D(Camera.Audio, Audio);
            engineNoise.IsLooped = true;
            engineNoise.Play();
            Entity.CollisionInformation.Tag = this;
        }
        public void Explode()
        {
            new PExplosion(Position,25);
            var explosionNoise = Resources.Sounds.ShipExplosion().CreateInstance();
            explosionNoise.Apply3D(Camera.Audio, Audio);
            explosionNoise.Play();
            Sector.Redria.Ships.Remove(this);
            //Sector.Redria.Space.Remove(Thrusters);
            //Sector.Redria.Space.Remove(ControlSurfaces);
            Sector.Redria.Space.Remove(Entity);
            Game.Components.Remove(this);
        }
        public void SetThrottle(float throttle)
        {
            engineNoise.Pitch = -0.5f+throttle;
        }
        public override void Update(float dt)
        {
            float Thrust = Data.Thrust;
            float vVel = 0;
            float hVel = 0;
            if (Camera.Target == this)
            {
                if (Controls.Instance.Up) vVel += Data.StrafeVelocity;
                if (Controls.Instance.Down) vVel -= Data.StrafeVelocity;
                if (Controls.Instance.Left) hVel -= Data.StrafeVelocity;
                if (Controls.Instance.Right) hVel += Data.StrafeVelocity;
                //Thrusters.Settings.VelocityMotor.GoalVelocity = Vector3.Transform(new Vector3(hVel, vVel, -Thrust), Orientation);

                Quaternion target = Quaternion.CreateFromRotationMatrix(Matrix.CreateFromYawPitchRoll(Controls.Instance.Yaw, Controls.Instance.Pitch, Controls.Instance.Roll));
                //ControlSurfaces.Settings.Servo.Goal = target;
                //ControlSurfaces.Settings.Servo.MaxCorrectiveVelocity = Data.Turn / 5;
            }
            else
            {
                //Thrusters.Settings.VelocityMotor.GoalVelocity = Vector3.Transform(new Vector3(0, 0, -Thrust), Orientation);
            }

            /*if (engineParticle = !engineParticle)
            {
                float x = (float)Particle.pRandomizer.NextDouble() - .5f;
                float y = (float)Particle.pRandomizer.NextDouble() - .5f;
                Vector3 vel = Vector3.Transform(new Vector3(x, y, 4), Orientation);
                new PEngine(this, worldTransform.Backward * 1.5f + worldTransform.Right*hVel*0.01f + worldTransform.Up*vVel*0.01f, vel, 1.75f);

                if (Position.Y < 10)
                {
                    Matrix YawM = Matrix.CreateFromAxisAngle(Vector3.Up, Controls.Instance.Yaw);
                    var r = (float)Particle.pRandomizer.NextDouble() - 0.5f;
                    new PGroundSmoke(this, Vector3.Transform(new Vector3(0, 0, -3), YawM), Vector3.Transform(new Vector3(r * 5, 0, 8), YawM), 1);
                }
            }*/

            Audio.Forward = worldTransform.Forward;
            Audio.Up = worldTransform.Up;
            Audio.Position = Position/Resources.AudioPositionQuotient;
            Audio.Velocity = Velocity;

            base.Update(dt);
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsResource Nose;
            GraphicsResource Core;
            GraphicsResource Tail;
            Data.SetDrawingResources(out Nose, out Core, out Tail);

            LightMaterial Material = Resources.gMaterials[0];
            DrawPart(Core, Core.Effect, Material, Vector3.Zero, Data.CoreColor);
            DrawPart(Nose, Nose.Effect, Material, Vector3.Forward*1.5f, Data.NoseColor);
            DrawPart(Tail, Tail.Effect, Material, Vector3.Backward*1.5f, Data.TailColor);
            foreach (KeyValuePair<Vector3, WeaponData> kvp in Data.Weapons)
            {
                if (kvp.Value != null && kvp.Value.Draw)
                {
                    var res = Resources.GetResource((ushort)(Resources.WeaponIndex + kvp.Value.Index));
                    DrawPart(res, res.Effect, Material, kvp.Key, Data.WeaponColor);
                }
            }
        }

        void DrawPart(GraphicsResource Part, Effect Effect, LightMaterial Mat, Vector3 Offset, Color Color)
        {
            Effect.Parameters["customColors"].SetValue(true);
            Effect.Parameters["customColorA"].SetValue(Color.ToVector4());
            if (Part.lightmap != null)
            {
                Effect.Parameters["xLightmap"].SetValue(Part.lightmap);
            }
            foreach (ModelMesh mesh in Part.model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Matrix world = mesh.ParentBone.Transform * Matrix.CreateTranslation(Offset) * finalTransform;
                    part.Effect = Effect;
                    Weather.SetStandardEffect(ref Effect, Mat, world);
                    
                    if (Part.texture != null)
                    {
                        Effect.CurrentTechnique = Effect.Techniques["TTexSM2"];
                        Effect.Parameters["xTexture"].SetValue(Part.texture);
                    }
                    else
                        Effect.CurrentTechnique = Effect.Techniques["TSM2"];
                }
                mesh.Draw();
            }
            Effect.Parameters["customColors"].SetValue(false);
        }
    }
}
