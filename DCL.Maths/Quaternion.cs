using System;
using Microsoft.Xna.Framework;

namespace DCL.Maths
{
    public struct Quaternion
    {
        #region Fields
        //Accessing fields is much faster than properties (!)

        /// <summary>
        /// The scalar (real) part of the quaternion.
        /// </summary>
        public float Re;

        /// <summary>
        /// The i factor of the quaternion.
        /// </summary>
        public float X;

        /// <summary>
        /// The j factor of the quaternion.
        /// </summary>
        public float Y;

        /// <summary>
        /// The k factor of the quaternion.
        /// </summary>
        public float Z; 
        #endregion

        #region Constants
            //Quaternions that represent the X, Y and Z axes
            public static readonly Quaternion i = new Quaternion(0, 1, 0, 0);
            public static readonly Quaternion j = new Quaternion(0, 0, 1, 0);
            public static readonly Quaternion k = new Quaternion(0, 0, 0, 1);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the vector part of the quaternion as an instance of the Vector3 Structure (used in WP7)
        /// </summary>
        public Vector3 VectorPart
        {
            get { return new Vector3(X, Y, Z); }
        }

        /// <summary>
        /// The magnitude (absolute value) of the quaternion
        /// </summary>
        public float Abs
        {
            get { return (float)Math.Sqrt(Re * Re + X * X + Y * Y + Z * Z); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a quaternion, which has only the scalar (real) part
        /// </summary>
        /// <param name="RealNumber">A real number of single precision</param>
        public Quaternion(float RealNumber)
        {
            Re = RealNumber;
            X = 0;
            Y = 0;
            Z = 0;
        }
        /// <summary>
        /// Creates a quaternion, which has only the vector part
        /// </summary>
        /// <param name="v">An instance of the Vector3 (used in WP7) structure</param>
        public Quaternion(Vector3 v)
        {
            Re = 0;
            X = v.X;
            Y = v.Y;
            Z = v.Z;
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
            this.Re = Re;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the conjugate of the quaternion
        /// </summary>
        public Quaternion Conjugate()
        {
            return new Quaternion(Re, -X, -Y, -Z);
        }
        /// <summary>
        /// Returns the inverse of the quaternion
        /// </summary>
        public Quaternion Reciprocal()
        {
            if (Re == 0 && X == 0 && Y == 0 && Z == 0)
                throw new DivideByZeroException("Trying to find the reciprocal of a zero number");

            Quaternion temp = this.Conjugate();
            float Abs2 = Re * Re + X * X + Y * Y + Z * Z;
            temp.Re /= Abs2;
            temp.X /= Abs2;
            temp.Y /= Abs2;
            temp.Z /= Abs2;

            return temp;
        }
        /// <summary>
        /// Returns the normalized (with the magnitude of 1) quaternion
        /// </summary>
        public Quaternion Normalize()
        {
            float a = Abs;
            return new Quaternion(Re / a, X / a, Y / a, Z / a);
        }
        /// <summary>
        /// Rounds the quaternion, so that each component has the specified amount of signs after dot
        /// </summary>
        /// <param name="precision">The amount of signs after dot</param>
        public Quaternion Round(int precision)
        {
            return new Quaternion((float)Common.Round(Re, precision), (float)Common.Round(X, precision),
                                (float)Common.Round(Y, precision), (float)Common.Round(Z, precision));
        }
        /// <summary>
        /// Approximates each component of the quaternion to the specified precision
        /// </summary>
        /// <param name="precision"></param>
        /// <returns></returns>
        public Quaternion Approximate(float precision)
        {
            float k = 1 / precision;
            return (this * k).Round(0) / k;
        }
        /// <summary>
        /// Calculates the rotation quaternion from the axis and the rotation angle
        /// </summary>
        /// <param name="Axis">The quaternion that represents axis; it may be denormalized</param>
        /// <param name="Angle">The angle of rotation in radians</param>
        public static Quaternion RotationQuaternion(Quaternion Axis, float Angle)
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
        public static Quaternion Rotate(Quaternion Source, Quaternion Axis, float Angle)
        {
            Quaternion qRot = Quaternion.RotationQuaternion(Axis, Angle);
            return (qRot * Source * qRot.Conjugate());
        }
        /// <summary>
        /// Rotates the vector around the translated axis by the specified angle.
        /// </summary>
        /// <param name="Source">The quaternion that represents the vector to be rotated</param>
        /// <param name="Axis">The quaternion that represents axis; it may be denormalized</param>
        /// <param name="Angle">The angle of rotation in radians</param>
        /// <param name="AxisTranslation">The translation of the axis</param>
        public static Quaternion Rotate(Quaternion Source, Quaternion Axis, Quaternion AxisTranslation, float Angle)
        {
            Quaternion qTemp = Rotate(Source - AxisTranslation, Axis, Angle);
            qTemp += AxisTranslation;
            return qTemp;
        }
       
        /// <summary>
        /// Makes two rotations of a vector by once.
        /// </summary>
        /// <param name="Source">The quaternion that represents the vector to be rotated</param>
        /// <param name="Axis1">The first axis</param>
        /// <param name="Angle1">The angle of the first rotation in radians</param>
        /// <param name="Axis2">The second axis</param>
        /// <param name="Angle2">The angle of the second rotation in radians</param>
        public static Quaternion RotateComposition(Quaternion Source, Quaternion Axis1, float Angle1, Quaternion Axis2, float Angle2)
        {
            Quaternion q1 = RotationQuaternion(Axis1, Angle1);
            Quaternion q2 = RotationQuaternion(Axis2, Angle2);
            Quaternion qComb = q2 * q1;

            return qComb * Source  * qComb.Conjugate();
            
        }

        public static Quaternion Slerp(Quaternion qStart, Quaternion qEnd, float t)
        {
            float CosOm = qStart.X * qEnd.X + qStart.Y * qEnd.Y + qStart.Z * qEnd.Z + qStart.Re * qEnd.Re;
            float Om = (float)Math.Acos(CosOm);

            float Rsqrt = 1 - CosOm * CosOm; //for 1/SinOm

            union u = new union();
            float xhalf = 0.5f * Rsqrt;
            u.asFloat = Rsqrt;
            u.asInt = 0x5f3759df - (u.asInt >> 1);
            Rsqrt = u.asFloat * (1.5f - xhalf * u.asFloat * u.asFloat);

            return ((float)Math.Sin((1 - t) * Om) * qStart + (float)Math.Sin(t * Om) * qEnd) * Rsqrt;
        }


        public static Quaternion LSlerp(Quaternion qStart, Quaternion qEnd, float t)
        {
            float CosA = qStart.X * qEnd.X + qStart.Y * qEnd.Y + qStart.Z * qEnd.Z + qStart.Re * qEnd.Re;
            t *= (0.5069269f - CosA * (0.7987229f + 0.5069269f * CosA)) * (t * (2 * t - 3) + 1) + 1;

            Quaternion qRes = qStart + t * (qEnd - qStart);

            float Rsqrt = qRes.X * qRes.X + qRes.Y * qRes.Y + qRes.Z * qRes.Z + qRes.Re * qRes.Re;
            
            union u = new union();
			float xhalf = 0.5f * Rsqrt;
			u.asFloat = Rsqrt;
			u.asInt = 0x5f3759df - (u.asInt >> 1);
			Rsqrt = u.asFloat * (1.5f - xhalf * u.asFloat * u.asFloat);

            return qRes * Rsqrt;
        }

        public static Quaternion Lerp(Quaternion qStart, Quaternion qEnd, float t)
        {
            Quaternion qRes = qStart + t * (qEnd - qStart);

            float Rsqrt = qRes.X * qRes.X + qRes.Y * qRes.Y + qRes.Z * qRes.Z + qRes.Re * qRes.Re;

            union u = new union();
            float xhalf = 0.5f * Rsqrt;
            u.asFloat = Rsqrt;
            u.asInt = 0x5f3759df - (u.asInt >> 1);
            Rsqrt = u.asFloat * (1.5f - xhalf * u.asFloat * u.asFloat);

            return qRes * Rsqrt;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct union
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public int asInt; //32bit

            [System.Runtime.InteropServices.FieldOffset(0)]
            public float asFloat; //32bit
        }
        #endregion

        #region Operators
        public static Quaternion operator +(Quaternion l, Quaternion r)
        {
            return new Quaternion(l.Re + r.Re, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
        }
        public static Quaternion operator +(Quaternion l, Vector3 r)
        {
            return new Quaternion(l.Re, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
        }
        public static Quaternion operator +(Vector3 l, Quaternion r)
        {
            return new Quaternion(r.Re, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
        }
        public static Quaternion operator +(Quaternion l, float r)
        {
            l.Re += r;
            return l;
        }
        public static Quaternion operator +(float l, Quaternion r)
        {
            r.Re += l;
            return r;
        }

        public static Quaternion operator -(Quaternion l, Quaternion r)
        {
            return new Quaternion(l.Re - r.Re, l.X - r.X, l.Y - r.Y, l.Z - r.Z);
        }
        public static Quaternion operator -(Quaternion l, Vector3 r)
        {
            return new Quaternion(l.Re, l.X - r.X, l.Y - r.Y, l.Z - r.Z);
        }
        public static Quaternion operator -(Quaternion l, float r)
        {
            l.Re -= r;
            return l;
        }

        public static Quaternion operator +(Quaternion q)
        {
            return q;
        }
        public static Quaternion operator -(Quaternion q)
        {
            return new Quaternion(-q.Re, -q.X, -q.Y, -q.Z);
        }

        public static Quaternion operator *(Quaternion l, Quaternion r)
        {
            return new Quaternion(l.Re * r.Re - l.X * r.X - l.Y * r.Y - l.Z * r.Z,
                                    l.Re * r.X + l.X * r.Re + l.Y * r.Z - l.Z * r.Y,
                                    l.Re * r.Y + l.Y * r.Re + l.Z * r.X - l.X * r.Z,
                                    l.Re * r.Z + l.Z * r.Re + l.X * r.Y - l.Y * r.X);
        }
        public static Quaternion operator *(Quaternion l, Vector3 r)
        {
            return new Quaternion(-l.X * r.X - l.Y * r.Y - l.Z * r.Z,
                                    l.Re * r.X + l.Y * r.Z - l.Z * r.Y,
                                    l.Re * r.Y + l.Z * r.X - l.X * r.Z,
                                    l.Re * r.Z + l.X * r.Y - l.Y * r.X);
        }
        public static Quaternion operator *(Vector3 l, Quaternion r)
        {
            return new Quaternion(-l.X * r.X - l.Y * r.Y - l.Z * r.Z,
                                    l.X * r.Re + l.Y * r.Z - l.Z * r.Y,
                                    l.Y * r.Re + l.Z * r.X - l.X * r.Z,
                                    l.Z * r.Re + l.X * r.Y - l.Y * r.X);
        }
        public static Quaternion operator *(float l, Quaternion r)
        {
            return new Quaternion(l * r.Re, l * r.X, l * r.Y, l * r.Z);
        }
        public static Quaternion operator *(Quaternion l, float r)
        {
            return new Quaternion(l.Re * r, l.X * r, l.Y * r, l.Z * r);
        }
        public static Quaternion operator /(Quaternion l, float r)
        {
            return new Quaternion(l.Re / r, l.X / r, l.Y / r, l.Z / r);
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
            return new Vector3(q.X, q.Y, q.Z);
        }
        #endregion

        #region Overridden methods
        /// <summary>
        /// Returns the string representation of the quaternion
        /// </summary>
        public override string ToString()
        {
            string fmt = "";
            if (Re > 0)
                fmt += Re.ToString()+" ";
            else if (Re < 0)
                fmt += ("- " + (-Re).ToString()) + " ";

            if (X > 0)
                fmt += String.Format("{0}{1}i ", (fmt=="") ? "" : "+ ", (X == 1) ? "" : X.ToString());
            else if (X < 0)
                fmt += String.Format("- {0}i ", (X == -1) ? "" : (-X).ToString());

            if (Y > 0)
                fmt += String.Format("{0}{1}j ", (fmt == "") ? "" : "+ ", (Y == 1) ? "" : Y.ToString());
            else if (Y < 0)
                fmt += String.Format("- {0}j ", (Y == -1) ? "" : (-Y).ToString());

            if (Z > 0)
                fmt += String.Format("{0}{1}k", (fmt == "") ? "" : "+ ", (Z == 1) ? "" : Z.ToString());
            else if (Z < 0)
                fmt += String.Format("- {0}k", (Z == -1) ? "" : (-Z).ToString());

            if (fmt == "") fmt = "0";

            return fmt;
        }

        /// <summary>
        /// Returns the string representation of the quaternion with specified precision
        /// </summary>
        public string ToString(int Precision)
        {
            string fmt = "";
            if (Re > 0)
                fmt += Common.Round(Re, Precision).ToString() + " ";
            else if (Re < 0)
                fmt += ("- " + (-Common.Round(Re, Precision)).ToString()) + " ";

            if (X > 0)
                fmt += String.Format("{0}{1}i ", (fmt == "") ? "" : "+ ", (X == 1) ? "" : Common.Round(X, Precision).ToString());
            else if (X < 0)
                fmt += String.Format("- {0}i ", (X == -1) ? "" : (-Common.Round(X, Precision)).ToString());

            if (Y > 0)
                fmt += String.Format("{0}{1}j ", (fmt == "") ? "" : "+ ", (Y == 1) ? "" : Common.Round(Y, Precision).ToString());
            else if (Y < 0)
                fmt += String.Format("- {0}j ", (Y == -1) ? "" : (-Common.Round(Y, Precision)).ToString());

            if (Z > 0)
                fmt += String.Format("{0}{1}k", (fmt == "") ? "" : "+ ", (Z == 1) ? "" : Common.Round(Z, Precision).ToString());
            else if (Z < 0)
                fmt += String.Format("- {0}k", (Z == -1) ? "" : (-Common.Round(Z, Precision)).ToString());

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
            return (int)Re ^ (int)X ^ (int)Y ^ (int)Z;
        }
        #endregion
    }
}