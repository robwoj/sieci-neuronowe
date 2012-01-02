using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using PerceptronLib;
using System.Drawing;
using System.Threading;

namespace RozpoznawanieTwarzy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Algorytm redukcji składowych głównych
        /// </summary>
        internal void reduction(List<LearningExample> exampleList)
        {
            Dispatcher.Invoke(OnReductionStarted, this, new EventArgs());

            List<LearningExample> list = new List<LearningExample>(exampleList);
            int dimension = exampleList[0].Example.Dimension;

            List<Perceptron> principalComponents = new List<Perceptron>(outputDimension);
            for (int i = 0; i < outputDimension; i++)
            {
#if DEBUG
                printLine("i = " + i); 
#endif
                principalComponents.Add(ojLearn(list));
                Perceptron p = principalComponents[i];
                List<LearningExample> nextList = new List<LearningExample>();
                foreach (LearningExample ex in list)
                {
                    double val = p.Weights * p.Weights;
                    double activation = p.Weights * ex.Example;
                    PerceptronLib.Vector nextExVector = new PerceptronLib.Vector(dimension);
                    for (int j = 0; j < dimension; j++)
                    {
                        nextExVector[j] = ex.Example[j] - p.Weights[j] * activation / val;
                    }
                    LearningExample nextEx = new LearningExample(nextExVector, 0);
                    nextList.Add(nextEx);
                }
                list = nextList;

            }

            saveImages(principalComponents, examplesWidth, examplesHeight);

            Dispatcher.Invoke(OnReductionFinished, this, new EventArgs());
        }

        /// <summary>
        /// Algorytm Oja 
        /// </summary>
        internal Perceptron ojLearn(List<LearningExample> exampleList)
        {
            #if DEBUG
            Console.WriteLine("Algorytm Oja: początek");
            #endif

            Random r = new Random();
            double eta = 0.5;
            Perceptron perceptron = new Perceptron(exampleList[0].Example.Dimension);
            perceptron.Weights.normalizeWeights();
            //printVectorLength(perceptron.Weights);
            
            for (int i = 0; i < ojIterations; i++)
            {
#if DEBUG
                if (i % 10 == 0)
                    printLine("Oj: i = " + i);
                Console.WriteLine("Algorytm Oja: Wektor: " + perceptron.Weights + " długość: " + perceptron.Weights.Length);
#endif
                // Losuje następny przykład uczący
                LearningExample ex = exampleList[r.Next(exampleList.Count)];
#if DEBUG
                Console.WriteLine("Algorytm Oja: Wybrany przykład: " + ex.Example);
#endif
                double activation = perceptron.Weights * ex.Example;
#if DEBUG
                Console.WriteLine("Algorytm Oja: Aktywacja: " + activation);
#endif

                //printLine("act: " + activation);
                for (int j = 0; j < perceptron.Dimension; j++)
                {
                    perceptron.Weights[j] += eta * activation * ex.Example[j];
                }

                // Normalizuje długość wektora do 1
                perceptron.Weights.normalizeWeights();
#if DEBUG

                Console.WriteLine("Algorytm Oja: " + "wektor główny: "  + perceptron.Weights);
#endif
            }

#if DEBUG
            Console.WriteLine("Algorytm Oja: koniec");
#endif
            return perceptron;
        }
    }

}