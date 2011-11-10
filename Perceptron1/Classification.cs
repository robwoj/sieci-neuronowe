using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;

namespace Perceptron1
{

    /// <summary>
    /// Klasa reprezentująca klasyfikację przykładu w postaci wektora klasyfikacji
    /// perceptronów składowych warstwy
    /// </summary>
    class Classification
    {
        // Klasyfikacje kolejnych perceptronów
        private List<double> classifications;

        /// <summary>
        /// Przeciążenie odróżniające dwie klasyfikacje
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Classification)
            {
                Classification c = (Classification)obj;

                // Liczba składowych obu klasyfikacji musi się zgadzać
                if (classifications.Count != c.classifications.Count) return false;

                // Klasyfikacje muszą być równe na każdej współrzędnej
                for (int i = 0; i < classifications.Count; i++)
                {
                    if (classifications[i] != c.classifications[i]) return false;
                }

                return true;
            }
            else
            {
                // Jeśli argument nie jest obiektem Classification to porównanie nie ma sensu
                // czyli obiekty są różne
                return false;
            }
        }

        /// <summary>
        /// Przeciążenie przeznaczone do odróżniania przez słownik i wymagane 
        /// przez przeciążenie Equals
        /// </summary>
        /// <remarks>
        /// Zwraca jedynie sumę po współrzędnych. Wg definicji GetHashCode wymaga jedynie, 
        /// aby dla takich samych obiektów zwracała te same wartości. Dla sum ten warunek pozostaje
        /// spełniony
        /// </remarks>
        public override int GetHashCode()
        {
            double value = 0;
            foreach (double d in classifications)
            {
                value += d;
            }

            return (int)value;
        }

        /// <summary>
        /// Tworzy nową klasyfikację
        /// </summary>
        /// <param name="perceptrons">
        /// Lista perceptronów
        /// </param>
        /// <param name="example">
        /// Przykład
        /// </param>
        public Classification(List<Perceptron> perceptrons, LearningExample example)
        {
            classifications = new List<double>();
            LearningExample e = example;

            // Kolejne współrzędne są klasyfikacjami odpowiednich perceptronów
            foreach (Perceptron p in perceptrons)
            {
                classifications.Add(p.function(e.Example));
            }
        }

    }
}
