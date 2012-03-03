using System;

namespace DCL.Maths
{
    public struct Fraction: IComparable
    {
        #region Fields
        long num;
        uint den;
        #endregion

        #region Constants
        public static readonly Fraction Empty = new Fraction(long.MaxValue, uint.MaxValue);
        #endregion

        #region Properties
        public long Numerator
        {
            set { num = value; }
            get { return num; }
        }
        public uint Denominator
        {
            set
            {
                if (value == 0) throw new DivideByZeroException("denominator");
                den = value;
            }
            get { return den; }
        }
        #endregion

        #region Constructors
        public Fraction(long numerator, uint denominator)
        {
            num = numerator;
            if (denominator == 0) throw new DivideByZeroException("denominator");
            den = denominator;
        }
        #endregion

        #region Operators
        public static Fraction operator +(Fraction l, Fraction r)
        {
            if (l.Numerator == Empty.Numerator || l.Denominator == Empty.Denominator)
                if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                {
                    Fraction f = 0;
                    return Empty;
                }
                else
                    return r;
            if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                return l;

            uint CommonDenominator = Common.LCM(l.Denominator, r.Denominator);
            Fraction fr = new Fraction(l.Numerator * CommonDenominator / l.Denominator
                                        + r.Numerator * CommonDenominator / r.Denominator,
                                        CommonDenominator);
            return fr.Cancel();
        }
        public static Fraction operator -(Fraction l, Fraction r)
        {
            if (l.Numerator == Empty.Numerator || l.Denominator == Empty.Denominator)
                if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                    return Empty;
                else
                    return r;
            if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                return l;

            uint CommonDenominator = Common.LCM(l.Denominator, r.Denominator);
            Fraction fr = new Fraction(l.Numerator * CommonDenominator / l.Denominator
                                        - r.Numerator * CommonDenominator / r.Denominator,
                                        CommonDenominator);
            return fr.Cancel();
        }
        public static Fraction operator *(Fraction l, Fraction r)
        {
            if (l.Numerator == Empty.Numerator || l.Denominator == Empty.Denominator)
                if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                    return Empty;
                else
                    return r;
            if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                return l;

            Fraction fr = new Fraction(l.Numerator * r.Numerator,
                                        l.Denominator * r.Denominator);
            return fr.Cancel();
        }
        public static Fraction operator /(Fraction l, Fraction r)
        {
            return l*r.Reverse();
        }
        public static Fraction operator +(Fraction f)
        {
            return new Fraction(f.Numerator, f.Denominator);
        }
        public static Fraction operator -(Fraction f)
        {
            if (f.Numerator == Empty.Numerator || f.Denominator == Empty.Denominator)
                return f;

            return new Fraction(-f.Numerator, f.Denominator);
        }
        public static Fraction operator ++(Fraction f)
        {
            if (f.Numerator == Empty.Numerator || f.Denominator == Empty.Denominator)
                return f;

            return new Fraction(f.Numerator + f.Denominator, f.Denominator);
        }
        public static Fraction operator --(Fraction f)
        {
            if (f.Numerator == Empty.Numerator || f.Denominator == Empty.Denominator)
                return f;

            return new Fraction(f.Numerator - f.Denominator, f.Denominator);
        }
        public static bool operator ==(Fraction l, Fraction r)
        {
            return (l.Cancel().Numerator == r.Cancel().Numerator &&
                    l.Cancel().Denominator == r.Cancel().Denominator) ||
                    ((l.Numerator==Empty.Numerator || l.Denominator==Empty.Denominator) &&
                     (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator));
        }
        public static bool operator >(Fraction l, Fraction r)
        {
            if (l.Numerator == Empty.Numerator || l.Denominator == Empty.Denominator)
                return false;
            else if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                return true;

            if (r.Numerator == 0) return l.Numerator > 0;
            if (l.Numerator == 0) return r.Numerator < 0;

            uint CommonDenominator = Common.LCM(l.Denominator, r.Denominator);
            return l.Numerator * CommonDenominator / l.Denominator >
                    r.Numerator * CommonDenominator / r.Denominator;
        }
        public static bool operator >=(Fraction l, Fraction r)
        {
            if (l.Numerator == Empty.Numerator || l.Denominator == Empty.Denominator)
                if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                    return true;
                else
                    return false;
            if (r.Numerator == Empty.Numerator || r.Denominator == Empty.Denominator)
                return true;

            if (r.Numerator == 0) return l.Numerator >= 0;
            if (l.Numerator == 0) return r.Numerator <= 0;

            uint CommonDenominator = Common.LCM(l.Denominator, r.Denominator);
            return l.Numerator * CommonDenominator / l.Denominator >=
                    r.Numerator * CommonDenominator / r.Denominator;
        }
        public static bool operator <(Fraction l, Fraction r)
        {
            return r > l;
        }
        public static bool operator <=(Fraction l, Fraction r)
        {
            return r >= l;
        }
        public static bool operator !=(Fraction l, Fraction r)
        {
            return !(l == r);
        }
        #endregion

        #region Type casting
        public static implicit operator Fraction(long l)
        {
            return new Fraction(l,1);
        }
        public static explicit operator Fraction(double d)
        {
            if (d == 0) return new Fraction(0, 1);

            int l = Math.Min(Common.FractionalPartLength(d), 8);//int l = Math.Min(19 - (int)Math.Log10(Math.Abs(d)), 8);
            uint dn = (uint)Math.Pow(10, l);
            long nm = (long)(d * dn);
            Fraction fr = new Fraction(nm, dn);

            return fr;//fr.Cancel();
        }
        public static explicit operator double(Fraction f)
        {
            if (f.Numerator == Fraction.Empty.Numerator || f.Denominator == Fraction.Empty.Denominator)
                return 0;

            return (double)f.Numerator / f.Denominator;
        }
        #endregion

        #region Methods
        public Fraction Cancel()
        {
            uint t = Common.GCD((uint)Math.Abs(num), den);
            return new Fraction(num/t, den/t);
        }
        public Fraction Reverse()
        {
            if (num == 0) throw new DivideByZeroException("denominator");

            Fraction f = new Fraction();

            if (den == Empty.Denominator) f.Numerator = Empty.Numerator;
            else f.Numerator = (num < 0) ? -den : den;

            if (num == Empty.Numerator) f.Denominator = Empty.Denominator;
            else f.Denominator = (uint)Math.Abs(num);

            return f;
        }
        public Fraction ReduceToDenominator(uint newDenominator)
        {
            return new Fraction(Numerator * newDenominator / Denominator, newDenominator);
        }
        public void ToMixedNumber(out long WholePart, out Fraction FractionalPart)
        {
            if (this.Numerator == Empty.Numerator || this.Denominator == Empty.Denominator)
            {
                WholePart = 0;
                FractionalPart = Empty;
                return;
            }
            bool neg = this.Numerator < 0;
            WholePart = this.Numerator / this.Denominator;
            FractionalPart = (new Fraction(Math.Abs(this.Numerator) - Math.Abs(WholePart * this.Denominator), this.Denominator)).Cancel();
            if (WholePart == 0 && neg)
                FractionalPart.Numerator *= -1;
        }
        public static Fraction FromMixedNumber(long wholePart, Fraction fractionalPart)
        {
            if (fractionalPart.Numerator == Fraction.Empty.Numerator || fractionalPart.Denominator == Fraction.Empty.Denominator)
                return Fraction.Empty;

            bool neg = wholePart < 0 || fractionalPart.Numerator < 0;
            if (wholePart == 0 && neg) fractionalPart *= -1;
            if (wholePart < 0) wholePart *= -1;
            Fraction f = wholePart + fractionalPart;
            if (neg) f *= -1;
            return f;
        }
        public static Fraction Parse(string str)
        {
            long nm;
            uint dn;

            str = str.Trim();

            if (str.IndexOf('/') == -1) { nm = long.Parse(str); dn = 1; }
            else
            {
                nm = long.Parse(str.Substring(0, str.IndexOf('/')));
                dn = uint.Parse(str.Substring(str.IndexOf('/') + 1, str.Length - str.IndexOf('/')-1));
            }

            return new Fraction(nm, dn);
        }
        public static bool TryParse(string str, out Fraction f)
        {
            try
            {
                f = Parse(str);
                return true;
            }
            catch
            {
                f = new Fraction(0,1);
                return false;
            }
        }
        public static bool IsEmpty(Fraction f)
        {
            return f.Numerator == Fraction.Empty.Numerator || f.Denominator == Fraction.Empty.Denominator;
        }
        #endregion

        #region Overridden methods
        public override bool Equals(object obj)
        {
            return (obj is Fraction) && (this == (Fraction)obj);
        }

        public override int GetHashCode()
        {
            return (num/den).GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}/{1}",
                (num == Empty.Numerator) ? "" : num.ToString(),
                (den == Empty.Denominator) ? "" : den.ToString());
        }
        #endregion

        #region IComparable interface realization
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (!(obj is Fraction)) throw new ArgumentException();

            if (this > (Fraction)obj) return 1;
            else if (this < (Fraction)obj) return -1;
            else return 0;
        }
        #endregion
    }
}
