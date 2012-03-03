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
    /// <summary>
    /// Represents an ellipse which is actually a many sided polygon.
    /// </summary>
    public class Ellipse : Shape
    {
        #region Properties
        /// <summary>
        /// Gets the radius of the ellipse.
        /// </summary>
        public float Radius { get; private set; }

        /// <summary>
        /// Gets a normalized vector with direction from the center to the initial upper point.
        /// </summary>
        public Vector3 Axis
        {
            get
            {
                Vector3 v = new Vector3(currentVertices[0].Position.X,
                                         currentVertices[0].Position.Y,
                                         currentVertices[0].Position.Z) - Center;
                v.Normalize();
                return v;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Sets up an ellipse.
        /// </summary>
        /// <param name="cent">The center of the ellipse.</param>
        /// <param name="radius">The radius of the ellipse.</param>
        /// <param name="radiusRatioX">The coefficient of stretching along the X axis.</param>
        /// <param name="radiusRatioY">The coefficient of stretching along the Y axis.</param>
        /// <param name="precision">A factor which influences the precision with that the ellipse is drawn. The value is actually the number of sides in the polygon.</param>
        public Ellipse(Vector3 center, float radius, float radiusRatioX, float radiusRatioY, int precision)
        {
            if (radius <= 0) throw new ArgumentOutOfRangeException("radius", "Radius must be a positive value");
            if (precision <= 0) throw new ArgumentOutOfRangeException("precision", "Precision coefficient must be a positive value");

            Center = startCenter = center;
            Radius = radius;

            startVertices = new VertexPositionNormalTexture[precision * 2 + 1];
            currentVertices = new VertexPositionNormalTexture[precision * 2 + 1];
            lineIndices = new short[precision * 2];
            triangleIndices = new short[precision * 3];

            //SETTING UP AN ELLIPSE
            float t;//parameter
            for (int i = 0; i < precision; i++)
            {
                t = (float)Math.PI * 2 * i / precision;
                currentVertices[i * 2] = new VertexPositionNormalTexture
                                    (new Vector3(radius * radiusRatioX * (float)Math.Sin(t), radius * radiusRatioY * (float)Math.Cos(t), 0) + Center,
                                    Vector3.Up, Vector2.Zero);
                t = (float)Math.PI * 2 * (i + 1) / precision;
                currentVertices[i * 2 + 1] = new VertexPositionNormalTexture
                                    (new Vector3(radius * radiusRatioX * (float)Math.Sin(t), radius * radiusRatioY * (float)Math.Cos(t), 0) + Center,
                                    Vector3.Up, Vector2.UnitX);

                lineIndices[i * 2] = (short)(i * 2);
                lineIndices[i * 2 + 1] = (short)(i * 2 + 1);

                triangleIndices[i * 3] = (short)(i * 2);
                triangleIndices[i * 3 + 1] = (short)(i * 2 + 1);
                triangleIndices[i * 3 + 2] = (short)(precision * 2);
            }

            currentVertices[precision * 2] = new VertexPositionNormalTexture
                                    (Center, Vector3.Up, Vector2.UnitY);

            Array.Copy(currentVertices, startVertices, startVertices.Length);
        }

        /// <summary>
        /// Sets up an ellipse.
        /// </summary>
        /// <param name="cent">The center of the ellipse.</param>
        /// <param name="radius">The radius of the ellipse.</param>
        /// <param name="radiusRatioX">The coefficient of stretching along the X axis.</param>
        /// <param name="radiusRatioY">The coefficient of stretching along the Y axis.</param>
        /// <param name="precision">A factor which influences the precision with that the ellipse is drawn. The value is actually the number of sides in the polygon.</param>
        /// <param name="texture">The texture that is drawn to the surface of the ellipse.</param>
        public Ellipse(Vector3 cent, float radius, float radiusRatioX, float radiusRatioY, int precision, Texture2D texture) :
            this(cent, radius, radiusRatioX, radiusRatioY, precision)
        {
            Texture = texture;
        }

        /// <summary>
        /// Sets up an ellipse.
        /// </summary>
        /// <param name="cent">The center of the ellipse.</param>
        /// <param name="radius">The radius of the ellipse.</param>
        /// <param name="radiusRatioX">The coefficient of stretching along the X axis.</param>
        /// <param name="radiusRatioY">The coefficient of stretching along the Y axis.</param>
        /// <param name="precision">A factor which influences the precision with that the ellipse is drawn. The value is actually the number of sides in the polygon.</param>
        /// <param name="texture">The texture that is drawn to the surface of the ellipse.</param>
        /// <param name="graphicsDevice">The graphics device to which the ellipse is drawn.</param>
        public Ellipse(Vector3 cent, float radius, float radiusRatioX, float radiusRatioY, int precision, Texture2D texture, GraphicsDevice graphicsDevice) :
            this(cent, radius, radiusRatioX, radiusRatioY, precision, texture)
        {
            GraphicsDevice = graphicsDevice;
        }
        #endregion
    }
}
