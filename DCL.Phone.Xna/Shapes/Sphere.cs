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
    /// Represents a sphere which is actually a many sided three-dimensional polygon.
    /// </summary>
    public class Sphere : Ellipsoid
    {
        #region Constructors
        /// <summary>
        /// Sets up a sphere.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="precision">A factor which influences the precision with that the sphere is drawn. The recommended value for Windows Phone 7 is 10-14.</param>
        public Sphere(Vector3 center, float radius, int precision): base(center, radius, 1, 1, 1, precision) {}

        /// <summary>
        /// Sets up a sphere.
        /// </summary>
        /// <param name="cent">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="precision">A factor which influences the precision with that the sphere is drawn. The recommended value for Windows Phone 7 is 10-14.</param>
        /// <param name="texture">The texture that is drawn to the surface of the sphere.</param>
        public Sphere(Vector3 center, float radius, int precision, Texture2D texture) :
            this(center, radius, precision)
        {
            Texture = texture;
        }

        /// <summary>
        /// Sets up a sphere.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="precision">A factor which influences the precision with that the sphere is drawn. The recommended value for Windows Phone 7 is 10-14.</param>
        /// <param name="texture">The texture that is drawn to the surface of the sphere.</param>
        /// <param name="graphicsDevice">The graphics device to which the sphere is drawn.</param>
        public Sphere(Vector3 center, float radius, int precision, Texture2D texture, GraphicsDevice graphicsDevice) :
            this(center, radius, precision, texture)
        {
            GraphicsDevice = graphicsDevice;
        }
        #endregion
    }
}