using System;
using System.Collections.Generic;
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
using My=DCL.Maths;

namespace DCL.Phone.Xna
{
    /// <summary>
    /// The enumeration of ways in which the shape can be drawn.
    /// </summary>
    public enum DrawMode
    {
        /// <summary>
        /// Draws the outline of the shape only.
        /// </summary>
        Lines = 0x001,

        /// <summary>
        /// Draws the shape's surface.
        /// </summary>
        Solid = 0x010,

        /// <summary>
        /// Draws both the surface and the outline of the shape.
        /// </summary>
        SolidWithLines = 0x011,     // = Solid | Lines

        /// <summary>
        /// Draws the texture on the shape's surface.
        /// </summary>
        Textured = 0x100,
        //TexturedWithLines = 0x101   // = Textured | Lines
    }

    /// <summary>
    /// An abstract class that represents a three-dimensional shape and has methods
    /// to draw and rotate it.
    /// </summary>
    public abstract class Shape
    {
        #region Fields
        /// <summary>
        /// The array that stores the shape's vertices after the transformations occured.
        /// These vertices are used while drawing the shape.
        /// </summary>
        protected VertexPositionNormalTexture[] currentVertices;
        /// <summary>
        /// The array that stores the shape's initial vertices.
        /// Calling Reset() copies the values from this array to currentVertices.
        /// </summary>
        protected VertexPositionNormalTexture[] startVertices;

        /// <summary>
        /// The triangle indices array that is used when drawing in solid and texture modes.
        /// </summary>
        protected short[] triangleIndices;
        /// <summary>
        /// The line indices array that is used when drawing in line mode.
        /// </summary>
        protected short[] lineIndices;

        /// <summary>
        /// The initial center of the shape.
        /// Calling Reset() copies this value to the Center property.
        /// </summary>
        protected Vector3 startCenter;

        private My.Quaternion qDefaultRotation = My.Quaternion.RotationQuaternion(My.Quaternion.j, 0.05f);
        private My.Quaternion qDefaultRotationConj = My.Quaternion.RotationQuaternion(My.Quaternion.j, 0.05f).Conjugate();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the center point of the shape.
        /// </summary>
        public Vector3 Center { get; protected set; }

        /// <summary>
        /// Gets or sets the texture that is drawn to the surface of shape
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets the graphics device that is responsible for drawing the shape
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; set; }


        /// <summary>
        /// Gets or sets the quaternion that represents the default rotation for the shape.
        /// This quaternion is used in the RotateDefault() method.
        /// </summary>
        protected My.Quaternion DefaultRotation
        {
            get { return qDefaultRotation; }
            set
            {
                qDefaultRotation = value;
                qDefaultRotationConj = value.Conjugate();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor that initializes the center with zero vector.
        /// </summary>
        protected Shape()
        {
            Center = startCenter = Vector3.Zero;
        }
        #endregion

        #region Non-static methods
        /// <summary>
        /// Returns the shape to its initial state.
        /// </summary>
        public void Reset()
        {
            Array.Copy(startVertices, currentVertices, currentVertices.Length);
            Center = startCenter;
        }

        /// <summary>
        /// Rotates the shape around an axis over a specified angle. The rotation is based on quaternions.
        /// </summary>
        /// <param name="Axis">A quaternion or a Vector3 object that represents the axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle">The angle of rotation.</param>
        public void Rotate(My.Quaternion Axis, float RotationAngle)
        {
            if (RotationAngle == 0) return;
            
            My.Quaternion qRot = My.Quaternion.RotationQuaternion(Axis, RotationAngle);
            My.Quaternion qRotConj = qRot.Conjugate();
            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = new VertexPositionNormalTexture(
                    (Vector3)(qRot * currentVertices[i].Position * qRotConj),
                        Vector3.Up, currentVertices[i].TextureCoordinate);
            }
            Center = (Vector3)(qRot * Center * qRotConj);
        }

        /// <summary>
        /// Rotates the shape around an axis over a specified angle. The rotation is based on quaternions.
        /// </summary>
        /// <param name="Axis">A quaternion or a Vector3 object that represents the axis.</param>
        /// <param name="Translation">A quaternion or a Vector3 object that represents the point at which the axis starts.</param>
        /// <param name="RotationAngle">The angle of rotation.</param>
        public void Rotate(My.Quaternion Axis, My.Quaternion Translation, float RotationAngle)
        {
            if (RotationAngle == 0) return;
            
            My.Quaternion qRot = My.Quaternion.RotationQuaternion(Axis, RotationAngle);
            My.Quaternion qRotConj = qRot.Conjugate();
            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = new VertexPositionNormalTexture(
                    (Vector3)(qRot * (currentVertices[i].Position - Translation) * qRotConj + Translation),
                        Vector3.Up, currentVertices[i].TextureCoordinate);
            }
            Center = (Vector3)(qRot*(Center - Translation)*qRotConj + Translation);
        }

        /// <summary>
        /// Rotates the shape around two axises over two specified angles. The rotation is based on quaternions.
        /// </summary>
        /// <param name="Axis1">A quaternion or a Vector3 object that represents the first axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle1">The angle of rotation around the first axis.</param>
        /// <param name="Axis2">A quaternion or a Vector3 object that represents the second axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle2">The angle of rotation around the second axis.</param>
        public void RotateComposition(My.Quaternion Axis1, float RotationAngle1,
                                    My.Quaternion Axis2, float RotationAngle2)
        {
            My.Quaternion qRot1 = My.Quaternion.RotationQuaternion(Axis1, RotationAngle1);
            My.Quaternion qRot2 = My.Quaternion.RotationQuaternion(Axis2, RotationAngle2);
            My.Quaternion qComb = qRot2 * qRot1;
            My.Quaternion qCombConj = qComb.Conjugate();

            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = new VertexPositionNormalTexture(
                    (Vector3)(qComb * currentVertices[i].Position * qCombConj),
                        Vector3.Up, currentVertices[i].TextureCoordinate);
            }
            Center = (Vector3)(qComb*Center*qCombConj);
        }

        /// <summary>
        /// Rotates the shape around two axises over two specified angles. The rotation is based on quaternions.
        /// </summary>
        /// <param name="Axis1">A quaternion or a Vector3 object that represents the first axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle1">The angle of rotation around the first axis.</param>
        /// <param name="Axis2">A quaternion or a Vector3 object that represents the second axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle2">The angle of rotation around the second axis.</param>
        /// <param name="Axis3">A quaternion or a Vector3 object that represents the third axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle3">The angle of rotation around the third axis.</param>
        public void RotateComposition(My.Quaternion Axis1, float RotationAngle1,
                                    My.Quaternion Axis2, float RotationAngle2,
                                    My.Quaternion Axis3, float RotationAngle3)
        {
            My.Quaternion qRot1 = My.Quaternion.RotationQuaternion(Axis1, RotationAngle1);
            My.Quaternion qRot2 = My.Quaternion.RotationQuaternion(Axis2, RotationAngle2);
            My.Quaternion qRot3 = My.Quaternion.RotationQuaternion(Axis3, RotationAngle3);
            My.Quaternion qComb = qRot3 * qRot2 * qRot1; //Quaternion's multiplication is accotiative
            My.Quaternion qCombConj = qComb.Conjugate();

            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = new VertexPositionNormalTexture(
                    (Vector3)(qComb * currentVertices[i].Position * qCombConj),
                        Vector3.Up, currentVertices[i].TextureCoordinate);
            }
            Center = (Vector3)(qComb * Center * qCombConj);
        }

        /// <summary>
        /// Performs the default rotation, which can be changed through the SetDefaultRotation method.
        /// Is preferable in cases when the axises and angles don't change each frame.
        /// </summary>
        public void RotateDefault()
        {
            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = new VertexPositionNormalTexture(
                    (Vector3)(qDefaultRotation * currentVertices[i].Position * qDefaultRotationConj),
                        Vector3.Up, currentVertices[i].TextureCoordinate);
            }
            Center = (Vector3)(qDefaultRotation * Center * qDefaultRotationConj);
        }

        /// <summary>
        /// Sets the default rotation for the RotateDefault() method.
        /// </summary>
        /// <param name="Axis">A quaternion or a Vector3 object that represents the axis.</param>
        /// <param name="RotationAngle">The angle of rotation around the axis.</param>
        public void SetDefaultRotation(My.Quaternion Axis, float RotationAngle)
        {
            DefaultRotation = My.Quaternion.RotationQuaternion(Axis, RotationAngle);
        }

        /// <summary>
        /// Sets the default rotation for the RotateDefault() method.
        /// </summary>
        /// <param name="Axis1">A quaternion or a Vector3 object that represents the first axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle1">The angle of rotation around the first axis.</param>
        /// <param name="Axis2">A quaternion or a Vector3 object that represents the second axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle2">The angle of rotation around the second axis.</param>
        public void SetDefaultRotation(My.Quaternion Axis1, float RotationAngle1,
                                        My.Quaternion Axis2, float RotationAngle2)
        {
            My.Quaternion qRot1 = My.Quaternion.RotationQuaternion(Axis1, RotationAngle1);
            My.Quaternion qRot2 = My.Quaternion.RotationQuaternion(Axis2, RotationAngle2);

            DefaultRotation = qRot2 * qRot1;
        }

        /// <summary>
        /// Sets the default rotation for the RotateDefault() method.
        /// </summary>
        /// <param name="Axis1">A quaternion or a Vector3 object that represents the first axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle1">The angle of rotation around the first axis.</param>
        /// <param name="Axis2">A quaternion or a Vector3 object that represents the second axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle2">The angle of rotation around the second axis.</param>
        /// <param name="Axis3">A quaternion or a Vector3 object that represents the third axis. It is assumed that the axis starts at the origin.</param>
        /// <param name="RotationAngle3">The angle of rotation around the third axis.</param>
        public void SetDefaultRotation(My.Quaternion Axis1, float RotationAngle1,
                                        My.Quaternion Axis2, float RotationAngle2,
                                        My.Quaternion Axis3, float RotationAngle3)
        {
            My.Quaternion qRot1 = My.Quaternion.RotationQuaternion(Axis1, RotationAngle1);
            My.Quaternion qRot2 = My.Quaternion.RotationQuaternion(Axis2, RotationAngle2);
            My.Quaternion qRot3 = My.Quaternion.RotationQuaternion(Axis3, RotationAngle3);

            DefaultRotation = qRot3 * qRot2 * qRot1;
        }

        /// <summary>
        /// Translates the shape over a specified vector.
        /// </summary>
        /// <param name="Translation">A quaternion or a Vector3 object that represents the translation.</param>
        public void Translate(My.Quaternion Translation)
        {
            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = new VertexPositionNormalTexture(
                    (Vector3)(currentVertices[i].Position + Translation),
                        Vector3.Up, currentVertices[i].TextureCoordinate);
            }
            Center = (Vector3)(Center + Translation);
        }

        /// <summary>
        /// Scales the shape.
        /// </summary>
        /// <param name="scale">the scaling coefficient.</param>
        public void Scale(float scale)
        {
            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = new VertexPositionNormalTexture(
                    (Vector3)((currentVertices[i].Position - Center) * scale + Center),
                        Vector3.Up, currentVertices[i].TextureCoordinate);
            }

        }

        /// <summary>
        /// Draws the shape to the specified GraphicsDevice.
        /// </summary>
        /// <param name="basicEffect">The effect of drawing.</param>
        /// <param name="mode">The drawing mode.</param>
        public void Draw(BasicEffect basicEffect, DrawMode mode)
        {
            basicEffect.TextureEnabled = ((mode & DrawMode.Textured) > 0);
            if (basicEffect.TextureEnabled)
                basicEffect.Texture = Texture;

            basicEffect.CurrentTechnique.Passes[0].Apply();

            if ((mode & DrawMode.Textured) > 0
                || (mode & DrawMode.Solid) > 0)
            {
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, currentVertices, 0, currentVertices.Length, triangleIndices, 0, triangleIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }

            if ((mode & DrawMode.Lines) > 0)
            {
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.LineList, currentVertices, 0, currentVertices.Length, lineIndices, 0, lineIndices.Length / 2, VertexPositionNormalTexture.VertexDeclaration);
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Draws a set of shapes to the specified GraphicsDevice.
        /// The assumtion is that the camera is placed on the positive half of the Z axis.
        /// </summary>
        /// <param name="effect">The effect of drawing.</param>
        /// <param name="mode">The drawing mode.</param>
        /// <param name="shapes">The shapes to draw. The order they come is not important.</param>
        public static void DrawScene(BasicEffect effect, DrawMode mode, params Shape[] shapes)
        {
            float[] CenterZs = new float[shapes.Length];
            for (int i = 0; i < shapes.Length; i++)
                CenterZs[i] = shapes[i].Center.Z;
            Array.Sort(CenterZs, shapes);
            for (int i = 0; i < shapes.Length; i++)
                shapes[i].Draw(effect, mode);
        }

        /// <summary>
        /// Draws a set of shapes to the specified GraphicsDevice.
        /// The assumtion is that the camera is placed on the positive half of the Z axis.
        /// </summary>
        /// <param name="effect">The effect of drawing.</param>
        /// <param name="mode">The drawing mode.</param>
        /// <param name="shapes">The shapes to draw. The order they come is not important.</param>
        /*public static void DrawShapes(BasicEffect effect, DrawMode mode, Shape[] shapes)
        {
            float[] CenterZs = new float[shapes.Length];
            for (int i = 0; i < shapes.Length; i++)
                CenterZs[i] = shapes[i].Center.Z;
            Array.Sort(CenterZs, shapes);
            for (int i = 0; i < shapes.Length; i++)
                shapes[i].Draw(effect, mode);
        }*/
        /// <summary>
        /// Draws a set of shapes to the specified GraphicsDevice.
        /// The assumtion is that the camera is placed on the positive half of the Z axis.
        /// </summary>
        /// <param name="effect">The effect of drawing.</param>
        /// <param name="mode">The drawing mode.</param>
        /// <param name="shapes1">The shapes to draw. The order they come is not important.</param>
        /// <param name="shapes2">The shapes to draw. The order they come is not important.</param>
        public static void DrawScene(BasicEffect effect, DrawMode mode, Shape[] shapes1, params Shape[] shapes2)
        {
            Shape[] shapes = new Shape[shapes1.Length+shapes2.Length];
            Array.Copy(shapes1, shapes, shapes1.Length);
            Array.Copy(shapes2, 0, shapes, shapes1.Length, shapes2.Length);
 
            float[] CenterZs = new float[shapes.Length];
            for (int i = 0; i < shapes.Length; i++)
                CenterZs[i] = shapes[i].Center.Z;
            Array.Sort(CenterZs, shapes);
            for (int i = 0; i < shapes.Length; i++)
                shapes[i].Draw(effect, mode);
        }
        #endregion
    }
}
