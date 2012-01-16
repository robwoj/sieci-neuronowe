using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;
using System.Runtime.Serialization;

namespace RozpoznawanieTwarzy
{
    /// <summary>
    /// Klasa reprezentująca bazę obrazów zapamiętanych za pomocą eigenfaces
    /// </summary>
    [Serializable]
    class EigenFacesDB
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
        public EigenFacesDB()
        {
            dataBase = new List<EigenNode>();
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
