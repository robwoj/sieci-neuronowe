using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerceptronLib
{
    delegate double perceptronOutputFunction(Vector input);

    public enum PerceptronOutputFunctionType
    {
        /// <summary>
        /// Progowa funkcja wyjścia
        /// </summary>
        BiasFunction,

        /// <summary>
        /// Sigmoidalna funkcja wyjścia
        /// </summary>
        SigmoidalFunction
    }

    /// <summary>
    /// Klasa reprezentująca perceptron
    /// </summary>
    public class Perceptron
    {
        /// <summary>
        /// Wektor wag
        /// </summary>
        private Vector weights;

        //private Vector lastWeights;

        //public Vector LastWeights
        //{
        //    get
        //    {
        //        return lastWeights;
        //    }
        //}

        /// <summary>
        /// Tworzy nowy perceptron na podstawie zadanego wektora wag
        /// </summary>
        public Perceptron(Vector weightVector)
        {
            if (weightVector == null)
            {
                throw new NullReferenceException("Przekazany wektor wag jest niezainicjowany");
            }

            weights = weightVector;
            outputFunctionDelegate = biasFunction;
        }

        /// <summary>
        /// Konstruktor
        /// Tworzy wektor wag na podstawie wartości losowych 0.0 - 1.0
        /// </summary>
        public Perceptron(int dimension, int seed = 0)
        {
            weights = new Vector(dimension);
            Random r = seed == 0 ? new Random() : new Random(seed);
            for (int i = 0; i < weights.Dimension; i++)
            {
                weights[i] = r.NextDouble();
            }
            outputFunctionDelegate = biasFunction;

            //weights.round();
            //lastWeights = weights;
        }

        /// <summary>
        /// Przeciążenie przeznaczone do drukowania wektora
        /// </summary>
        public override string ToString()
        {
            return weights.ToString();
        }

        /// <summary>
        /// Zwraca wektor wag
        /// </summary>
        public Vector Weights
        {
            get
            {
                return weights;
            }
        }

        /// <summary>
        /// Algorytm uczenia kieszonkowego z zapadką
        /// </summary>
        /// <param name="iterations">
        /// Maksymalna liczba iteracji
        /// </param>
        /// <param name="learnConstant">
        /// Stała uczenia
        /// </param>
        /// <param name="examples">
        /// Lista przykładów uczących
        /// </param>
        /// <returns>
        /// Wektor wag nauczonego perceptronu
        /// </returns>
        public Vector pocketLearn(int iterations, double learnConstant, List<LearningExample> examples)
        {
            // Wagi rekordzisty
            Vector master = weights;

            // Czas życia rekordzisty
            int masterAlive = 0;

            // Czas życia obecnego wektora wag
            int weightsAlive = 0;

            // Liczba przykładów, które są klasyfikowane przez rekordzistę
            int masterExCount = 0;

            int count = examples.Count;
            if (count == 0)
            {
                return weights;
            }
            Random r = new Random(DateTime.Now.Millisecond);
            //System.Windows.MessageBox.Show("Przed iteracjami, " + examples[0].Example.Dimension
            //    + " " + weights.Dimension);

            // Ustawienie biasu jako zerowego elementu wymaga ujemnej wartości
            weights[0] = -weights[0];

            // Iteracja do momentu osiągnięcia maksymalnej liczby iteracji lub do momentu aż 
            // zostaną sklasyfikowane poprawnie wszystkie przykłady uczące
            for (int i = 0; i < iterations && checkPerceptron(examples) != count; i++)
            {
                // Losuje kolejny przykład z listy
                LearningExample example = examples[r.Next(count)];

                // Oblicza błąd
                double error = example.ExpectedDoubleValue - outputFunction(example.Example);

                if (error == 0)
                {
                    // Zwiększa czas życia obecnej wagi
                    weightsAlive++;
                    if (weightsAlive > masterAlive)
                    {
                        if (checkPerceptron(examples) > masterExCount)
                        {
                            // Zmienia obecnego rekordzistę
                            masterAlive = weightsAlive;
                            masterExCount = checkPerceptron(examples);
                        }
                    }
                }
                else
                {
                    //weights[0] += error;

                    // Zmienia wektor wag wg wzoru
                    for (int j = 0; j < weights.Dimension; j++)
                    {
                        weights[j] += learnConstant * error * example.Example[j];
                    }

                    // Zeruje czas życia obecnego wektora wag
                    weightsAlive = 0;
                }
            }

            // Zwraca rekordzistę
            return master;
        }

        /// <summary>
        /// Funkcja przeznaczona do sprawdzenia poprawności działania perceptronu.
        /// </summary>
        /// <returns>
        /// Liczba poprawnie klasyfikowanych przykładów
        /// </returns>
        private int checkPerceptron(List<LearningExample> examples)
        {
            int good = 0;
            foreach (LearningExample e in examples)
            {

                // Jeśli wartość oczekiwana jest równa wartości obliczonej, zwiększa licznik
                if(e.ExpectedDoubleValue == outputFunction(e.Example)) good++;
            }

            return good;
        }

        /// <summary>
        /// Funkcja klasyfikująca perceptronu
        /// </summary>
        /// <param name="input">
        /// Wektor wejściowy
        /// </param>
        /// <returns>
        /// Klasyfikacja
        /// </returns>
        public double outputFunction(Vector input)
        {
            if (outputFunctionDelegate != null)
            {
                return outputFunctionDelegate(input);
            }
            else
            {
                return biasFunction(input);
            }
        }

        /// <summary>
        /// Delegacja przeznaczona do wywołania funkcji wyjścia
        /// </summary>
        private perceptronOutputFunction outputFunctionDelegate;

        /// <summary>
        /// Progowa funkcja wyjścia
        /// </summary>
        private double biasFunction(Vector input)
        {
            if (input * weights > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Sigmoidalna funkcja wyjścia
        /// </summary>
        private double sigmoidalFunction(Vector input)
        {
            //Console.WriteLine((beta * (input * weights)).ToString());
            //if (1 / (1 + Math.Exp(-beta * (input * weights))) <= 0.00001
            //    || 1 / (1 + Math.Exp(-beta * (input * weights))) >= 0.99999)
            //    throw new Exception("Iloczyn bliski zeru");
            return 1 / (1 + Math.Exp(-(input * weights)));
        }

        /// <summary>
        /// Funkcja przeznaczona do zmiany typu funkcji wyjścia perceptronu
        /// </summary>
        public void setOutputFunctionType(PerceptronOutputFunctionType type)
        {
            switch (type)
            {
                case PerceptronOutputFunctionType.BiasFunction:
                    {
                        outputFunctionDelegate = biasFunction;
                    } break;
                case PerceptronOutputFunctionType.SigmoidalFunction:
                    {
                        outputFunctionDelegate = sigmoidalFunction;
                    } break;
            }
        }

        /// <summary>
        /// Zwraca wymiar wektora
        /// </summary>
        public int Dimension
        {
            get
            {
                return weights.Dimension;
            }
        }
    }

}
