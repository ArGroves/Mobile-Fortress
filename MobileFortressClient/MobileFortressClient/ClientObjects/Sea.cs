using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.ClientObjects
{
    class Seaplane : ClientObject
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        const float SeaSize = 2500;

        public Seaplane()
            : base()
        {
            var device = MobileFortressClient.Game.GraphicsDevice;
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), 4, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(
                new VertexPositionNormalTexture[]
                {
                    new VertexPositionNormalTexture(new Vector3(-SeaSize,0,-SeaSize),Vector3.Up,new Vector2(0,0)),
                    new VertexPositionNormalTexture(new Vector3(SeaSize,0,-SeaSize),Vector3.Up,new Vector2(SeaSize/20,0)),
                    new VertexPositionNormalTexture(new Vector3(-SeaSize,0,SeaSize),Vector3.Up,new Vector2(0,SeaSize/20)),
                    new VertexPositionNormalTexture(new Vector3(SeaSize,0,SeaSize),Vector3.Up,new Vector2(SeaSize/20,SeaSize/20))
                }
                );
            /*
             * 0--1
             * |  |
             * 2--3
             */
            indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, 6, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(new int[] { 0, 1, 2, 2, 1, 3 });
        }
        public override void Draw()
        {
            var device = MobileFortressClient.Game.GraphicsDevice;
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            Effect effect = Resources.gEffects[0];
            LightMaterial material = Resources.gMaterials[1];
            Weather.SetStandardEffect(ref effect, material, Matrix.Identity);

            effect.CurrentTechnique = effect.Techniques["TTexSM2"];
            effect.Parameters["xTexture"].SetValue(Resources.Island.OceanTex);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            }
        }
        public override void Update(float dt)
        {
        }
    }
}
