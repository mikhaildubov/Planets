using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCL.Phone.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Planets
{
    class FlatRingSector: Shape
    {
        public float Radius { get; private set; }

        public float Width { get; private set; }

        public FlatRingSector(Vector3 center, float radius, float width, int precision, float angle)
        {
            Center = center;
            Radius = radius;

            startVertices = new VertexPositionNormalTexture[precision * 4];
            currentVertices = new VertexPositionNormalTexture[precision * 4];
            lineIndices = new short[precision * 8];
            triangleIndices = new short[precision * 12];

            //SETTING UP A RING
            float t;
            for (int i = 0; i < precision; i++)
            {
                t = angle * i / precision;
                currentVertices[i * 4] = new VertexPositionNormalTexture
                                    (new Vector3((radius + width / 2) * (float)Math.Sin(t), 0, (radius + width / 2) * (float)Math.Cos(t)) + Center,
                                    Vector3.Up, new Vector2((float)i/precision, 0));
                currentVertices[i * 4 + 3] = new VertexPositionNormalTexture
                                    (new Vector3((radius - width / 2) * (float)Math.Sin(t), 0, (radius - width / 2) * (float)Math.Cos(t)) + Center,
                                    Vector3.Up, new Vector2((float)i / precision, 1));

                t = angle * (i + 1) / precision;
                currentVertices[i * 4 + 1] = new VertexPositionNormalTexture
                                    (new Vector3((radius + width / 2) * (float)Math.Sin(t), 0,  (radius + width / 2) * (float)Math.Cos(t)) + Center,
                                    Vector3.Up, new Vector2((float)(i + 1) / precision, 0));
                currentVertices[i * 4 + 2] = new VertexPositionNormalTexture
                                    (new Vector3((radius - width / 2) * (float)Math.Sin(t), 0, (radius - width / 2) * (float)Math.Cos(t)) + Center,
                                    Vector3.Up, new Vector2((float)(i + 1) / precision, 1));

                lineIndices[i * 8] = (short)(i * 4);
                lineIndices[i * 8 + 1] = (short)(i * 4 + 1);
                lineIndices[i * 8 + 2] = (short)(i * 4 + 1);
                lineIndices[i * 8 + 3] = (short)(i * 4 + 2);
                lineIndices[i * 8 + 4] = (short)(i * 4 + 2);
                lineIndices[i * 8 + 5] = (short)(i * 4 + 3);
                lineIndices[i * 8 + 6] = (short)(i * 4 + 3);
                lineIndices[i * 8 + 7] = (short)(i * 4);

                //Кольцо - с обейих сторон
                triangleIndices[i * 12] = (short)(i * 4);
                triangleIndices[i * 12 + 1] = (short)(i * 4 + 1);
                triangleIndices[i * 12 + 2] = (short)(i * 4 + 2);

                triangleIndices[i * 12 + 3] = (short)(i * 4 + 2);
                triangleIndices[i * 12 + 4] = (short)(i * 4 + 3);
                triangleIndices[i * 12 + 5] = (short)(i * 4);

                triangleIndices[i * 12 + 6] = (short)(i * 4);
                triangleIndices[i * 12 + 7] = (short)(i * 4 + 2);
                triangleIndices[i * 12 + 8] = (short)(i * 4 + 1);

                triangleIndices[i * 12 + 9] = (short)(i * 4 + 3);
                triangleIndices[i * 12 + 10] = (short)(i * 4 + 2);
                triangleIndices[i * 12 + 11] = (short)(i * 4);
            }

            Center = currentVertices[2].Position;
            startCenter = Center;

            Array.Copy(currentVertices, startVertices, startVertices.Length);

        }

        public FlatRingSector(Vector3 center, float radius, float width, int precision, float angle, Texture2D texture) :
            this(center, radius, width, precision, angle)
        {
            Texture = texture;
        }

        public FlatRingSector(Vector3 center, float radius, float width, int precision, float angle, Texture2D texture, GraphicsDevice graphicsDevice) :
            this(center, radius, width, precision, angle, texture)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
