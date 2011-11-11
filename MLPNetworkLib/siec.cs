using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using PerceptronLib;

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

        /// <summary>
        /// Tworzy nową sieć warstwową
        /// </summary>
        /// <param name="dimension">
        /// Wymiar wejścia
        /// </param>
        /// <param name="layersDimensions">
        /// Lista wymiarów każdej warstwy
        /// </param>
        public MLPNetwork(int dimension, List<int> layersDimensions)
        {
            inputDimension = dimension;

            layers = new List<UniqueLayer>(layersDimensions.Count);

            // Tworzy każdą z warstw nie ucząc ich
            for (int i = 0; i < layersDimensions.Count; i++)
            {
                layers.Add(new UniqueLayer(inputDimension, layersDimensions[i], perceptronCreated));
            }
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
                        iterations, learningConstant, layers[layers.Count - 1].NextExamples, perceptronCreated);
                    
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
        public double classify(LearningExample example)
        {
            LearningExample newExample = example;

            // Przechodzi kolejno przez wszystkie warstwy sieci
            for (int i = 0; i < layers.Count; i++)
            {
                newExample = new LearningExample(layers[i].compute(newExample), example.ExpectedDoubleValue);

            }

            return newExample.Example[1];
        }
    }

}
