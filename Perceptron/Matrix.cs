using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerceptronLib
{
    public class Matrix
    {
        private double[][] array;
        private int rows;
        private int cols;

        private void allocArray(int r, int c)
        {
            array = new double[r][];
            for (int i = 0; i < r; i++)
            {
                array[i] = new double[c];
                for (int j = 0; j < c; j++)
                {
                    array[i][j] = -1; 
                }
            }
        }

        public Matrix(int rowsNumber, int columnsNumber)
        {
            allocArray(rowsNumber, columnsNumber);
            rows = rowsNumber;
            cols = columnsNumber;
        }

        public double this[int row, int col]
        {
            get
            {
                return array[row][col];
            }
            set
            {
                array[row][col] = value;
            }
        }

        public static explicit operator Vector(Matrix m)
        {
            Vector v = new Vector(m.cols * m.rows);

            for (int i = 0; i < m.rows; i++)
            {
                for (int j = 0; j < m.cols; j++)
                {
                    v[i * m.cols + j] = m[i, j];
                }
            }

            return v;
        }

        public static explicit operator Matrix(Vector v)
        {
            int dim = (int)Math.Sqrt(v.Dimension);
            Matrix m = new Matrix(dim, dim);
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    m[i, j] = v[i * dim + j];
                }
            }
            return m;
        }

        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    res += array[i][j].ToString() + " ";
                }
                res += "\n";
            }

            return res;
        }
    }
}
