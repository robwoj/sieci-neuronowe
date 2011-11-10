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
        private double expectedValue;

        public LearningExample(Vector example, double expectedValue)
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
        /// Zwraca wartość oczekiwaną
        /// </summary>
        public double ExpectedValue
        {
            get
            {
                return expectedValue;
            }
        }
    }
}
