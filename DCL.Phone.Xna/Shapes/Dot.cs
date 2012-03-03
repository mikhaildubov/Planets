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
    public class Dot: Shape
    {
        public Dot(Vector3 position, int scale)
        {
            if (scale <= 0) throw new ArgumentOutOfRangeException("scale", "Parameter must be grater than zero");

            Center = startCenter = position;

            startVertices = new VertexPositionNormalTexture[3];
            currentVertices = new VertexPositionNormalTexture[3];
            lineIndices = new short[6];
            triangleIndices = new short[6];

            currentVertices[0] = new VertexPositionNormalTexture
                                    (position - 0.01f * scale * Vector3.UnitX + 0.01f * scale * Vector3.UnitZ,
                                    Vector3.Up, Vector2.Zero);
            currentVertices[1] = new VertexPositionNormalTexture
                                    (position + 0.01f * scale * Vector3.UnitX + 0.01f * scale * Vector3.UnitZ,
                                    Vector3.Up, Vector2.UnitX);
            currentVertices[2] = new VertexPositionNormalTexture
                                    (position - 0.01f * scale * Vector3.UnitZ,
                                    Vector3.Up, Vector2.UnitY);

            lineIndices[0] = 0; lineIndices[1] = 1; lineIndices[2] = 1; lineIndices[3] = 2; lineIndices[4] = 2; lineIndices[5] = 0;
            triangleIndices[0] = 0; triangleIndices[1] = 1; triangleIndices[2] = 2; triangleIndices[3] = 2; triangleIndices[4] = 1; triangleIndices[5] = 0;

            Array.Copy(currentVertices, startVertices, startVertices.Length);
        }

        public Dot(Vector3 position, int scale, Texture2D texture) :
            this(position, scale)
        {
            Texture = texture;
        }

        public Dot(Vector3 position, int scale, Texture2D texture, GraphicsDevice graphicsDevice) :
            this(position, scale, texture)
        {
            GraphicsDevice = graphicsDevice;
        }

    }
}
