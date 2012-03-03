using System;

namespace DCL.Maths
{
    public struct Flexible: IComparable
    {
        #region Fields
        double value;
        #endregion

        #region Properties
        public double Value
        {
            set { this.value = value; }
            get { return value; }
        }

        public string this[int scale]
        {
            get
            {
                return ToString(scale);
            }
        }
        #endregion

        #region Constructors
        public Flexible(double value)
        {
            this.value = value;
        }
        #endregion

        #region Operators
        public static Flexible operator +(Flexible l, Flexible r)
        {
            return new Flexible(l.Value+r.Value);
        }
        public static Flexible operator -(Flexible l, Flexible r)
        {
            return new Flexible(l.Value - r.Value);
        }
        public static Flexible operator *(Flexible l, Flexible r)
        {
            return new Flexible(l.Value * r.Value);
        }
        public static Flexible operator /(Flexible l, Flexible r)
        {
            return new Flexible(l.Value / r.Value);
        }
        public static Flexible operator +(Flexible f)
        {
            return new Flexible(f.Value);
        }
        public static Flexible operator -(Flexible f)
        {
            return new Flexible(-f.Value);
        }
        public static Flexible operator ++(Flexible f)
        {
            return new Flexible(f.Value+1);
        }
        public static Flexible operator --(Flexible f)
        {
            return new Flexible(f.Value-1);
        }
        public static bool operator <(Flexible l, Flexible r)
        {
            return l.Value < r.Value;
        }
        public static bool operator >(Flexible l, Flexible r)
        {
            return l.Value > r.Value;
        }
        public static bool operator <=(Flexible l, Flexible r)
        {
            return l.Value <= r.Value;
        }
        public static bool operator >=(Flexible l, Flexible r)
        {
            return l.Value >= r.Value;
        }
        public static bool operator ==(Flexible l, Flexible r)
        {
            return l.Value==r.Value;
        }
        public static bool operator !=(Flexible l, Flexible r)
        {
            return !(l == r);
        }
        #endregion

        #region Type casting
        public static implicit operator Flexible(double d)
        {
            return new Flexible(d);
        }
        public static implicit operator double(Flexible f)
        {
            return f.Value;
        }
        #endregion

        #region Methods
        public static Flexible Parse(string str, int scale)
        {
            if(scale<2)
                throw new ArgumentOutOfRangeException("scale");
 
            double value = 0;
            bool negative = false;
            int indexer = 0;

            str = str.Replace('.', ',').Trim();

            if (str[0] == '-')
            {
                negative = true;
                indexer++;
            }
            else if (str[0] == '+') indexer++;

            int dotPos = str.IndexOf(',');
            char t;
            int dig;
            if (dotPos == -1)
                for (int i = indexer; i < str.Length; i++)
                {
                    t=str[i];
                    dig = Char.IsDigit(t) ? (t - '0') : Char.IsLetter(t) ? (Char.ToUpper(t) - 'A' + 10) : -1;
                    if (dig < 0 || dig >= scale)
                        throw new FormatException("Incorrect input string");
                    value += dig*Math.Pow(scale, str.Length - i - 1);
                }
            else
            {
                for (int i = indexer; i < dotPos; i++)
                {
                    t = str[i];
                    dig = Char.IsDigit(t) ? (t - '0') : (Char.ToUpper(t) - 'A' + 10);
                    if (dig < 0 || dig >= scale)
                        throw new FormatException("Incorrect input string");
                    value += dig * Math.Pow(scale, dotPos - i - 1);
                }
                for (int i = dotPos+1; i < str.Length; i++)
                {
                    t = str[i];
                    dig = Char.IsDigit(t) ? (t - '0') : (Char.ToUpper(t) - 'A' + 10);
                    if (dig < 0 || dig >= scale)
                        throw new FormatException("Incorrect input string");
                    value += dig * Math.Pow(scale, dotPos - i);
                }
            }

            if (negative) value *= -1;

            return value;
        }
        
        public static bool TryParse(string str, int scale, out Flexible fn)
        {
            try
            {
                fn = Parse(str, scale);
                return true;
            }
            catch
            {
                fn = 0;
                return false;
            }
        }
        #endregion

        #region Overridden methods
        public override bool Equals(object obj)
        {
            return (obj is Flexible) && (this==(Flexible)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public string ToString(int scale)
        {
            return ToString(scale, 10);
        }

        public string ToString(int scale, int Precision)
        {
            string temp = "";
            long integer = (long)this.value;
            double fractional = (double)Common.FractionalPart((decimal)this.value); //avoiding unprecisement
            int dig;
            bool negative=false;
            if(this.value<0){negative = true;integer*=-1;}

            if (integer == 0) temp = "0";
            while (integer != 0)
            {
                dig = (int)(integer % scale);
                temp=temp.Insert(0, (dig < 10) ? (dig.ToString()) : ((char)('A' + dig - 10)).ToString());
                integer /= scale;
            }

            if (fractional != 0)
            {
                temp += System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                while (fractional != 0 && Precision > 0)
                {
                    fractional *= scale;
                    dig = (int)fractional;
                    fractional = Common.Round(Common.FractionalPart(fractional), Precision);
                    temp += ((dig < 10) ? (dig.ToString()) : ((char)('A' + dig - 10)).ToString());
                    Precision--;
                }
            }

            if (negative) temp = temp.Insert(0, "-");

            return temp;
        }
        #endregion

        #region IComparable interface realization
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is double || obj is float || obj is ulong || obj is long || obj is uint
                 || obj is int || obj is ushort || obj is short || obj is byte || obj is sbyte)
            {
                if (this.Value > (double)obj) return 1;
                else if (this.Value < (double)obj) return -1;
                else return 0;
            }
            if (!(obj is Flexible)) throw new ArgumentException();

            if (this > (Flexible)obj) return 1;
            else if (this < (Flexible)obj) return -1;
            else return 0;
        }
        #endregion
    }
}
