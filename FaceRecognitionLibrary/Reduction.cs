using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;

namespace FaceRecognitionLibrary
{
    public partial class FaceRecognitionLibrary
    {
        private int outputDimension;
        private int ojIterations;

        /// <summary>
        /// Algorytm redukcji składowych głównych
        /// </summary>
        internal void reduction(List<LearningExample> exampleList)
        {
            try
            {

                //Dispatcher.Invoke(OnReductionStarted, this, new EventArgs());

                List<LearningExample> list = new List<LearningExample>(exampleList);
                int dimension = exampleList[0].Example.Dimension;

                List<PerceptronLib.Vector> principalComponents = new List<PerceptronLib.Vector>(outputDimension);
                for (int i = 0; i < outputDimension; i++)
                {
                    principalComponents.Add(ojLearn(list).Weights);
                    PerceptronLib.Vector w = principalComponents[i];
                    //printLine("Składowa główna: " + w[0] + ", " + w[1] + ", " + w[2] + ", " + w[3]);
                    //printLine("Składowa główna: długość = " + w.Length);
                    List<LearningExample> nextList = new List<LearningExample>();
                    foreach (LearningExample ex in list)
                    {
                        PerceptronLib.Vector x = ex.Example;
                        double val = w * w;
                        double activation = w * x;
                        PerceptronLib.Vector nextExVector = new PerceptronLib.Vector(dimension);
                        nextExVector = x - w * (activation / val);
                        nextExVector.normalizeWeights();
                        LearningExample nextEx = new LearningExample(nextExVector, 0);
                        nextList.Add(nextEx);
                    }
                    list = nextList;

                }

                //saveImages(principalComponents, examplesWidth, examplesHeight);

                //Dispatcher.Invoke(OnReductionFinished, this, new EventArgs());

            }
            catch (Exception ex)
            {
                //printLine(ex.Message + " [ " + ex.StackTrace + " ]");
            }
        }

        /// <summary>
        /// Algorytm Oja 
        /// </summary>
        internal Perceptron ojLearn(List<LearningExample> exampleList)
        {
            #if DEBUG
            //Console.WriteLine("Algorytm Oja: początek");
            #endif

            Random r = new Random();
            double eta = 0.5;
            PerceptronLib.Vector w = (new Perceptron(exampleList[0].Example.Dimension)).Weights;
            w.normalizeWeights();
            //printVectorLength(perceptron.Weights);
            
            for (int i = 0; i < ojIterations; i++)
            {
#if DEBUG
                //if (i % 10 == 0)
                //    printLine("Oj: i = " + i);
                //Console.WriteLine("Algorytm Oja: Wektor: " + w + " długość: " + w.Length);
                //Console.WriteLine("Algorytm Oja: <W,W> = " + w * w);
#endif
                // Losuje następny przykład uczący
                LearningExample ex = exampleList[r.Next(exampleList.Count)];
                PerceptronLib.Vector x = ex.Example;
#if DEBUG
                //Console.WriteLine("Algorytm Oja: Wybrany przykład: " + x);
#endif
                double activation = w * x;
#if DEBUG
                //Console.WriteLine("Algorytm Oja: Aktywacja: " + activation);
                //Console.WriteLine("Algorytm Oja: yW = " + activation * w);
                //Console.WriteLine("Algorytm Oja: <(X - yW), W> = " + (x - activation * w)
                //    * w);
#endif

                //printLine("act: " + activation);
                //for (int j = 0; j < perceptron.Dimension; j++)
                //{
                //    perceptron.Weights[j] += eta * activation * (ex.Example[j]
                //        - activation * perceptron.Weights[j]);
                //}
                w += eta * activation * (x - activation * w);
                //perceptron = new Perceptron(w);

                // Normalizuje długość wektora do 1
                //perceptron.Weights.normalizeWeights();
#if DEBUG

                //Console.WriteLine("Algorytm Oja: " + "wektor główny: "  + w);
#endif
            }

#if DEBUG
            //Console.WriteLine("Algorytm Oja: koniec");
#endif
            return new Perceptron(w);
        }
    }


    
}
