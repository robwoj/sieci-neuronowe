using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceptron1
{
    /// <summary>
    /// Klasa reprezentująca przykład uczący
    /// </summary>
    public class LearningExample
    {
        // Wektor wejściowy
        private Vector v;

        // Wartość oczekiwana
        private Vector expectedValue;

        public LearningExample(Vector example, double expectedValue)
        {
            v = example;
            Vector expectedVector = new Vector();
            expectedVector[0] = expectedValue;
            this.expectedValue = expectedVector;
        }

        public LearningExample(Vector example, Vector expectedValue)
        {
            v = example;
            this.expectedValue = expectedValue;
        }

        /// <summary>
        /// Zwraca wektor wejściowy
        /// </summary>
        public Vector Example
        {
            get
            {
                return v;
            }
        }

        /// <summary>
        /// Zwraca wartość oczekiwaną jako liczbę typu double
        /// Ma to sens, jeśli oczekujemy jedynie wartości liczbowych jako wyniku
        /// </summary>
        public double ExpectedDoubleValue
        {
            get
            {
                return expectedValue[0];
            }
        }

        /// <summary>
        /// Zwraca wartość oczekiwaną przykładu w postaci wektora
        /// </summary>
        public Vector ExpectedValue
        {
            get
            {
                return expectedValue;
            }
        }
    }
}
