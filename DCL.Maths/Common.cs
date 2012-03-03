using System;

namespace DCL.Maths
{
    /// <summary>
    /// A static class that contains several common mathematical methods
    /// and extends the functionality of Math.Round.
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Rounds a number in mathematical rules.
        /// Unlike Math.Round, Common.Round(34.45, 1) returns 34.5 instead of 34.4.
        /// </summary>
        /// <param name="number">A fractional number to be rounded.</param>
        /// <param name="precision">The number of decimal places in the return value.</param>
        /// <returns>A double value.</returns>
        public static double Round(double number, int precision)
        {
            double temp = Math.Pow(10, precision);
            return Math.Floor(number * temp + 0.5) / temp;
        }

        /// <summary>
        /// Rounds a number in mathematical rules.
        /// Unlike Math.Round, Common.Round(34.5) returns 35 instead of 34.
        /// </summary>
        /// <param name="number">A fractional number to be rounded.</param>
        /// <returns>A double value.</returns>
        public static double Round(double number)
        {
            return Math.Floor(number+0.5);
        }

        /// <summary>
        /// Calculates the cube root of a number.
        /// </summary>
        /// <param name="x">The initial double value.</param>
        /// <returns>A double value.</returns>
        public static double Cbrt(double x)
        {
            double a1, //Previous value
                a2, //Current value
                eps; //Next value

            double standartPrecision = 1e-10;

            a2 = x;
            do
            {
                a1 = a2;
                eps = (x - a1 * a1 * a1) / (3 * a1 * a1);
                a2 = a1 + eps;
            } while (Math.Abs(eps)>standartPrecision); //till reaching the "machine null"
            return a2;
        }

        public static float InvSqrt(float x)
        {
            union u = new union();
            float xhalf = 0.5f * x;
            u.asFloat = x;
            u.asInt = 0x5f3759df - (u.asInt >> 1);
            x = u.asFloat * (1.5f - xhalf * u.asFloat * u.asFloat);

            return x;
        }


        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct union
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public int asInt; //32bit

            [System.Runtime.InteropServices.FieldOffset(0)]
            public float asFloat; //32bit
        }

        /// <summary>
        /// Calculates the factorial of a natural number.
        /// </summary>
        /// <param name="x">The initial natural value; the value can be zero.</param>
        /// <returns>A natural value.</returns>
        public static double Factorial(uint x)
        {
            //if (x > 65) throw new OverflowException("Too large!");
            if (x == 0) return 1;
            else
            {
                double res = 1;
                for (uint i = 2; i <= x; i++)
                    res *= i;
                return res;
            }
        }

        /// <summary>
        /// Calculates the double factorial of a natural number.
        /// </summary>
        /// <param name="x">The initial natural value; the value can be zero.</param>
        /// <returns>A natural value.</returns>
        public static ulong DoubleFactorial(uint x)
        {
            if (x == 0) return 1;
            else
            {
                ulong res = 1;
                for (uint i = (x%2==0)?2u:3u; i <= x; i+=2)
                    res *= i;
                return res;
            }
        }

        /// <summary>
        /// Calculates the least common multiple of two natural numbers.
        /// </summary>
        /// <param name="a">The first natural number.</param>
        /// <param name="b">The second natural number.</param>
        /// <returns>A natural value.</returns>
        public static uint LCM(uint a, uint b) //НОК
        {
            return a*b/GCD(a,b);
        }

        /*
        public static uint GCD_1(uint a, uint b) //НОД
        {
            if (a == 0 || b == 0) return Math.Max(a, b);
            while (a != b)
                if (a > b) a -= b;
                else b -= a;
            return a;
        }
        
        public static uint GCD_2(uint a, uint b) //Faster when handling large values
        {
            if (a == 0 || b == 0) return Math.Max(a, b);
            //if (a > b) Swap(ref a, ref b);

            uint r;
            while (b != 0)
            {
                r = a % b;
                a = b;
                b = r;
            }
            return a;
        }*/

        /// <summary>
        /// Calculates the greatest common divisor of two natural numbers.
        /// </summary>
        /// <param name="a">The first natural number.</param>
        /// <param name="b">The second natural number.</param>
        /// <returns>A natural value.</returns>
        public static uint GCD(uint a, uint b)
        {
            if (a == 0 || b == 0) return Math.Max(a, b);

            while (true)
            {
                a = a % b;
                if (a == 0) return b;
                b = b % a;
                if (b == 0) return a;
            }
        }
        

        /// <summary>
        /// Calculates the fractional part of a number.
        /// </summary>
        /// <param name="x">The initial fractional number.</param>
        /// <returns>A double value, the integral part of which is equal to zero.</returns>
        public static double FractionalPart(double x)
        {
            return (Math.Abs(x) - Math.Abs((long)x));
        }

        /// <summary>
        /// Calculates the fractional part of a number.
        /// </summary>
        /// <param name="x">The initial fractional number.</param>
        /// <returns>A decimal value, the integral part of which is equal to zero.</returns>
        public static decimal FractionalPart(decimal x)
        {
            return (Math.Abs(x) - Math.Abs((long)x));
        }

        /// <summary>
        /// Calculates the length of the fractional part of a number.
        /// </summary>
        /// <param name="x">The initial fractional number.</param>
        /// <returns>An integer value.</returns>
        public static int FractionalPartLength(double x)
        {
            return FractionalPartLength((decimal)x); //should pe precise!!
            /*int l = 0;
            while (FractionalPart(x) > 0)
            {
                x *= 10;
                l++;
            }
            return l;*/
        }

        /// <summary>
        /// Calculates the length of the fractional part of a number.
        /// </summary>
        /// <param name="x">The initial fractional number.</param>
        /// <returns>An integer value.</returns>
        public static int FractionalPartLength(decimal x)
        {
            int l = 0;
            while (FractionalPart(x) > 0)
            {
                x *= 10;
                l++;
            }
            return l;
        }

        /// <summary>
        /// Defines if an unsigned integer is a prime number.
        /// </summary>
        /// <param name="number">A number to be analyzed.</param>
        /// <returns>True if prime; false otherwise.</returns>
        public static bool IsPrime(uint number)
        {
            uint temp = (uint)Math.Sqrt(number);
            for (uint i = 2; i <= temp; i++)
                if (number % i == 0) return false;
            return true;
        }

        /// <summary>
        /// Defines if an unsigned integer is a prime number.
        /// </summary>
        /// <param name="number">A number to be analyzed.</param>
        /// <returns>True if prime; false otherwise.</returns>
        public static bool IsPrime(ulong number)
        {
            ulong temp = (ulong)Math.Sqrt(number);
            for (ulong i = 2; i <= temp; i++)
                if (number % i == 0) return false;
            return true;
        }

        /// <summary>
        /// Swaps the values of two variables.
        /// </summary>
        /// <param name="a">The first variable.</param>
        /// <param name="b">The second variable.</param>
        public static void Swap(ref uint a, ref uint b)
        {
            uint t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// Swaps the values of two variables.
        /// </summary>
        /// <param name="a">The first variable.</param>
        /// <param name="b">The second variable.</param>
        public static void Swap(ref int a, ref int b)
        {
            int t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// Calculates the binomial coeffitient (which is also the amount of k-combinations of n)
        /// </summary>
        public static double BinomialCoefficient(uint n, uint k)
        {
            uint max = Math.Max(k, n - k);
            uint min = Math.Min(k, n - k);

            double res = 1;

            while (n > max)
                res *= n--;

            if (Double.IsPositiveInfinity(res)) return Double.PositiveInfinity;

            double fact = Factorial(min);
            if (Double.IsPositiveInfinity(fact)) return Double.PositiveInfinity;

            res /= Factorial(min);

            return res;
        }
    }
}
