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
    /// Represents an ellipse which is actually a many sided three-dimensional polygon.
    /// </summary>
    public class Ellipsoid: Shape
    {
        #region Properties
        /// <summary>
        /// Gets the radius.
        /// </summary>
        public float Radius { get; private set; }

        /// <summary>
        /// Gets a normalized vector with direction from the center to the upper pole.
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
        /// <param name="radiusRatioZ">The coefficient of stretching along the Z axis.</param>
        /// <param name="precision">A factor which influences the precision with that the ellipse is drawn. The recommended value for Windows Phone 7 is 10-14.</param>
        public Ellipsoid(Vector3 cent, float radius, float radiusRatioX, float radiusRatioY, float radiusRatioZ, int precision)
        {
            if (radius <= 0) throw new ArgumentOutOfRangeException("radius", "Radius must be a positive value");
            if (precision <= 0) throw new ArgumentOutOfRangeException("precision", "Precision coefficient must be a positive value");

            Center = startCenter = cent;
            Radius = radius;
            precision *= 2;

            startVertices = new VertexPositionNormalTexture[precision * (precision + 1) * 4];
            currentVertices = new VertexPositionNormalTexture[precision * (precision + 1) * 4];
            lineIndices = new short[precision * precision * 8];
            triangleIndices = new short[precision * precision * 6];

            //SETTING UP A SPHERE
            float p, t; //parameters
            Vector3 Point; //Normal, temp;
            int ind;
            for (int j = 0; j < precision; j++)
            {//2 + (n-2)/2
                //SETTING UP A CIRCLE
                for (int i = 0; i < precision; i++)
                {
                    ind = (j * precision + i);

                    for (int m = 0; m < 2; m++)
                        for (int n = 0; n < 2; n++)
                        {
                            p = MathHelper.Pi * ((j + m) - precision / 2) / precision;
                            t = MathHelper.Pi * 2 * (i + n) / precision;
                            Point = new Vector3(radius * radiusRatioX * (float)(Math.Sin(t) * Math.Cos(p)),
                                                radius * radiusRatioY * (float)(Math.Cos(t)),
                                                radius * radiusRatioZ * (float)(Math.Sin(t) * Math.Sin(p)))
                                                + Center;

                            currentVertices[ind * 4 + n * 2 + m] = new VertexPositionNormalTexture
                                (Point, Vector3.Up,
                                (t / MathHelper.Pi < 1 || i == precision / 2 - 1) ? //!!!!!!!!
                                new Vector2((MathHelper.PiOver2 - p) / MathHelper.TwoPi, t / MathHelper.Pi) :
                                new Vector2((MathHelper.PiOver2 - p) / MathHelper.TwoPi + 0.5f, 2 - t / MathHelper.Pi));

                        }

                    lineIndices[ind * 8] = lineIndices[ind * 8 + 7] = (short)(ind * 4);
                    lineIndices[ind * 8 + 1] = lineIndices[ind * 8 + 2] = (short)(ind * 4 + 1);
                    lineIndices[ind * 8 + 6] = lineIndices[ind * 8 + 4] = (short)(ind * 4 + 2);
                    lineIndices[ind * 8 + 3] = lineIndices[ind * 8 + 5] = (short)(ind * 4 + 3);

                    if (i < precision / 2)
                    {
                        triangleIndices[ind * 6] = (short)(ind * 4);
                        triangleIndices[ind * 6 + 1] = triangleIndices[ind * 6 + 3] = (short)(ind * 4 + 2);
                        triangleIndices[ind * 6 + 2] = triangleIndices[ind * 6 + 5] = (short)(ind * 4 + 1);
                        triangleIndices[ind * 6 + 4] = (short)(ind * 4 + 3);
                    }
                    else
                    {
                        triangleIndices[ind * 6] = (short)(ind * 4);
                        triangleIndices[ind * 6 + 2] = triangleIndices[ind * 6 + 4] = (short)(ind * 4 + 2);
                        triangleIndices[ind * 6 + 1] = triangleIndices[ind * 6 + 5] = (short)(ind * 4 + 1);
                        triangleIndices[ind * 6 + 3] = (short)(ind * 4 + 3);
                    }
                }
            }

            Array.Copy(currentVertices, startVertices, startVertices.Length);
        }


        /// <summary>
        /// Sets up an ellipse.
        /// </summary>
        /// <param name="cent">The center of the ellipse.</param>
        /// <param name="radius">The radius of the ellipse.</param>
        /// <param name="radiusRatio">The coefficient of stretching along the X axis.</param>
        /// <param name="precision">A factor which influences the precision with that the ellipse is drawn. The recommended value for Windows Phone 7 is 10-14.</param>
        /// <param name="texture">The texture that is drawn to the surface of the ellipse.</param>
        public Ellipsoid(Vector3 cent, float radius, float radiusRatioX, float radiusRatioY, float radiusRatioZ, int precision, Texture2D texture) :
            this(cent, radius, radiusRatioX, radiusRatioY, radiusRatioZ, precision)
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
        /// <param name="radiusRatioZ">The coefficient of stretching along the Z axis.</param>
        /// <param name="precision">A factor which influences the precision with that the ellipse is drawn. The recommended value for Windows Phone 7 is 10-14.</param>
        /// <param name="texture">The texture that is drawn to the surface of the ellipse.</param>
        /// <param name="graphicsDevice">The graphics device to which the ellipse is drawn.</param>
        public Ellipsoid(Vector3 cent, float radius, float radiusRatioX, float radiusRatioY, float radiusRatioZ, int precision, Texture2D texture, GraphicsDevice graphicsDevice) :
            this(cent, radius, radiusRatioX, radiusRatioY, radiusRatioZ, precision, texture)
        {
            GraphicsDevice = graphicsDevice;
        }
        #endregion
    }
}
