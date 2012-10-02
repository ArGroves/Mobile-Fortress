using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;
using BEPUphysics.CollisionShapes;
using Microsoft.Xna.Framework.Graphics;
using MobileFortressClient.ClientObjects;
using BEPUphysics.Collidables;

namespace MobileFortressClient.Physics
{
    public struct VertexMultitextured
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector4 TexWeights;

        public static int SizeInBytes = (3 + 3 + 4 + 4) * sizeof(float);
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
        {
            new VertexElement(  0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
            new VertexElement(  sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
            new VertexElement(  sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0 ),
            new VertexElement(  sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1 ),
        });
    }
    class Map : DrawableGameComponent
    {
        VertexMultitextured[] vertexPNTs;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int[] indices;

        Terrain Terrain;

        const float verticalScale = 32;
        const float lateralScale = 40;

        public Map(Terrain terrain)
            : base(MobileFortressClient.Game)
        {
            Terrain = terrain;
            var map = Terrain.Shape.Heights;
            int width = map.GetLength(0);
            int x, y;

            vertexPNTs = new VertexMultitextured[width * width];

            ResetVertexBuffer();

            indices = new int[(width - 1) * (width - 1) * 6];
            int counter = 0;
            for (y = 0; y < width - 1; y++)
            {
                for (x = 0; x < width - 1; x++)
                {
                    int lowerLeft = x + y * width;
                    int lowerRight = (x + 1) + y * width;
                    int topLeft = x + (y + 1) * width;
                    int topRight = (x + 1) + (y + 1) * width;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
            MobileFortressClient.Game.Components.Add(this);
            CopyToBuffers();
        }
        void CopyToBuffers()
        {
            vertexBuffer = new VertexBuffer(GraphicsDevice, VertexMultitextured.VertexDeclaration, vertexPNTs.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexMultitextured>(vertexPNTs);

            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(indices);
        }
        public void ResetVertexBuffer()
        {
            float[,] map = Terrain.Shape.Heights;
            int width = map.GetLength(0);
            int x, y;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < width; y++)
                {
                    int i = x + y * width;
                    //vertexPNTs[i].Position = new Vector3(x - width / 2, map[x, y], -y + width / 2) * new Vector3(lateralScale, verticalScale, lateralScale);
                    Terrain.GetPosition(x, y, out vertexPNTs[i].Position);
                    float sx = map[x < width - 1 ? x + 1 : x, y] - map[x > 0 ? x - 1 : x, y];
                    if (x == 0 || x == width - 1)
                        sx *= 2;

                    float sy = map[x, y < width - 1 ? y + 1 : y] - map[x, y > 0 ? y - 1 : y];
                    if (y == 0 || y == width - 1)
                        sy *= 2;

                    vertexPNTs[i].Normal = new Vector3(-sx * verticalScale, 2 * lateralScale, sy * verticalScale);
                    vertexPNTs[i].Normal.Normalize();
                    vertexPNTs[i].TextureCoordinate = new Vector2(x, y) / 2f;

                    if (map[x, y] < 0)
                        vertexPNTs[i].TexWeights = new Vector4(
                            1,
                            0,
                            0,
                            0
                            );
                    else
                        vertexPNTs[i].TexWeights = new Vector4(
                            MathHelper.Clamp(1.0f - Math.Abs(map[x, y] + 1.9f) / 2f, 0, 1),
                            MathHelper.Clamp(1.0f - Math.Abs(map[x, y] - 0.5f) / 0.75f, 0, 1),
                            MathHelper.Clamp(1.0f - Math.Abs(map[x, y] - 3) / 2.5f, 0, 1),
                            MathHelper.Clamp(1.0f - Math.Abs(map[x, y] - 7) / 2.0f, 0, 1)
                            );
                    vertexPNTs[i].TexWeights.Normalize();
                }
            }
            if (vertexBuffer != null) CopyToBuffers();
        }
        public override void Draw(GameTime gameTime)
        {

            Effect effect = Resources.gEffects[2];
            LightMaterial material = Resources.gMaterials[0];

            Weather.SetStandardEffect(ref effect, material, Matrix.Identity);

            effect.CurrentTechnique = effect.Techniques["Simple"];
            effect.Parameters["xTexture0"].SetValue(Resources.Island.SeafloorTex);
            effect.Parameters["xTexture1"].SetValue(Resources.Island.SandTex);
            effect.Parameters["xTexture2"].SetValue(Resources.Island.PlainTex);
            effect.Parameters["xTexture3"].SetValue(Resources.Island.SnowTex);
            effect.Parameters["mipStart"].SetValue(55f);
            effect.Parameters["mipEnd"].SetValue(150f);

            GraphicsDevice.Indices = indexBuffer;
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPNTs.Length, 0, indices.Length / 3);
            }
        }
    }
}
