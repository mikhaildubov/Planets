/*using System;

namespace DCL.Maths
{
    public struct Matrix
    {
        #region Fields
        double[,] matr;
        #endregion

        #region Properties
        public int Rows
        {
            get { return matr.GetLength(0); }
        }
        public int Cols
        {
            get { return matr.GetLength(0); }
        }
        public int Degree
        {
            get
            {
                if (Rows == Cols) return Rows;
                else return -1;
            }
        }
        //Индексатор
        public double this[int i, int j]
        {
            set 
            {
                if (i > matr.GetLength(0) || j > matr.GetLength(0) || i<1 || j<1)
                    throw new IndexOutOfRangeException("Matrix index out of range");
                matr[i-1, j-1] = value; 
            }
            get 
            {
                if (i > matr.GetLength(0) || j > matr.GetLength(0) || i < 1 || j < 1)
                    throw new IndexOutOfRangeException("Matrix index out of range");
                return matr[i-1, j-1]; 
            }
        }
        #endregion

        #region Constructors
        public Matrix(int rows, int columns)
        {
            matr = new double[rows, columns];
        }
        public Matrix(double[,] m)
        {
            matr = new double[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < m.GetLength(0); i++)
                for (int j = 0; j < m.GetLength(1); j++)
                    matr[i, j] = m[i, j];
        }
        public Matrix(int[,] m)
        {
            matr = new double[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < m.GetLength(0); i++)
                for (int j = 0; j < m.GetLength(1); j++)
                    matr[i, j] = m[i, j];
        }
        #endregion

        #region Operators
        public static Matrix operator +(Matrix l, Matrix r)
        {
            if(l.Rows!=r.Rows || l.Cols!=r.Cols)
                throw new DimensionsDiscordanceException("Discordance of Dimensions (addition)");

            Matrix temp = new Matrix(l.Rows, l.Cols);
            for (int i = 1; i <= l.Rows; i++)
                for (int j = 1; j <= l.Cols; j++)
                    temp[i, j] = l[i, j]+r[i, j];
            return temp;
        }
        public static Matrix operator -(Matrix l, Matrix r)
        {
            if (l.Rows != r.Rows || l.Cols != r.Cols)
                throw new DimensionsDiscordanceException("Discordance of Dimensions (substraction)");

            Matrix temp = new Matrix(l.Rows, l.Cols);
            for (int i = 1; i <= l.Rows; i++)
                for (int j = 1; j <= l.Cols; j++)
                    temp[i, j] = l[i, j] - r[i, j];
            return temp;
        }
        public static Matrix operator *(double l, Matrix r)
        {
            Matrix temp = new Matrix(r.Rows, r.Cols);
            for (int i = 1; i <= r.Rows; i++)
                for (int j = 1; j <= r.Cols; j++)
                    temp[i, j] = l * r[i, j];
            return temp;
        }
        public static Matrix operator *(Matrix l, double r)
        {
            Matrix temp = new Matrix(l.Rows, l.Cols);
            for (int i = 1; i <= l.Rows; i++)
                for (int j = 1; j <= l.Cols; j++)
                    temp[i, j] = l[i, j] * r;
            return temp;
        }
        public static Matrix operator *(Matrix l, Matrix r)
        {
            if(l.Cols!=r.Rows)
                throw new DimensionsDiscordanceException("Discordance of Dimensions: the amount of columns in the left matrix must be equal to the amount of the rows in the right.");

            Matrix temp = new Matrix(l.Rows, r.Cols);

            double t;

            for (int i = 1; i <= l.Rows; i++)
                for (int j = 1; j <= r.Cols; j++)
                {
                    t=0.0;
                    for (int k = 1; k <= l.Cols; k++)
                        t+=l[i, k]*r[k, j];
                    temp[i,j] = t;
                }

            return temp;

        }
        #endregion

        #region Methods
        Matrix CutDown(int i, int j)
        {
            Matrix temp = new Matrix(this.Rows-1, this.Cols-1);

            for (int a = 1; a < i; a++)
            {
                for (int b = 1; b < j; b++)
                    temp[a, b] = this[a, b];
                for (int b = j+1; b <= this.Cols; b++)
                    temp[a, b-1] = this[a, b];
            }
            for (int a = i+1; a <= this.Rows; a++)
            {
                for (int b = 1; b < j; b++)
                    temp[a-1, b] = this[a, b];
                for (int b = j + 1; b <= this.Cols; b++)
                    temp[a-1, b - 1] = this[a, b];
            }

            return temp;
        }

        public double Minor(int i, int j)
        {
            if(i>this.Rows || j>this.Cols)
                throw new IndexOutOfRangeException("Matrix index out of range");

            return Det(CutDown(i, j));
        }

        public double Cofactor(int i, int j)
        {
            return ((i + j) % 2 == 0) ? (Minor(i, j)) : (-Minor(i, j));
        }

        public static double Det(Matrix m)
        {
            if (m.Degree < 0) throw new DimensionsDiscordanceException("A non-square matrix has no determinant.");
            if (m.Degree == 1) return m[1, 1];

            double sum = 0.0;

            for(int i = 1; i <= m.Cols; i++) //Разложение по i-й строке
                sum += m[i, 1]*m.Cofactor(i, 1); //Тут неявная рекурсия

            return sum;
        }
        #endregion

        #region Overridden methods
        public override string ToString()
        {
            string res="";

            for (int i = 1; i <= Rows; i++, res += "\n")
                for (int j = 1; j <= Cols; j++)
                    res += String.Format("{0,7}", Common.Round(this[i,j], 2));

            return res;
        }
        public string ToString(string format)
        {
            string res="";

            for (int i = 1; i <= Rows; i++, res += "\n")
                for (int j = 1; j <= Cols; j++)
                    res += String.Format("format", this[i, j]);

            return res;
        }
        #endregion
    }
}*/
