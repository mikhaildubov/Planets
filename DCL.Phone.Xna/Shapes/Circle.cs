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
    /// Represents a circle which is actually a many sided polygon.
    /// </summary>
    public class Circle : Ellipse
    {
        #region Constructors
        /// <summary>
        /// Sets up a circle.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="precision">A factor which influences the precision with that the circle is drawn. The value is actually the number of sides in the polygon.</param>
        public Circle(Vector3 center, float radius, int precision) : base(center, radius, 1, 1, precision){}

        /// <summary>
        /// Sets up a circle.
        /// </summary>
        /// <param name="cent">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="precision">A factor which influences the precision with that the circle is drawn. The value is actually the number of sides in the polygon.</param>
        /// <param name="texture">The texture that is drawn to the surface of the circle.</param>
        public Circle(Vector3 cent, float radius, int precision, Texture2D texture):
            this(cent, radius, precision)
        {
            Texture = texture;
        }

        /// <summary>
        /// Sets up a circle.
        /// </summary>
        /// <param name="cent">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="precision">A factor which influences the precision with that the circle is drawn. The value is actually the number of sides in the polygon.</param>
        /// <param name="texture">The texture that is drawn to the surface of the circle.</param>
        /// <param name="graphicsDevice">The graphics device to which the circle is drawn.</param>
        public Circle(Vector3 cent, float radius, int precision, Texture2D texture, GraphicsDevice graphicsDevice):
            this(cent, radius, precision, texture)
        {
            GraphicsDevice = graphicsDevice;
        }
        #endregion
    }
}
