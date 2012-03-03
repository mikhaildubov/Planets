using System;

namespace DCL.Maths
{
    public struct Angle: IComparable
    {
        //Данная реализация структуры градусов
        //представляет собой как бы "обертку"
        //над структурой Fraction
        internal Fraction val;

        #region Constants
        public static readonly Angle PI = new Angle(180,1);
        public static readonly Angle Eps = new Angle(0,0,1, 1);
        #endregion

        #region Properties
        public int Sign
        {
            get
            {
                if (val < 0) return -1;
                if (val > 0) return 1;
                return 0;
            }
        }

        public uint Degrees
        {
            set
            {
                val = value + new Fraction(Minutes, 60) + new Fraction(Seconds, 3600);
            }
            get { return (uint)Math.Abs((double)val.ReduceToDenominator(3600)); }
        }
        public uint Minutes
        {
            set
            {
                if(value>60) throw new ArgumentOutOfRangeException("Minutes");
                
                val = Degrees + new Fraction(value, 60) + new Fraction(Seconds, 3600);
            }
            get { return (uint)(Math.Abs((val.ReduceToDenominator(3600).Numerator) % 3600) / 60); }
        }
        public uint Seconds
        {
            set
            {
                if (value > 60) throw new ArgumentOutOfRangeException("Minutes");

                val = Degrees + new Fraction(Minutes, 60) + new Fraction(value, 3600);
            }
            get { return (uint)(Math.Abs(val.ReduceToDenominator(3600).Numerator) % 60); }
        }
        #endregion

        #region Constructors
        public Angle(uint degrees, int sign)
        {
            val = degrees;
            if (sign < 0) val = -val;
        }
        public Angle(uint degrees, uint minutes, uint seconds, int sign)
        {
            val = degrees + new Fraction(minutes, 60) + new Fraction(seconds, 3600);
            if (sign < 0) val = -val;
        }
        private Angle(Fraction f)
        {
            val = f;
        }
        #endregion

        #region Operators
        public static Angle operator +(Angle l, Angle r)
        {
            Fraction res = (l.val + r.val);
            return new Angle(res);
        }
        public static Angle operator -(Angle l, Angle r)
        {
            Fraction res = (l.val - r.val);
            return new Angle(res);
        }
        public static Angle operator *(Angle l, Angle r)
        {
            return (Angle)((double)l * (double)r);
        }
        public static Angle operator *(Angle l, int r)
        {
            Fraction res = l.val * r;
            return new Angle(res);
        }
        public static Angle operator *(int l, Angle r)
        {
            return r * l;
        }
        public static Angle operator /(Angle l, Angle r)
        {
            return (Angle)((double)l / (double)r);
        }
        public static Angle operator /(Angle l, int r)
        {
            Fraction res = l.val / r;
            return new Angle(res);
        }
        public static Angle operator +(Angle a)
        {
            return new Angle(a.val);
        }
        public static Angle operator -(Angle a)
        {
            return new Angle(-a.val);
        }
        public static Angle operator ++(Angle a)
        {
            return new Angle(a.val + 1);
        }
        public static Angle operator --(Angle a)
        {
            return new Angle(a.val - 1);
        }
        public static bool operator >(Angle l, Angle r)
        {
            return (l.val > r.val);
        }
        public static bool operator >=(Angle l, Angle r)
        {
            return (l.val >= r.val);
        }
        public static bool operator <(Angle l, Angle r)
        {
            return (l.val < r.val);
        }
        public static bool operator <=(Angle l, Angle r)
        {
            return (l.val <= r.val);
        }
        public static bool operator ==(Angle l, Angle r)
        {
            return l.val == r.val;
        }
        public static bool operator !=(Angle l, Angle r)
        {
            return !(l == r);
        }
        #endregion

        #region Type casting
        public static explicit operator double(Angle a)
        {
            return (double)a.val;
        }
        public static explicit operator Angle(double d)
        {
            return new Angle((Fraction)d);
        }
        public static explicit operator Fraction(Angle a)
        {
            return a.val;
        }
        public static explicit operator Angle(Fraction f)
        {
            return new Angle(f);
        }
        #endregion

        #region Methods
        public static Angle Parse(string str)
        {
            str = str.Trim();
            uint deg, min=0, sec=0;
            int sign=1;

            if (str.IndexOf('°') < 0)
                throw new ArgumentException();

            if (str[0] == '-')
            {
                sign = -1;
                str = str.Substring(1);
            }

            deg = UInt32.Parse(str.Substring(0, str.IndexOf('°')));

            if (str.IndexOf('\'') > 0)
            {
                str = str.Substring(str.IndexOf('°') + 1);
                min = UInt32.Parse(str.Substring(0, str.IndexOf('\'')));
            }
            if (str.IndexOf('\"') > 0)
            {
                str = str.Substring(str.IndexOf('\'') + 1);
                min = UInt32.Parse(str.Substring(0, str.IndexOf('\"')));
            }

            return new Angle(deg, min, sec, sign);
        }
        public static bool TryParse(string str, out Angle a)
        {
            try
            {
                a = Parse(str);
                return true;
            }
            catch
            {
                a = (Angle)0;
                return false;
            }
        }
        public static Angle Wrap(Angle a)
        {
            if(a.Sign>0)
                while (a.Degrees > 180)
                    a -= new Angle(360, 1);
            else
                while (a.Degrees > 180)
                    a += new Angle(360, 1);

            return a;
        }
        #endregion

        #region Overridden methods
        public override bool Equals(object obj)
        {
            return (obj is Angle) && (this == (Angle)obj);
        }

        public override int GetHashCode()
        {
            return ((double)this).GetHashCode();
        }
        public override string ToString()
        {
            if (Minutes == 0 && Seconds == 0)
                return String.Format("{0}{1}°", (val < 0)?"-":"", Degrees);
            else if(Seconds==0)
                return String.Format("{0}{1}° {2}\'", (val < 0) ? "-" : "", Degrees, Minutes);
            else
                return String.Format("{0}{1}° {2}\' {3}\"", (val < 0) ? "-" : "", Degrees, Minutes, Seconds);
        }
        public string ToString(bool AlwaysPrintMinutes, bool AlwaysPrintSeconds)
        {
            string str = String.Format("{0}{1}°", (val < 0) ? "-" : "", Degrees);

            if(Minutes!=0 || AlwaysPrintMinutes || Seconds!=0)
                str += String.Format(" {0}\'", Minutes);

            if (Seconds != 0 || AlwaysPrintSeconds)
                str += String.Format(" {0}\"", Seconds);

            return str;
        }
        #endregion

        #region IComparable interface realization
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (!(obj is Angle)) throw new ArgumentException();

            if (this > (Angle)obj) return 1;
            else if (this < (Angle)obj) return -1;
            else return 0;
        }
        #endregion
    }
}
