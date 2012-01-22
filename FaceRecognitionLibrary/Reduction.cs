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

        /// <summary>
        /// Algorytm Oja 
        /// </summary>
        private Perceptron ojLearn(List<LearningExample> exampleList)
        {
            Random r = new Random();
            double eta = 0.5;
            PerceptronLib.Vector w = (new Perceptron(exampleList[0].Example.Dimension)).Weights;
            w.normalizeWeights();

            for (int i = 0; i < ojIterations; i++)
            {
                // Losuje następny przykład uczący
                LearningExample ex = exampleList[r.Next(exampleList.Count)];
                PerceptronLib.Vector x = ex.Example;
                double activation = w * x;
                w += eta * activation * (x - activation * w);
            }
            return new Perceptron(w);
        }

        private void checkDimensions(IEnumerable<IEnumerable<double>> examples,
            IEnumerable<IUserInfo> userInfoList)
        {
            if (examples.Count() == 0)
            {
                throw new DimensionException("Liczba przykłóadów musi być większa od 0");
            }

            if (userInfoList.Count() != examples.Count())
            {
                throw new DimensionException("Niezgodna liczba elementów w "
                    + "liście użytkowników i liście przykładów");
            }

            int dim = examples.First().Count();
            foreach (IEnumerable<double> ex in examples)
            {
                if (ex.Count() != dim)
                    throw new DimensionException("Niezgodne wymiary wektorów wejściowych");
            }
        }

        // TODO: 
        // Do zmiany: należy zastosować profesjonalny schemat bazy danych zamiast zwykłej
        // serializacji
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
            FileStream stream = null;
            try
            {
                stream = new System.IO.FileStream(dataBaseFileName, FileMode.Create, System.IO.FileAccess.Write);
                if (stream == null)
                {
                    throw new FileException("Nie można utworzyć bazy pliku bazy danych");
                }
            }
            catch (Exception ex)
            {
                throw new FileException("Błąd tworzenia pliku bazy danych. Patrz wyjątek wewnętrzny",
                    ex);
            }

            EigenFacesDB db = new EigenFacesDB(principalComponents);
            BinaryFormatter formatter = new BinaryFormatter();

            int dimension = examples[0].Example.Dimension;
            for (int l = 0; l < examples.Count; l++)
            {
                LearningExample ex = examples[l];

                // Tworzy nowy element bazy danych
                EigenNode node = new EigenNode(userInfoList[l]);

                for (int k = 0; k < outputDimension; k++)
                {
                    PerceptronLib.Vector p = principalComponents[k];
                    double activation = p * ex.Example;

                    node.Coordinates.Add(activation);

                }

                db.Add(node);
            }

            try
            {
                formatter.Serialize(stream, db);
            }
            catch (Exception ex)
            {
                throw new FaceRecognitionEngineException(
                    "Nie można zserializować bazy danych. Patrz wyjątek wewnętrzny", ex);
            }
            stream.Close();

            dataBase = db;
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
            else
            {
                throw new FileException("Plik bazy danych nie istnieje");
            }
        }
    }



}
