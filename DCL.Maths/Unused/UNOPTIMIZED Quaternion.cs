using System;
using Microsoft.Xna.Framework;

namespace DCL.Maths
{
    public struct Quaternion
    {
        #region Fields
            //4 Factors (Real numbers of single precision)
            float fRe, fX, fY, fZ;
        #endregion

        #region Constants
            //Quaternions that represent the X, Y and Z axes
            public static readonly Quaternion i = new Quaternion(0, 1, 0, 0);
            public static readonly Quaternion j = new Quaternion(0, 0, 1, 0);
            public static readonly Quaternion k = new Quaternion(0, 0, 0, 1);
        #endregion

        #region Properties
        /****4 Factors (Real numbers of single precision)****/
        /// <summary>
        /// Gets or sets the scalar (real) part of the quaternion
        /// </summary>
        public float Re
        {
            set { fRe = value; }
            get { return fRe; }
        }
        /// <summary>
        /// Gets or sets the i factor of the quaternion
        /// </summary>
        public float X
        {
            set { fX = value; }
            get { return fX; }
        }
        /// <summary>
        /// Gets or sets the j factor of the quaternion
        /// </summary>
        public float Y
        {
            set { fY = value; }
            get { return fY; }
        }
        /// <summary>
        /// Gets or sets the k factor of the quaternion
        /// </summary>
        public float Z
        {
            set { fZ = value; }
            get { return fZ; }
        }

        /// <summary>
        /// Gets the vector part of the quaternion as an instance of the Vector3 Structure (used in WP7)
        /// </summary>
        public Vector3 VectorPart
        {
            get { return new Vector3((float)X, (float)Y, (float)Z); }
        }

        /// <summary>
        /// The magnitude (absolute value) of the quaternion
        /// </summary>
        public float Abs
        {
            get { return (float)Math.Sqrt(fRe * fRe + fX * fX + fY * fY + fZ * fZ); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a quaternion, which has only the scalar (real) part
        /// </summary>
        /// <param name="RealNumber">A real number of single precision</param>
        public Quaternion(float RealNumber)
        {
            fRe = RealNumber;
            fX = 0;
            fY = 0;
            fZ = 0;
        }
        /// <summary>
        /// Creates a quaternion, which has only the vector part
        /// </summary>
        /// <param name="v">An instance of the Vector3 (used in WP7) structure</param>
        public Quaternion(Vector3 v)
        {
            fRe = 0;
            fX = v.X;
            fY = v.Y;
            fZ = v.Z;
        }
        /// <summary>
        /// Creates a quaternion by setting all the four factors
        /// </summary>
        /// <param name="Re">The scalar (real) part of the quaternion</param>
        /// <param name="X">The i factor</param>
        /// <param name="Y">The i factor</param>
        /// <param name="Z">The i factor</param>
        public Quaternion(float Re, float X, float Y, float Z)
        {
            fRe = Re;
            fX = X;
            fY = Y;
            fZ = Z;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the conjugate of the quaternion
        /// </summary>
        public Quaternion Conjugate()
        {
            return new Quaternion(fRe, -fX, -fY, -fZ);
        }
        /// <summary>
        /// Returns the inverse of the quaternion
        /// </summary>
        public Quaternion Reciprocal()
        {
            if (fRe == 0 && fX == 0 && fY == 0 && fZ == 0)
                throw new DivideByZeroException("Trying to find the reciprocal of a zero number");

            Quaternion temp = this.Conjugate();
            temp.Re /= (fRe * fRe + fX * fX + fY * fY + fZ * fZ);
            temp.X /= (fRe * fRe + fX * fX + fY * fY + fZ * fZ);
            temp.Y /= (fRe * fRe + fX * fX + fY * fY + fZ * fZ);
            temp.Z /= (fRe * fRe + fX * fX + fY * fY + fZ * fZ);

            return temp;
        }
        /// <summary>
        /// Returns the normalized (with the magnitude of 1) quaternion
        /// </summary>
        public Quaternion Normalize()
        {
            float a = Abs;
            return new Quaternion(fRe / a, fX / a, fY / a, fZ / a);
        }
        /// <summary>
        /// Calculates the rotation quaternion from the axis and the rotation angle
        /// </summary>
        /// <param name="Axis">The quaternion that represents axis; it may be denormalized</param>
        /// <param name="Angle">The angle of rotation in radians</param>
        public static Quaternion RotationQuaternion(Quaternion Axis, double Angle)
        {
            Quaternion normAxis = Axis.Normalize();
            float sin = (float)Math.Sin(Angle/2);
            return new Quaternion((float)Math.Cos(Angle / 2),
                                    sin * normAxis.X,
                                    sin * normAxis.Y,
                                    sin * normAxis.Z);
        }
        /// <summary>
        /// Rotates the vector around the axis by the specified angle
        /// </summary>
        /// <param name="Source">The quaternion that represents the vector to be rotated</param>
        /// <param name="Axis">The quaternion that represents axis; it may be denormalized</param>
        /// <param name="Angle">The angle of rotation in radians</param>
        public static Quaternion Rotate(Quaternion Source, Quaternion Axis, double Angle)
        {
            Quaternion qRot = Quaternion.RotationQuaternion(Axis, Angle);
            return (qRot * Source * qRot.Conjugate());
        }
        #endregion

        #region Operators
        public static Quaternion operator +(Quaternion l, Quaternion r)
        {
            return new Quaternion(l.Re + r.Re, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
        }
        public static Quaternion operator -(Quaternion l, Quaternion r)
        {
            return new Quaternion(l.Re - r.Re, l.X - r.X, l.Y - r.Y, l.Z - r.Z);
        }
        public static Quaternion operator *(Quaternion l, Quaternion r)
        {
            return new Quaternion(l.Re * r.Re - l.X * r.X - l.Y * r.Y - l.Z * r.Z,
                                    l.Re * r.X + l.X * r.Re + l.Y * r.Z - l.Z * r.Y,
                                    l.Re * r.Y + l.Y * r.Re + l.Z * r.X - l.X * r.Z,
                                    l.Re * r.Z + l.Z * r.Re + l.X * r.Y - l.Y * r.X);
        }
        public static Quaternion operator *(float l, Quaternion r)
        {
            return new Quaternion(l * r.Re, l * r.X, l * r.Y, l * r.Z);
        }
        public static Quaternion operator *(Quaternion l, float r)
        {
            return new Quaternion(l.Re * r, l.X * r, l.Y * r, l.Z * r);
        }
        //public static Quaternion operator /(Quaternion l, Quaternion r)
        //{
               //two variants?
        //}
        public static bool operator ==(Quaternion l, Quaternion r)
        {
            return (l.Re == r.Re && l.X == r.X && l.Y == r.Y && l.Z == r.Z);
        }
        public static bool operator !=(Quaternion l, Quaternion r)
        {
            return !(l == r);
        }
        #endregion

        #region Type casting
        //The quaternion can be created implicity from a vector or from a real number
        public static implicit operator Quaternion(float d)
        {
            return new Quaternion(d);
        }
        public static implicit operator Quaternion(Vector3 v)
        {
            return new Quaternion(0, v.X, v.Y, v.Z);
        }
        //An explicit convert from the quaternion to a vector; the real-part may be non-zero
        public static explicit operator Vector3(Quaternion q)
        {
            return new Vector3(q.fX, q.fY, q.fZ);
        }
        #endregion

        #region Overridden methods
        /// <summary>
        /// Returns the string representation of the quaternion
        /// </summary>
        public override string ToString()
        {
            string fmt = "";
            if (fRe > 0)
                fmt += fRe.ToString()+" ";
            else if (fRe < 0)
                fmt += ("- " + (-fRe).ToString()) + " ";

            if (fX > 0)
                fmt += String.Format("{0}{1}i ", (fmt=="") ? "" : "+ ", (fX == 1) ? "" : fX.ToString());
            else if (fX < 0)
                fmt += String.Format("- {0}i ", (fX == -1) ? "" : (-fX).ToString());

            if (fY > 0)
                fmt += String.Format("{0}{1}j ", (fmt == "") ? "" : "+ ", (fY == 1) ? "" : fY.ToString());
            else if (fY < 0)
                fmt += String.Format("- {0}j ", (fY == -1) ? "" : (-fY).ToString());

            if (fZ > 0)
                fmt += String.Format("{0}{1}k", (fmt == "") ? "" : "+ ", (fZ == 1) ? "" : fZ.ToString());
            else if (fZ < 0)
                fmt += String.Format("- {0}k", (fZ == -1) ? "" : (-fZ).ToString());

            if (fmt == "") fmt = "0";

            return fmt;
        }

        /// <summary>
        /// Returns the string representation of the quaternion with specified precision
        /// </summary>
        public string ToString(int Precision)
        {
            string fmt = "";
            if (fRe > 0)
                fmt += Common.Round(fRe, Precision).ToString() + " ";
            else if (fRe < 0)
                fmt += ("- " + (-Common.Round(fRe, Precision)).ToString()) + " ";

            if (fX > 0)
                fmt += String.Format("{0}{1}i ", (fmt == "") ? "" : "+ ", (fX == 1) ? "" : Common.Round(fX, Precision).ToString());
            else if (fX < 0)
                fmt += String.Format("- {0}i ", (fX == -1) ? "" : (-Common.Round(fX, Precision)).ToString());

            if (fY > 0)
                fmt += String.Format("{0}{1}j ", (fmt == "") ? "" : "+ ", (fY == 1) ? "" : Common.Round(fY, Precision).ToString());
            else if (fY < 0)
                fmt += String.Format("- {0}j ", (fY == -1) ? "" : (-Common.Round(fY, Precision)).ToString());

            if (fZ > 0)
                fmt += String.Format("{0}{1}k", (fmt == "") ? "" : "+ ", (fZ == 1) ? "" : Common.Round(fZ, Precision).ToString());
            else if (fZ < 0)
                fmt += String.Format("- {0}k", (fZ == -1) ? "" : (-Common.Round(fZ, Precision)).ToString());

            if (fmt == "") fmt = "0";

            return fmt;
        }

        /// <summary>
        /// Defines if the current instance of quaternion is equal to the parameter
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Quaternion && this == (Quaternion)obj;
        }

        /// <summary>
        /// Returns the hash code of the current instance of quaternion
        /// </summary>
        public override int GetHashCode()
        {
            return (int)fRe ^ (int)fX ^ (int)fY ^ (int)fZ;
        }
        #endregion
    }
}