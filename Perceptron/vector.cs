﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerceptronLib
{
    /// <summary>
    /// Klasa reprezentująca wektor
    /// </summary>
    public class Vector
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
                throw new SizeException("Niezgodne wymiary wektorów");
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
        /// Operator dodawania wektorów. Wektory muszą mieć identyczne wymiary.
        /// </summary>
        public static Vector operator +(Vector a, Vector b)
        {
            compareDimensions(a, b);
            Vector sum = new Vector(a.Dimension);

            for (int i = 0; i < a.Dimension; i++)
            {
                sum[i] = a[i] + b[i];
            }

            return sum;
        }

        /// <summary>
        /// Operator odejmowania wektorów. Wektory muszą mieć identyczne wymiary.
        /// </summary>
        public static Vector operator -(Vector a, Vector b)
        {
            compareDimensions(a, b);
            Vector sum = new Vector(a.Dimension);

            for (int i = 0; i < a.Dimension; i++)
            {
                sum[i] = a[i] - b[i];
            }

            return sum;
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

    }
}
