using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace DCL.Phone.Xna
{
    public class FlatRing: Shape
    {
        public float Radius { get; private set; }

        public float Width { get; private set; }

        public FlatRing(Vector3 center, float radius, float width, int precision)
        {
            Center = center; startCenter = baseCenter = center;
            Radius = radius;

            startVertices = new VertexPositionNormalTexture[precision * 4];
            baseVertices = new VertexPositionNormalTexture[precision * 4];
            currentVertices = new VertexPositionNormalTexture[precision * 4];
            lineIndices = new short[precision * 8];
            triangleIndices = new short[precision * 6];

            //SETTING UP A RING
            float t;
            for (int i = 0; i < precision; i++)
            {
                t = (float)Math.PI * 2 * i / precision;
                currentVertices[i * 4] = new VertexPositionNormalTexture
                                    (new Vector3((radius + width / 2) * (float)Math.Sin(t), (radius + width / 2) * (float)Math.Cos(t), 0) + Center,
                                    Vector3.Up, new Vector2(i/precision, 0));
                currentVertices[i * 4 + 3] = new VertexPositionNormalTexture
                                    (new Vector3((radius - width / 2) * (float)Math.Sin(t), (radius - width / 2) * (float)Math.Cos(t), 0) + Center,
                                    Vector3.Up, new Vector2(i / precision, 1));

                t = (float)Math.PI * 2 * (i + 1) / precision;
                currentVertices[i * 4 + 1] = new VertexPositionNormalTexture
                                    (new Vector3((radius + width / 2) * (float)Math.Sin(t), (radius + width / 2) * (float)Math.Cos(t), 0) + Center,
                                    Vector3.Up, new Vector2((i+1) / precision, 0));
                currentVertices[i * 4 + 2] = new VertexPositionNormalTexture
                                    (new Vector3((radius - width / 2) * (float)Math.Sin(t), (radius - width / 2) * (float)Math.Cos(t), 0) + Center,
                                    Vector3.Up, new Vector2((i + 1) / precision, 1));

                lineIndices[i * 8] = (short)(i * 4);
                lineIndices[i * 8 + 1] = (short)(i * 4 + 1);
                lineIndices[i * 8 + 2] = (short)(i * 4 + 1);
                lineIndices[i * 8 + 3] = (short)(i * 4 + 2);
                lineIndices[i * 8 + 4] = (short)(i * 4 + 2);
                lineIndices[i * 8 + 5] = (short)(i * 4 + 3);
                lineIndices[i * 8 + 6] = (short)(i * 4 + 3);
                lineIndices[i * 8 + 7] = (short)(i * 4);

                triangleIndices[i * 6] = (short)(i * 4);
                triangleIndices[i * 6 + 1] = (short)(i * 4 + 1);
                triangleIndices[i * 6 + 2] = (short)(i * 4 + 2);

                triangleIndices[i * 6 + 3] = (short)(i * 4 + 2);
                triangleIndices[i * 6 + 4] = (short)(i * 4 + 3);
                triangleIndices[i * 6 + 5] = (short)(i * 4);
            }

            Array.Copy(currentVertices, baseVertices, baseVertices.Length);
            Array.Copy(currentVertices, startVertices, startVertices.Length);

        }

        public FlatRing(Vector3 center, float radius, float width, int precision, Texture2D texture) :
            this(center, radius, width, precision)
        {
            Texture = texture;
        }

        public FlatRing(Vector3 center, float radius, float width, int precision, Texture2D texture, GraphicsDevice graphicsDevice) :
            this(center, radius, width, precision, texture)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
