using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;
using BEPUphysics.PositionUpdating;

namespace MobileFortressClient.Physics
{
    class PhysicsObj : DrawableGameComponent
    {
        public Matrix finalTransform
        {
            get { return Resource.transform * worldTransform; }
        }
        public Matrix worldTransform
        {
            get { return Entity.WorldTransform; }
        }
        public Vector3 Position
        {
            get { return Entity.Position; }
            set { Entity.Position = value; }
        }
        public Quaternion Orientation
        {
            get { return Entity.Orientation; }
            set { Entity.Orientation = value; }
        }
        public Vector3 Velocity
        {
            get { return Entity.LinearVelocity; }
            set { Entity.LinearVelocity = value; }
        }

        public Entity Entity { get; set; }

        public ushort ID { get; set; }

        public Vector3 netPosition;
        Quaternion netOrientation = Quaternion.Identity;

        protected GraphicsResource Resource;
        bool hasTexture { get { return Resource.texture != null; } }

        public PhysicsObj(MobileFortressClient game, Vector3 position, Quaternion orientation, ushort resource)
            : base(game)
        {
            Resource = Resources.GetResource(resource);
            Entity = Resources.GetEntity(position, resource);
            Entity.PositionUpdateMode = PositionUpdateMode.Continuous;
            game.Components.Add(this);
            Sector.Redria.Add(this);
        }
        public PhysicsObj(MobileFortressClient game) : base(game)
        {
        }
        Matrix[] boneTransforms { get; set; }
        public override void Draw(GameTime gameTime)
        {
            Effect Effect = Resource.Effect;
            LightMaterial material = Resources.gMaterials[0];
            
            foreach (ModelMesh mesh in Resource.model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Matrix world = mesh.ParentBone.Transform*finalTransform;
                    part.Effect = Effect;
                    Weather.SetStandardEffect(ref Effect, material, world);
                    if (hasTexture)
                    {
                        Effect.CurrentTechnique = Effect.Techniques["TTexSM2"];
                        Effect.Parameters["xTexture"].SetValue(Resource.texture);
                    }
                    else Effect.CurrentTechnique = Effect.Techniques["TSM2"];
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

        public virtual void Update(float dt)
        {
            if (netPosition != Vector3.Zero)
            {
                Vector3 positionDifference = (netPosition - Position);
                if (positionDifference.LengthSquared() > 2*2) Position = netPosition;
                else Position = Vector3.SmoothStep(Position, netPosition, Network.Interpolation);
                if ((netOrientation - Orientation).LengthSquared() > 1*1) Orientation = netOrientation;
                else Orientation = Quaternion.Slerp(Orientation, netOrientation, 0.2f);
            }
        }

        public void UpdateFromNet(Vector3 position, Quaternion orientation, Vector3 velocity)
        {
            netPosition = position;
            netOrientation = orientation;
            //Velocity = velocity;
        }
        public virtual void Destroy()
        {
            Sector.Redria.Space.Remove(Entity);
            Sector.Redria.Objects.Remove(this);
            MobileFortressClient.Game.Components.Remove(this);
        }
    }
}
