using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerceptronLib;
using RozpoznawanieTwarzy;

namespace RozpoznawanieTwarzyTests
{
    /// <summary>
    /// Summary description for ReductionTest
    /// </summary>
    [TestClass]
    public class ReductionTest
    {
        private List<LearningExample> points;
        private double ojResultBias;
        public ReductionTest()
        {
            // Inicjalizacja listy punktów 
            points = new List<LearningExample>();
            addNewPoint(1, 1);
            addNewPoint(3, 2);
            addNewPoint(2, 3);
            addNewPoint(-1, -1);
            addNewPoint(-2, -2);
            addNewPoint(-2, -3);
            addNewPoint(-3, -2);
            addNewPoint(-4, -3);
            addNewPoint(-3, -4);

            double xsum = 0, ysum = 0;
            foreach (LearningExample e in points)
            {
                xsum += e.Example[0];
                ysum += e.Example[1];
            }

            double xmed = xsum / 9.0F;
            double ymed = ysum / 9.0F;

            foreach (LearningExample e in points)
            {
                e.Example[0] -= xmed;
                e.Example[1] -= ymed;
            }

            foreach (LearningExample ex in points)
            {
                ex.Example[0] /= 5;
                ex.Example[1] /= 5;
            }

            // Inicjalizacja progu akceptacji błędu wektora głównego
            ojResultBias = 0.001;
        }

        private void addNewPoint(double x, double y)
        {
            Vector v = new Vector(2);
            v[0] = x;
            v[1] = y;

            points.Add(new LearningExample(v, 0));
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Test metody oja. Oczekujemy wektora postaci 
        /// a * [1,1]
        /// gdzie a jest dowolną liczbą rzeczywistą.
        /// </summary>
        //[TestMethod]
        //public void OjTest()
        //{
        //        printPointsList();

        //        MainWindow w = new MainWindow(1000, 1);

        //        Perceptron p = w.ojLearn(points);

        //        Vector r = p.Weights;
        //        Console.WriteLine(r.Length.ToString());
        //        Console.WriteLine(r.ToString());

        //        double kat = Math.Abs(r[0] / r[1]);
        //        //Assert.IsTrue(kat <= 1.0F + ojResultBias && kat >= 1.0F - ojResultBias,
        //        //    "Kąt wektora głównego znacząco odbiega od oczekiwanego [" + kat + "]");
        //}

        private void printPointsList()
        {
            foreach (LearningExample e in points)
            {
                Console.WriteLine("Współrzędne: " + e.Example.ToString() + ", Długość: " + e.Example.Length);
            }
        }
    }
}
