using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MobileFortressClient.Physics;
using Microsoft.Xna.Framework.Audio;

namespace MobileFortressClient.ClientObjects
{
    class Explosion : ClientObject
    {
        Vector3 position;
        float rotation = 0f;
        float radius;
        const float decAccel = 10f;
        const float rotationRate = MathHelper.PiOver2;

        Matrix Transform;

        float decrease = 0f;

        public Explosion(Vector3 position, float radius)
        {
            SoundEffectInstance explosionNoise;
            if (radius > 15)
                explosionNoise = Resources.Sounds.ShipExplosion().CreateInstance();
            else
                explosionNoise = Resources.Sounds.Explosion().CreateInstance();
            explosionNoise.Apply3D(Camera.Audio,
                new AudioEmitter()
                {
                    Position = position / Resources.AudioPositionQuotient,
                    Forward = Vector3.Forward,
                    Up = Vector3.Up,
                    Velocity = Vector3.Zero
                });
            explosionNoise.Play();
            this.position = position;
            this.radius = radius;
            Transform = Matrix.CreateTranslation(position) * Matrix.CreateScale(radius);
            Sector.Redria.ClientObjects.Add(this);
        }

        public override void Update(float dt)
        {
            Transform = Matrix.CreateScale(radius)*Matrix.CreateRotationY(rotation)*Matrix.CreateTranslation(position);
            radius -= decrease * dt;
            decrease += decAccel * dt;
            rotation += rotationRate * dt;
            if (radius <= 0)
                Sector.Redria.ClientObjects.Remove(this);
        }
        public override void Draw()
        {
            GraphicsDevice device = MobileFortressClient.Game.GraphicsDevice;
            device.RasterizerState = RasterizerState.CullNone;
            GraphicsResource explosionResource = Resources.GetResource(1);
            Effect Effect = explosionResource.Effect;

            LightMaterial material = Resources.gMaterials[2];
            foreach (ModelMesh mesh in explosionResource.model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Matrix world = mesh.ParentBone.Transform * Transform * explosionResource.transform;
                    part.Effect = Effect;
                    Weather.SetStandardEffect(ref Effect, material, world);
                    Effect.CurrentTechnique = Effect.Techniques["TTexSM2"];
                    Effect.Parameters["xTexture"].SetValue(explosionResource.texture);
                }
                mesh.Draw();
            }
            device.RasterizerState = RasterizerState.CullCounterClockwise;

        }
    }
}
