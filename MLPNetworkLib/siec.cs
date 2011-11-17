using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using PerceptronLib;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("NetworkTests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("PropagacjaWsteczna")]

namespace MLPNetworkLib
{
    /// <summary>
    /// Sieć perceptronów złożona z warstw o jednakowym wejściu.
    /// </summary>
    public class MLPNetwork
    {
        // Lista przykładów uczących
        private List<LearningExample> examples;

        /// <summary>
        /// Zdarzenie wywoływane w momencie utworzenia nowej sieci
        /// </summary>
        public event NetworkEvent OnNetworkCreated;

        /// <summary>
        /// Zdarzenie wywoływane w momencie zakończenia uczenia sieci
        /// </summary>
        public event NetworkEvent OnNetworkLearned;

        /// <summary>
        /// Zdarzenie wywoływane w momencie utworzenia i nauczenia nowego perceptronu
        /// </summary>
        public event PerceptronEvent OnPerceptronCreated;

        /// <summary>
        /// Zdarzenie wywoływane w momencie utworzenia nowej warstwy
        /// </summary>
        public event LayerEvent OnLayerCreated;


        // Wymiar wejścia
        private int inputDimension;

        // Lista warstw
        private List<UniqueLayer> layers;

        internal List<UniqueLayer> Layers
        {
            get
            {
                return layers;
            }
        }

        /// <summary>
        /// Tworzy nowy obiekt
        /// </summary>
        /// <param name="dimension">
        /// Wymiar wejścia
        /// </param>
        /// <param name="examples">
        /// Lista przykładów uczących
        /// </param>
        public MLPNetwork(int dimension, List<LearningExample> examples)
        {
            layers = new List<UniqueLayer>();

            this.examples = examples;
            inputDimension = dimension;
        }

        private void checkDimensions(int dimension, List<int> layersDimensions)
        {
            if (dimension <= 0)
            {
                throw new NetworkDimensionException("Wymiar wejścia musi być większy lub równy 0", dimension);
            }

            if (layersDimensions == null)
            {
                throw new NetworkDimensionException("Lista wymiarów nie jest zainicjowana");
            }

            if (layersDimensions.Count == 0)
            {
                throw new NetworkDimensionException("Lista wymiarów jest być pusta");
            }

            for (int i = 0; i < layersDimensions.Count; i++)
            {
                if (layersDimensions[i] <= 0)
                {
                    throw new NetworkDimensionException("Wymiary wszystkich warstw muszą być dodatnie",
                        layersDimensions, i);
                }
            }

        }

        /// <summary>
        /// Tworzy nową sieć warstwową
        /// </summary>
        /// <param name="dimension">
        /// Wymiar wejścia
        /// </param>
        /// <param name="layersDimensions">
        /// Lista wymiarów każdej warstwy
        /// </param>
        /// <param name="examlesList">
        /// Lista przykładów uczących
        /// </param>
        public MLPNetwork(int dimension, List<int> layersDimensions, PerceptronEvent ev, LayerEvent lev, 
            List<LearningExample> examlesList)
        {
            checkDimensions(dimension, layersDimensions);

            examples = examlesList;
            OnPerceptronCreated += ev;
            OnLayerCreated += lev;
            inputDimension = dimension;

            layers = new List<UniqueLayer>(layersDimensions.Count);

            // Inicjuje generator liczb losowych przeznaczony do ustawiania zaren dla kolejnych warstw
            Random r = new Random();

            // Tworzy każdą z warstw nie ucząc ich
            for (int i = 0; i < layersDimensions.Count; i++)
            {
                //if (i > 0)
                //{
                //    System.Windows.MessageBox.Show("OutputDim: " + layers[i - 1].OutputDimension);
                //}
                layers.Add(new UniqueLayer(i > 0 ? layers[i - 1].OutputDimension : inputDimension,
                    layersDimensions[i], perceptronCreated, r.Next()));
                if (OnLayerCreated != null)
                    OnLayerCreated(this, new LayerEventArgs(layers[i]));

                // Przypisanie odpowiedniej funkcji wyjścia dla każdego perceptronu
                foreach (Perceptron p in layers.Last().Perceptrons)
                {
                    p.setOutputFunctionType(PerceptronOutputFunctionType.SigmoidalFunction);
                }
            }
            //System.Windows.MessageBox.Show("OutputDim: " + layers.Last().OutputDimension);
            if (OnNetworkCreated != null) OnNetworkCreated(this, new NetworkEventArgs(this));
        }

        /// <summary>
        /// Funkcja przeznaczona do tworzenia nowej sieci warstwowej algortymem kafelkowym
        /// </summary>
        /// <param name="iterations">
        /// Maksymalna liczba iteracji do uczenia perceptronów
        /// </param>
        /// <param name="learningConstant">
        /// Stała uczenia
        /// </param>
        public void createNetwork(int iterations, double learningConstant)
        {
            // Tworzy warstwy aż do momentu otrzymania poprawnej weryfikacji
            do
            {
                UniqueLayer layer;

                // Tworzy nową warstwę
                if (layers.Count == 0)
                {
                    // Przekazuje nowej warstwie pierwotny zbiór przykładów uczących
                    layer = new UniqueLayer(inputDimension, iterations, learningConstant, examples, perceptronCreated);
                }
                else
                {
                    // Przekazuje nowej warstwie zbiór przykładów uczących 
                    // zwrócony przez warstwę poprzednią
                    layer = new UniqueLayer(layers[layers.Count - 1].OutputDimension,
                        iterations, learningConstant, layers[layers.Count - 1].NextExamples,
                        perceptronCreated);

                }

                // Dodaje warstwę do listy
                layers.Add(layer);

                // Wywołuje zdarzenie utworzenia warstwy
                if (OnLayerCreated != null) OnLayerCreated(this, new LayerEventArgs(layer));
            }
            while (verify() == false); // Weryfikuje sieć

            // Wywołuje zdarzenie utworzenia sieci
            if (OnNetworkCreated != null) OnNetworkCreated(this, new NetworkEventArgs(this));

        }

        /// <summary>
        /// Funkcja przeznaczona do badania poprawności klasyfikowania przykładów przez sieć
        /// </summary>
        /// <returns>
        /// Zwraca true, jeśli wszystkie przykłady uczące są dobrze klasyfikowane przez sieć
        /// </returns>
        private bool verify()
        {
            foreach (LearningExample e in examples)
            {
                LearningExample newExample = e;

                // Oblicza klasyfikacje kolejno wszystkich warstw
                for (int i = 0; i < layers.Count; i++)
                {
                    newExample = new LearningExample(layers[i].compute(newExample), newExample.ExpectedDoubleValue);
                }

                // Zwrócić wartość true możemy tylko wtedy, gdy ostatnia warstwa zawiera
                // tylko jeden perceptron
                if (layers[layers.Count - 1].OutputDimension != 2)
                {
                    return false;
                }

                // Klasyfikacja ostatniego perceptronu zgadza się z wartością oczekiwaną
                if (newExample.Example[1] != newExample.ExpectedDoubleValue) return false;
            }

            return true;
        }

        /// <summary>
        /// Zwraca liczbę warstw
        /// </summary>
        public int LayersCount
        {
            get
            {
                return layers.Count;
            }
        }

        /// <summary>
        /// Zdarzenie wywoływane w momencie dodania i nauczenia perceptronu
        /// </summary>
        private void perceptronCreated(object sender, PerceptronEventArgs e)
        {
            if (OnPerceptronCreated != null) OnPerceptronCreated(this, e);
        }

        /// <summary>
        /// Funkcja przeznaczona do klasyfikacji konkretnego przykładu
        /// </summary>
        /// <param name="example">
        /// Przykład
        /// </param>
        /// <returns>
        /// Klasyfikacja podanego przykładu
        /// </returns>
        public Vector classify(LearningExample example)
        {
            LearningExample newExample = example;
            classificationExamples = new List<LearningExample>();

            // Przechodzi kolejno przez wszystkie warstwy sieci
            for (int i = 0; i < layers.Count; i++)
            {
                newExample = new LearningExample(layers[i].compute(newExample), example.ExpectedValue);
                classificationExamples.Add(newExample);
            }
            //System.Windows.MessageBox.Show(newExample.ToString());

            return newExample.Example;
        }

        private List<LearningExample> classificationExamples;

        internal List<LearningExample> ClassificationExamples
        {
            get
            {
                return classificationExamples;
            }
        }

        internal List<Vector> delty;

        /// <summary>
        /// Uczy sieć za pomocą propagacji wstecznej
        /// </summary>
        public void learnNetwork(int iterations, double learningConstant, double betaConstant)
        {

            if (examples.Count == 0)
            {
                throw new ExampleListException("Liczba przykładów musi być większa od zera");
            }

            Random r = new Random();
            double eta = learningConstant;

            foreach (UniqueLayer l in layers)
            {
                foreach (Perceptron p in l.Perceptrons)
                {
                    p.Beta = betaConstant;
                }
            }
            // Oblicza wartości u dla każdego perceptronu
            // Przykład wartości ui:
            /// <code>
            /// classificationExamples[0].Example 
            /// </code>
            Vector output;

            for (int i = 0; i < iterations; i++)
            {
                // Inicjalizacja testowego wektora delt
                delty = new List<Vector>();

                // Obecnie rozważany przykład
                LearningExample ex = examples[r.Next(examples.Count)];

                classify(ex);

                // Trzeba utworzyc wektory pamiętające wartości delta
                Vector delta = new Vector(classificationExamples.Last().Example.Dimension);
                Vector newDelta;

                // Zmienne pomocnicze
                Vector lastExapmle = classificationExamples.Last().Example;
                Vector expected = classificationExamples[0].ExpectedValue;

                // Utworzenie wartości delta dla warstwy ostatniej
                for (int j = 1; j < lastExapmle.Dimension; j++)
                {
                    //System.Windows.MessageBox.Show("Błąd wynosi " + (lastExapmle[j] - expected[j])
                    //    + "\nPochodna wynosi " + lastExapmle[j] * (1 - lastExapmle[j]),
                    //    "Obliczanie delty " + j);
                    delta[j] = (lastExapmle[j] - expected[j]) * // Błąd
                        lastExapmle[j] * (1 - lastExapmle[j]); // Pochodna cząstkowa
                    //Console.WriteLine("(" + expected[j].ToString() + " - " + lastExapmle[j] + ")"
                    //    + " * " + (lastExapmle[j] * (1 - lastExapmle[j])) + " = " + delta[j]);
                }

                // Obliczenie wartości delta dla warstw niższych
                // oraz odpowiednich wag
                for (int j = layers.Count - 2; j >= 0; j--)
                {

                    output = classificationExamples[j].Example;
                    newDelta = new Vector(classificationExamples[j].Example.Dimension);
                    newDelta.zeros();
                    for (int k = 1; k < classificationExamples[j + 1].Example.Dimension; k++)
                    {
                        for (int l = 1; l <= layers[j].Perceptrons.Count; l++)
                        {
                            //Console.WriteLine("W trakcie iteracja petli 'l': l = " + l + ", "
                            //    + "liczba perceptronów warstwy " + j + " wynosi " +
                            //    layers[j].Perceptrons.Count);

                            // Perceptron zwracający ui
                            Perceptron p = layers[j + 1].Perceptrons[k - 1];

                            newDelta[l] += delta[k] * p.Weights[l]
                                * output[l] * (1 - output[l]); // Pochodna cząstkowa
                        }
                    }

                    output = classificationExamples[j].Example;
                    // Przypisanie nowych wag
                    for (int k = 0; k < layers[j + 1].OutputDimension - 1; k++)
                    {
                        Perceptron p = layers[j + 1].Perceptrons[k];
                        for (int l = 0; l < p.Dimension; l++)
                        {
                            p.Weights[l] = p.Weights[l] - eta * delta[k + 1] * output[l];
                        }
                    }

                    delty.Add(delta);

                    // Przyjmujemy nowy wektor delta
                    delta = newDelta;
                }

                delty.Add(delta);

                output = classificationExamples[0].Example;

                // Przypisanie nowych wag
                for (int k = 0; k < layers[0].OutputDimension - 1; k++)
                {
                    Perceptron p = layers[0].Perceptrons[k];
                    for (int l = 0; l < p.Dimension; l++)
                    {
                        p.Weights[l] = p.Weights[l] - eta * delta[k + 1] * output[l];
                    }
                }

                if (OnLearningIterationEnded != null && i % 10000 == 0)
                    OnLearningIterationEnded(this, new NetworkLearningIterationEventArgs(this, i));
            }


            if (OnNetworkLearned != null) OnNetworkLearned(this, new NetworkEventArgs(this));
        }

        public event NetworkEvent OnLearningIterationEnded;

        private void mes(string str)
        {
            System.Windows.MessageBox.Show(str);
        }

        /// <summary>
        /// Funkcja przeznaczona do badania globalnego błędu.
        /// </summary>
        public double globalError()
        {
            double err = 0;
            if (examples.Count == 0) throw new Exception("Zerowa lista przkładów");

            Vector sum = new Vector(examples[0].ExpectedValue.Dimension);

            // Wyzerowuje wektor błędu
            sum.zeros();

            foreach (LearningExample ex in examples)
            {
                // Różnica kwadratowa
                //mes(classify(ex).ToString() + ", " + ex.ExpectedValue);
                sum += (ex.ExpectedValue - classify(ex)).power(2);
            }

            // Zaczynamy od 1, żeby pominąć bias
            for (int i = 1; i < examples[0].ExpectedValue.Dimension; i++)
            {
                err += sum[i];
            }

            err /= 2.0F;

            //System.Windows.MessageBox.Show(((long)err).ToString());
            return err;
        }
    }

}
