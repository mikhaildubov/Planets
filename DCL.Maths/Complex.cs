using System;

namespace DCL.Maths
{
    public struct Complex
    {
        #region Fields
        float fRe, fIm;
        #endregion

        #region Constants
        public static readonly Complex i = new Complex(0, 1);
        #endregion

        #region Properties
        public float Re
        {
            set { fRe = value; }
            get { return fRe; }
        }
        public float Im
        {
            set { fIm = value; }
            get { return fIm; }
        }
        public float Abs
        {
            get { return (float)Math.Sqrt(fRe * fRe + fIm * fIm); }
        }
        public float Arg
        {
            get { return (float)(Math.Acos(fRe/Abs) + Math.PI*Convert.ToInt32(fIm/Abs < 0)); } //Чтобы получить угол в [0; 2π]
        }
        #endregion

        #region Constructors
        public Complex(float RealNumber)
        {
            fRe = RealNumber;
            fIm = 0;
        }
        public Complex(float Re, float Im)
        {
            fRe = Re;
            fIm = Im;
        }
        #endregion

        #region Methods
        public Complex Conjugate()
        {
            return new Complex(fRe, -fIm);
        }
        public Complex Reciprocal()
        {
            if (fRe == 0 && fIm == 0)
                throw new DivideByZeroException("Trying to find the reciprocal of a zero number");

            Complex temp = this.Conjugate();
            temp.Re /= (fRe * fRe + fIm * fIm);
            temp.Im /= (fRe * fRe + fIm * fIm);

            return temp;
        }
        public Complex Normalize()
        {
            float a = Abs;
            return new Complex(fRe / a, fIm / a);
        }
        #endregion

        #region Overridden methods
        public override string ToString()
        {
            string fmt = "";
            if (fRe > 0)
                fmt += fRe.ToString()+" ";
            else if (fRe < 0)
                fmt += ("- " + (-fRe).ToString())+" ";

            if (fIm > 0)
                fmt += String.Format("{0}{1}i", (fmt == "") ? "" : "+ ", (fIm == 1) ? "" : fIm.ToString());
            else if (fIm < 0)
                fmt += String.Format("- {0}i", (fIm == -1) ? "" : (-fIm).ToString());

            if (fmt == "") fmt = "0";

            return fmt;
        }
        public string ToString(int Precision)
        {
            string fmt = "";
            if (fRe > 0)
                fmt += Common.Round(fRe, Precision).ToString() + " ";
            else if (fRe < 0)
                fmt += ("- " + (-Common.Round(fRe, Precision)).ToString()) + " ";

            if (fIm > 0)
                fmt += String.Format("{0}{1}i", (fmt == "") ? "" : "+ ", (fIm == 1) ? "" : Common.Round(fIm, Precision).ToString());
            else if (fIm < 0)
                fmt += String.Format("- {0}i", (fIm == -1) ? "" : (-Common.Round(fIm, Precision)).ToString());

            if (fmt == "") fmt = "0";

            return fmt;
        }
        public override bool Equals(object obj)
        {
            return obj is Complex && this == (Complex)obj;
        }
        public override int GetHashCode()
        {
            return (int)fRe ^ (int)fIm;
        }
        #endregion

        #region Operators
        public static Complex operator +(Complex l, Complex r)
        {
            return new Complex(l.Re+r.Re, l.Im+r.Im);
        }
        public static Complex operator -(Complex l, Complex r)
        {
            return new Complex(l.Re - r.Re, l.Im - r.Im);
        }
        public static Complex operator *(Complex l, Complex r)
        {
            return new Complex(l.Re*r.Re - l.Im*r.Im, l.Re*r.Im + l.Im*r.Re);
        }
        public static Complex operator *(float l, Complex r)
        {
            return new Complex(l * r.Re, l * r.Im);
        }
        public static Complex operator *(Complex l, float r)
        {
            return new Complex(l.Re * r, l.Im * r);
        }
        public static Complex operator /(Complex l, Complex r)
        {
            return new Complex((l.Re*r.Re + l.Im*r.Im)/(r.Re*r.Re + r.Im*r.Im), (l.Im*r.Re - l.Re*r.Im)/(r.Re*r.Re + r.Im*r.Im));
        }
        public static bool operator ==(Complex l, Complex r)
        {
            return (l.Re==r.Re && l.Im==r.Im);
        }
        public static bool operator !=(Complex l, Complex r)
        {
            return !(l==r);
        }
        #endregion

        #region Type casting
        public static implicit operator Complex(float d)
        {
            return new Complex(d);
        }
        #endregion
    }
}
