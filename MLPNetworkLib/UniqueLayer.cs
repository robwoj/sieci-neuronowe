using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;

namespace MLPNetworkLib
{

    /// <summary>
    /// Klasa reprezentująca warstwę sieci neuronów
    /// Warstwa ta ma wszystkie wejścia takie same dla każdego perceptronu składowego
    /// </summary>
    public class UniqueLayer
    {
        /// <summary>
        /// Przeciążenie przeznaczone do dukowania warstwy
        /// </summary>
        public override string ToString()
        {
            string str = "";
            foreach (Perceptron p in perceptrons)
            {
                str += p.Weights.ToString() + ", ";
            }
            return str;
        }

        // Lista perceptronów
        private List<Perceptron> perceptrons;

        // Wymiar wejścia - wspólny dla wszystkich perceptronów
        private int inputDimension;
        private Dictionary<Classification, List<LearningExample>> dict;
        private List<LearningExample> nextExamples;

        /// <summary>
        /// Zdarzenie wywoływane w momencie utworzenia i nuaczenia perceptronu
        /// </summary>
        public event PerceptronEvent OnPerceptronCreated;

        /// <summary>
        /// Zdarzenie wywoływane w momencie utworzenia warstwy 
        /// </summary>
        public event LayerEvent OnLayerLearned;

        /// <summary>
        /// Zwraca wymiar wejścia
        /// </summary>
        public int InputDimension
        {
            get
            {
                return inputDimension;
            }
        }

        /// <summary>
        /// Tworzy nową warstwę nie ucząc jej algorytmem kaflowym.
        /// Uczenie zostanie przeprowadzone później (za pomocą propagacji wstecznej)
        /// </summary>
        /// <param name="inputDim">
        /// Wymiar wejścia
        /// </param>
        /// <param name="perceptronsCount">
        /// Liczba perceptronów w warstwie
        /// </param>
        public UniqueLayer(int inputDim, int perceptronsCount, PerceptronEvent ev)
        {
            // Przypisanie obsługi zdarzenia
            OnPerceptronCreated += ev;

            // Przypisanie wartości wymiaru wejściowego i sprawdzenie
            inputDimension = inputDim;
            checkInputDim();

            // Inicjalizacja listy perceptronów
            perceptrons = new List<Perceptron>(perceptronsCount);

            for (int i = 0; i < perceptronsCount; i++)
            {
                perceptrons.Add(new Perceptron(inputDimension));
            }
        }


        private void checkInputDim()
        {
            // Sprawdzenie poprawności wymiaru wejścia
            if (inputDimension <= 0)
            {
                throw new InvalidOperationException(
                    "Rozmiar wejścia musi być większy od zera");
            }

        }

        /// <summary>
        /// Tworzy nową warstwę
        /// </summary>
        /// <param name="inputDim">
        /// Wymiar wejścia
        /// </param>
        /// <param name="iterations">
        /// Maksymalna liczba iteracji do uczenia percpeptronów
        /// </param>
        /// <param name="learningConstant">
        /// Stała uczenia
        /// </param>
        /// <param name="examples">
        /// Lista przykładów uczących
        /// </param>
        /// <param name="ev">
        /// Funkcja obsługująca zdarzenie utworzenia perceptronu
        /// </param>
        public UniqueLayer(int inputDim, int iterations, double learningConstant,
            List<LearningExample> examples, PerceptronEvent ev)
        {

            // Przypisanie obsługi zdarzenia
            OnPerceptronCreated += ev;

            inputDimension = inputDim;

            // Sprawdza poprawność wymiaru wejściowego
            checkInputDim();

            // Inicjuje listę perceptronów
            perceptrons = new List<Perceptron>();

            // Rozpoczyna uczenie warstwy
            learn(iterations, learningConstant, examples);

            // Tworzy listę przykładów wyjściowych
            nextExamples = new List<LearningExample>();

            foreach (LearningExample e in examples)
            {
                Vector v = compute(e);
                nextExamples.Add(new LearningExample(v, e.ExpectedDoubleValue));
            }
        }

        /// <summary>
        /// Funkcja przeznaczona do uczenia warstwy
        /// Uczenie warstwy polega na uczeniu i dodawaniu kolejnych perceptronów 
        /// aż do momentu odzyskania wierności, wg algorytmu kafelkowego
        /// </summary>
        /// <param name="iterations">
        /// Maksymalna liczba iteracji uczenia perceptronu
        /// </param>
        /// <param name="learningConstant">
        /// Stała uczenia
        /// </param>
        /// <param name="examples">
        /// Lista przykładów uczących
        /// </param>
        private void learn(int iterations, double learningConstant, List<LearningExample> examples)
        {

            perceptrons = new List<Perceptron>();
            List<LearningExample> differentClassExamples = null;

            // Dodaje perceptrony do listy dopóki nie odzyska wierności
            while ((differentClassExamples = checkFaithful(examples)) != null)
            {
                Perceptron p = new Perceptron(inputDimension);

                // Dodaje perceptron
                perceptrons.Add(p);

                // Uczy dodany perceptron
                p.pocketLearn(iterations, learningConstant, differentClassExamples);

                // Wywołuje zdarzenie dodania perceptronu
                if (OnPerceptronCreated != null) OnPerceptronCreated(this, new PerceptronEventArgs(p));
            }

            // Wywołuje zdarzenie dodanie warstwy
            if (OnLayerLearned != null) OnLayerLearned(this, new LayerEventArgs(this));
        }

        /// <summary>
        /// Funkcja przeznaczona do sprawdzania wierności
        /// </summary>
        /// <param name="examples">
        /// Lista przykładów uczących
        /// </param>
        /// <returns>
        /// Maksymalny zbiór przykładów z różnych klas posiadający tę samą klasyfikację 
        /// w obecnym zbiorze perceptronów
        /// </returns>
        private List<LearningExample> checkFaithful(List<LearningExample> examples)
        {

            // Zwraca całą listę w przypadku, gdy nie ma jeszcze perceptronów
            if (perceptrons.Count == 0)
            {
                return examples;
            }

            // Słownik rozdzielający przykłady uczące na podstawie klasyfikacji
            // zadanej zbiorem perceptronów
            dict = new Dictionary<Classification, List<LearningExample>>();

            // Iteruje po wsystkich przykłądach uczących i rozdziela je na podstawie klasyfikacji
            foreach (LearningExample ex in examples)
            {
                // Tworzy klasyfikację perceptronu
                Classification c = new Classification(perceptrons, ex);
                if (dict.ContainsKey(c))
                {
                    // Dodaje przykład ex do słownika o kluczu c
                    dict[c].Add(ex);
                }
                else
                {
                    // Tworzy w słowniku klucz c i dodaje przykład ex do klucza c
                    dict.Add(c, new List<LearningExample>());
                    dict[c].Add(ex);
                }
            }

            // Zwraca największy zbiór przykładów o tej samej klasyfikacji, ale z różnych klas
            return findMax(dict);
        }

        /// <summary>
        /// Zwraca największy zbiór przykładów o tej samej klasyfikacji, ale z różnych klas
        /// </summary>
        /// <param name="dict">
        /// Słownik z podziałem na klasy
        /// </param>
        private List<LearningExample> findMax(
            Dictionary<Classification, List<LearningExample>> dict)
        {
            List<LearningExample> max = dict.First().Value;
            bool found = false;

            // Przegląda słownik i sprawdza kolejne klasyfikacje
            foreach (Classification c in dict.Keys)
            {
                // Sprawdza liczbę elementów zbioru o kluczu c.
                // Porównujemy porzykłady tylko, jeśli zbiór jest większy od rekordzisty.
                // W innym przypadku nie ma to sensu.
                if (dict[c].Count >= max.Count)
                {
                    bool containsDifferentClass = false;
                    // Bierze pierwszą wartość oczekiwaną i sprawdza kolejno wszstkie elementy
                    // Jeśli znajdzie się jeden przykład z różnych klas - przerywa pętlę
                    double expectedClass = dict[c][0].ExpectedDoubleValue;
                    for (int i = 0; i < dict[c].Count; i++)
                    {
                        if (expectedClass != dict[c][i].ExpectedDoubleValue)
                        {
                            containsDifferentClass = true;
                            break;
                        }
                    }

                    // Jeśli znalazł się przykład z innych klas - zmienia rekordzistę
                    if (containsDifferentClass == false)
                    {
                        continue;
                    }
                    else
                    {
                        max = dict[c];

                        // Zaznacza, że znaleziono chćby jednego rekordzistę
                        found = true;
                    }
                }
            }

            // Jeśli nie znaleziono, zwraza null
            if (found == false) return null;


            return max;
        }

        /// <summary>
        /// Funkcja przeznaczona do zwrócenia wektora wyjściowego danej warstwy
        /// </summary>
        /// <param name="ex">
        /// Wejście
        /// </param>
        /// <returns>
        /// Klasyfikacja wejścia
        /// </returns>
        public Vector compute(LearningExample ex)
        {
            Vector v = new Vector(perceptrons.Count + 1);

            // Inicjuje bias
            v[0] = (new Random()).NextDouble();

            // Kolejne współrzędne wektora wyjściowego są wyjściami perceptronów składowych
            for (int i = 0; i < perceptrons.Count; i++)
            {
                v[i + 1] = perceptrons[i].function(ex.Example);
            }

            return v;
        }

        /// <summary>
        /// Zwraca wymiar wyjścia
        /// </summary>
        public int OutputDimension
        {
            get
            {
                return perceptrons.Count + 1;
            }
        }

        /// <summary>
        /// Zwraca listę przykładów dla kolejnej warstwy
        /// </summary>
        public List<LearningExample> NextExamples
        {
            get
            {
                return nextExamples;
            }
        }
    }
}
