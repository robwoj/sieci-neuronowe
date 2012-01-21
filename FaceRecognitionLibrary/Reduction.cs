using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;
using System.IO;
using System.Runtime.Serialization;

// Dopóki nie zostanie zastosowany schemat bazy danych
using System.Runtime.Serialization.Formatters.Binary;

namespace FaceRecognitionLibrary
{
    public partial class FaceRecognitionEngine
    {
        private int outputDimension;
        private int ojIterations;
        private string dataBaseFileName;
        private List<Vector> principalComponents;
        private EigenFacesDB dataBase;

        /// <summary>
        /// Algorytm redukcji składowych głównych
        /// </summary>
        internal void reduction(List<LearningExample> exampleList)
        {
            try
            {
                List<LearningExample> list = new List<LearningExample>(exampleList);
                int dimension = exampleList[0].Example.Dimension;

                principalComponents = new List<PerceptronLib.Vector>(outputDimension);
                for (int i = 0; i < outputDimension; i++)
                {
                    principalComponents.Add(ojLearn(list).Weights);
                    PerceptronLib.Vector w = principalComponents[i];
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
            }
            catch (Exception ex)
            {
                //printLine(ex.Message + " [ " + ex.StackTrace + " ]");
            }
        }

        /// <summary>
        /// Algorytm Oja 
        /// </summary>
        private Perceptron ojLearn(List<LearningExample> exampleList)
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

        // TODO: 
        // Do zmiany: należy zastosować profesjonalny schemat bazy danych zamiast zwykłej
        // serializacji
        // Dodać sprawdzanie równości wymiarów przykładów wejściowych i wektorów głównych
        /// <summary>
        /// Buduje bazę danych
        /// </summary>
        /// <param name="principalComponents">
        /// Lista wektorów głównych
        /// </param>
        /// <param name="userInfoList">
        /// Dane użytkownika
        /// </param>
        private void builDataBase(List<IUserInfo> userInfoList,
            List<LearningExample> examples)
        {
            FileStream stream = new System.IO.FileStream(dataBaseFileName, FileMode.Create, System.IO.FileAccess.Write);
            EigenFacesDB db = new EigenFacesDB(principalComponents);
            BinaryFormatter formatter = new BinaryFormatter();

            int dimension = examples[0].Example.Dimension;
            for (int l = 0; l < examples.Count; l++)
            {
                LearningExample ex = examples[l];

                // Tworzy nowy element bazy danych
                EigenNode node = new EigenNode(userInfoList[l].Login);

                for (int k = 0; k < outputDimension; k++)
                {
                    PerceptronLib.Vector p = principalComponents[k];
                    double activation = p * ex.Example;

                    node.Coordinates.Add(activation);

                }

                db.add(node);
            }

            formatter.Serialize(stream, db);
            stream.Close();

            dataBase = db;
            dataBaseCreated = true;
        }

        private void loadDataBase(string fileName)
        {
            if (File.Exists(fileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                EigenFacesDB db = (EigenFacesDB)formatter.Deserialize(stream);

                principalComponents = db.EigenVectors;
                dataBase = db;
            }
        }
    }



}
