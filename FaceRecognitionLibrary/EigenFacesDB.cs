using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;
using System.Runtime.Serialization;

namespace FaceRecognitionLibrary
{
    /// <summary>
    /// Klasa reprezentująca bazę obrazów zapamiętanych za pomocą eigenfaces
    /// </summary>
    [Serializable]
    public class EigenFacesDB
    {

        /// <summary>
        /// Wymiar przestrzeni
        /// </summary>
        private int dimension;

        /// <summary>
        /// Baza przestrzeni
        /// </summary>
        private List<Vector> eigenVectors;

        /// <summary>
        /// Zbiór danych
        /// </summary>
        private List<EigenNode> dataBase;

        /// <summary>
        /// Dodaje nowy element do bazy danych
        /// </summary>
        /// <param name="node">
        /// Element dodawany
        /// </param>
        public void add(EigenNode node)
        {

            // Sprawdza zgodność wymiarów
            if (node.Coordinates.Count != dimension)
            {
                throw new Exception("Wymiar bazy wynosi " + dimension + ", a współrzędne " +
                    node.Coordinates.Count);
            }

            // Dodaje element
            dataBase.Add(node);
        }

        /// <summary>
        /// Usuwa element bazy danych
        /// </summary>
        /// <param name="node">
        /// Element do usunięcia
        /// </param>
        /// <returns>
        /// Czy zostałusunięty żądany element
        /// </returns>
        public bool remove(EigenNode node)
        {
            return dataBase.Remove(node);
        }

        /// <summary>
        /// Tworzy pustą bazę danych
        /// </summary>
        public EigenFacesDB(List<Vector> eigenVs)
        {
            dataBase = new List<EigenNode>();
            eigenVectors = new List<Vector>();
            for (int i = 0; i < eigenVs.Count; i++)
            {
                eigenVectors.Add(eigenVs[i]);
            }

            dimension = eigenVectors.Count;
        }

        /// <summary>
        /// Zwraca wymiar przestrzeni
        /// </summary>
        public int Dimension
        {
            get
            {
                return dimension;
            }
        }

        /// <summary>
        /// Zwraca najbliższą twarz w bazie
        /// </summary>
        /// <param name="face">
        /// Wektor złożony z pikseli obrazka
        /// </param>
        /// <returns>
        /// Nazwa skojarzona ze znalezionym obrazem
        /// </returns>
        public string compareFace(Vector face)
        {
            face.normalizeWeights();

            Vector v = new Vector(face.Dimension);
            List<double> values = new List<double>();
            foreach (Vector eigenV in eigenVectors)
            {
                double val = eigenV * eigenV;
                values.Add(val);
            }

            for (int i = 0; i < dimension; i++)
            {
                double act = eigenVectors[i] * face;
                v += eigenVectors[i] * (act / values[i]);
            }

            double min = -1;
            EigenNode minNode = null;
            try
            {
                foreach (EigenNode node in dataBase)
                {
                    Vector w = new Vector(face.Dimension);

                    for (int j = 0; j < dimension; j++)
                    {
                        w += eigenVectors[j] * (node.Coordinates[j] / values[j]);
                    }

                    Vector sum = w - v;
                    double diff = sum * sum;

                    if (min == -1)
                    {
                        min = diff;
                        minNode = node;
                    }
                    else
                    {
                        if (min > diff)
                        {
                            min = diff;
                            minNode = node;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message + " [ " + ex.StackTrace + " ]";
            }

            if (minNode != null)
                return minNode.Name;

            return string.Empty;
        }
    }

    /// <summary>
    /// Pojedynczy obraz zapamiętany za pomocą eigenfaces
    /// </summary>
    [Serializable]
    public class EigenNode
    {
        public List<double> Coordinates { get; set; }

        /// <summary>
        /// Nazwa obrazu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tworzy nowy element
        /// </summary>
        public EigenNode(string name)
        {
            Name = name;
            Coordinates = new List<double>();
        }
    }
}
