using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MobileFortressClient.Physics;
using MobileFortressClient.ClientObjects;

namespace MobileFortressClient.Particles
{
    class Particle : ClientObject
    {
        public static Effect bulletEffect;
        public static Random pRandomizer = new Random();
        public static void Initialize(ContentManager Content)
        {
            bulletEffect = Content.Load<Effect>("effects");
        }
        public Vector3 Position;
        public Quaternion Orientation;
        public Vector3 Velocity;
        public float Size;
        public float Alpha = 1f;

        protected SpriteResource Resource;
        protected AudioEmitter Audio = new AudioEmitter();
        protected VertexPositionTexture[] vertices = new VertexPositionTexture[6];

        protected virtual float Gravity { get { return 4.5f; } }
        protected virtual float Damping { get { return 0.001f; } }

        public Particle(ushort spriteID, Vector3 position, Quaternion orientation, Vector3 velocity, float size)
        {
            Position = position;
            Orientation = orientation;
            Velocity = velocity;
            Size = size;
            Resource = Resources.GetSprite(spriteID);
            Sector.Redria.ClientObjects.Add(this);
        }
        public override void Update(float dt)
        {
            Position += Velocity * dt;
            Velocity -= new Vector3(0, Gravity * dt, 0);
            Velocity *= (1f - Damping);
            if (Position.Y < -5) Sector.Redria.ClientObjects.Remove(this);
        }
        protected void UpdateSource()
        {
            Audio.Forward = Velocity;
            Audio.Up = Vector3.Up;
            Audio.Position = Position / Resources.AudioPositionQuotient;
            Audio.Velocity = Velocity;
        }
        void UpdateVertices()
        {
            vertices[0] = new VertexPositionTexture(Position, new Vector2(1, 1));
            vertices[1] = new VertexPositionTexture(Position, new Vector2(0, 0));
            vertices[2] = new VertexPositionTexture(Position, new Vector2(1, 0));

            vertices[3] = new VertexPositionTexture(Position, new Vector2(1, 1));
            vertices[4] = new VertexPositionTexture(Position, new Vector2(0, 1));
            vertices[5] = new VertexPositionTexture(Position, new Vector2(0, 0));
        }
        public override void Draw()
        {
            if (Resource.isLoaded)
            {
                bulletEffect.CurrentTechnique = bulletEffect.Techniques["PointSprites"];
                bulletEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
                bulletEffect.Parameters["xProjection"].SetValue(Camera.Projection);
                bulletEffect.Parameters["xView"].SetValue(Camera.View);
                bulletEffect.Parameters["xCamPos"].SetValue(Camera.Position);
                bulletEffect.Parameters["xTexture"].SetValue(Resource.sprite);
                bulletEffect.Parameters["xCamUp"].SetValue(Vector3.Up);
                bulletEffect.Parameters["xPointSpriteSize"].SetValue(Size);
                bulletEffect.Parameters["xPointSpriteAlpha"].SetValue(Alpha);
                UpdateVertices();
                foreach (EffectPass pass in bulletEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    MobileFortressClient.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
                }
            }
        }
    }
}
