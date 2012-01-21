using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PerceptronLib
{
    /// <summary>
    /// Klasa reprezentująca wektor
    /// </summary>
    [Serializable]
    public class Vector : IEnumerable<double>
    {
        // Tablica double
        private double[] array;

        // Wymiar wektora
        private int dimension;

        public Vector(int dimension = 1)
        {
            if (dimension < 1)
            {
                throw new SizeException("Wymiar musi być większy lub równy 1");
            }

            this.dimension = dimension;

            // Inicjalizuje tablicę
            array = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                array[i] = 0;
            }
        }

        /// <summary>
        /// Konstruktor kopiujący
        /// </summary>
        public Vector(Vector v)
        {
            dimension = v.dimension;
            array = new double[dimension];
            for(int i = 0; i < dimension; i++)
            {
                array[i] = v.array[i];
            }
        }

        /// <summary>
        /// Konstruktor tworzący wektor na podstawie ogólnej kolekcji IEnumerable
        /// </summary>
        /// <param name="v"></param>
        public Vector(IEnumerable<double> v)
        {
            dimension = v.Count();
            array = new double[dimension];
            int i = 0;
            foreach (double d in v)
            {
                array[i] = d;
                i++;
            }
        }

        /// <summary>
        /// Zwraca wymiar wektora
        /// </summary>
        public int Dimension
        {
            get
            {
                return dimension;
            }
        }

        /// <summary>
        /// Funkcja przeznaczona do walidacji wymiarów wektorów
        /// </summary>
        private static void compareDimensions(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
            {
                throw new SizeException("Niezgodne wymiary wektorów: dim(a) = "
                    + a.dimension + ", dim(b) = " + b.dimension);
            }

        }

        /// <summary>
        /// Operator odpowiedzialny za obliczanie sumy wektorów
        /// </summary>
        /// <returns>
        /// Suma wektorów a i b
        /// </returns>
        public static Vector operator +(Vector a, Vector b)
        {
            compareDimensions(a, b);

            int dim = a.Dimension;
            Vector sum = new Vector(dim);

            for (int i = 0; i < dim; i++)
            {
                sum[i] = a[i] + b[i];
            }

            return sum;
        }

        /// <summary>
        /// Operator odpowiedzialny za obliczanie różnicy wektorów
        /// </summary>
        /// <returns>
        /// Różnica wektorów a i b
        /// </returns>
        public static Vector operator -(Vector a, Vector b)
        {
            compareDimensions(a, b);

            int dim = a.Dimension;
            Vector sum = new Vector(dim);

            for (int i = 0; i < dim; i++)
            {
                sum[i] = a[i] - b[i];
            }

            return sum;
        }

        /// <summary>
        /// Operator odpowiedzialny za obliczanie iloczynu skalarnego
        /// </summary>
        /// <returns>
        /// Iloczyn skalarny wektorów a i b
        /// </returns>
        public static double operator *(Vector a, Vector b)
        {
            compareDimensions(a, b);
            double sum = 0;
            int dim = a.Dimension;
            for (int i = 0; i < dim; i++)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }

        /// <summary>
        /// Operator mnożenia wektora przez skalar
        /// </summary>
        public static Vector operator *(Vector v, double scal)
        {
            Vector result = new Vector(v.dimension);
            for (int i = 0; i < v.dimension; i++)
            {
                result[i] = scal * v[i];
            }

            return result;
        }


        /// <summary>
        /// Operator mnożenia wektora przez skalar
        /// </summary>
        public static Vector operator *(double scal, Vector v)
        {
            return v * scal;
        }

        /// <summary>
        /// Funkcja przeznaczona do potęgowania wektora
        /// </summary>
        /// <param name="val">
        /// Wykładnik potęgi
        /// </param>
        /// <returns>
        /// Wektor podniesiony do potęgi val
        /// </returns>
        public Vector power(double val)
        {
            for (int i = 0; i < dimension; i++)
            {
                if (val != 2)
                {
                    array[i] = Math.Pow(array[i], val);
                }
                else
                {
                    array[i] = array[i] * array[i];
                }
            }

            return this;
        }

        /// <summary>
        /// Funkcja przeznaczona do wyzerowania wektora. Jest potrzebna, jeśli nie korzystamy z wekotra
        /// w typowy sposób, ponieważ wektor inicjowany jest wektorami losowymi.
        /// </summary>
        public void zeros()
        {
            for (int i = 0; i < dimension; i++)
            {
                array[i] = 0;
            }
        }

        /// <summary>
        /// Zwraca wektor zerowy o wymiarze zgodnym z tym wektorem
        /// </summary>
        public Vector Zero
        {
            get
            {
                Vector v = new Vector(dimension);
                v.zeros();
                return v;
            }
        }

        /// <summary>
        /// Operator indeksowania 
        /// </summary>
        public double this[int index]
        {
            get
            {
                return array[index];
            }
            set
            {
                array[index] = value;
            }
        }

        /// <summary>
        /// Przeciążenie przeznaczone do drukowania współrzędnych wektora
        /// </summary>
        public override string ToString()
        {
            string str = "[";
            for (int i = 0; i < dimension - 1; i++)
            {
                str += array[i].ToString() + ", ";
            }

            str += array[dimension - 1].ToString() + "]";

            return str;
        }

        /// <summary>
        /// Przeciżenie przeznaczone do rozróżniania wektorów.
        /// Wykorzystywane przez słownik używany podczas uczenia sieci
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Vector)
            {
                Vector v = (Vector)obj;

                try
                {
                    // Walidacja - jeśli wymmiary są niezgodne, nie ma sensu porównywanie 
                    // współrzędnych, wektory będą różne
                    compareDimensions(v, this);
                }
                catch (SizeException)
                {
                    return false;
                }
                // Wymiary zgodne


                // Porówanie wpółrzędnych
                for (int i = 0; i < dimension; i++)
                {
                    if (array[i] != v.array[i]) return false;
                }

                // Wszytkie współrzędne równe
                return true;
            }
            else
            {
                // Obiekt wejściowy nie jest wektorem, więc nie może być równy
                return false;
            }
        }


        /// <summary>
        /// Przeciążenie wymagane przez Equals
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public void round(int precision = 3)
        {
            for (int i = 0; i < dimension; i++)
            {
                array[i] = Math.Round(array[i], precision);
            }
        }

        public double Length
        {
            get
            {
                double sum = 0;
                for (int j = 0; j < Dimension; j++)
                {
                    sum += array[j] * array[j];
                }

                return Math.Sqrt(sum);
            }
        }

        public void normalizeWeights()
        {
            double len = Length;
            for (int i = 0; i < dimension; i++)
            {
                array[i] /= len;
            }
        }

        public IEnumerator<double> GetEnumerator()
        {
            List<double> array = new List<double>();
            return array.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }
    }
}
