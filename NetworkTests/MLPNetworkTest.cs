using MLPNetworkLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PerceptronLib;
using System.Collections.Generic;

namespace NetworkTests
{
    
    
    /// <summary>
    ///This is a test class for MLPNetworkTest and is intended
    ///to contain all MLPNetworkTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MLPNetworkTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for learnNetwork
        ///</summary>
        [TestMethod()]
        public void learnNetworkTest()
        {
            int dimension = 0; 
            List<LearningExample> examples = new List<LearningExample>();
            MLPNetwork target = null;

            List<int> dimensions = new List<int>();
            int iterations = 1; // Tylko jedna iteracja
            try
            {
                target = new MLPNetwork(dimension, dimensions, null, null, examples);
            }
            catch (NetworkDimensionException e1)
            {
                Assert.AreEqual(e1.BadDimension, 0);

                
                //dimensions.Add(3);
                dimensions.Add(2);
                dimensions.Add(1);
                dimension = 3;

                target = new MLPNetwork(dimension, dimensions, null, null, examples);

                try
                {
                    target.learnNetwork(iterations, 0.5);
                }
                catch (ExampleListException)
                {
                    Vector v = new Vector(3);
                    v[0] = 1;
                    v[1] = 0.8;
                    v[2] = 0.3;

                    Perceptron p0 = target.Layers[0].Perceptrons[0];
                    Perceptron p1 = target.Layers[0].Perceptrons[1];
                    Perceptron p2 = target.Layers[1].Perceptrons[0];
                    p0.Weights[0] = 0.3;
                    p0.Weights[1] = 0.5;
                    p0.Weights[2] = -0.2;
                    p1.Weights[0] = -0.5;
                    p1.Weights[1] = 0.1;
                    p1.Weights[2] = -0.2;
                    p2.Weights[0] = -0.4;
                    p2.Weights[1] = -0.8;
                    p2.Weights[2] = 0.1;

                    Vector expected = new Vector(2);
                    expected[1] = 0.8;

                    //Console.WriteLine("Iloczyn wektorowy: " + v * p0.Weights);
                    //Console.WriteLine(p2.Weights.ToString());
                    examples.Add(new LearningExample(v, expected));
                    //Console.WriteLine("Wyjście pierwszego perceptronu:");
                    //Console.WriteLine(p0.outputFunction(v).ToString());

                    //Console.WriteLine("Wyjście drugiego perceptronu:");
                    //Console.WriteLine(p1.outputFunction(v).ToString());

                    Vector v0 = new Vector(3);
                    v0[0] = 1;
                    v0[1] = s(0.64);
                    v0[2] = s(-0.48);

                    target.classify(examples[0]);

                    v0 = round(v0);

                    //Console.WriteLine("Wektor po zaokrągleniu: " + v0);
                    // Sprawdza, czy dobrze liczone są wartości wyjściowe perceptronów
                    Assert.AreEqual(v0.ToString(), round(target.ClassificationExamples[0].Example).ToString());

                    Vector v1 = new Vector(2);
                    v1[0] = 1;
                    v1[1] = s(-0.886);
                    v1 = round(v1);

                    Assert.AreEqual(v1.ToString(), round(target.ClassificationExamples[1].Example).ToString());
                    Assert.AreEqual(0.292, v1[1]);

                    Console.WriteLine("Blad: " + target.globalError());
                    target.learnNetwork(1, 0.5);
                    Console.WriteLine("Wyjścia kolejnych warstw:");
                    foreach (LearningExample e in target.ClassificationExamples)
                    {
                        Console.WriteLine(round(e.Example).ToString());
                    }

                    Vector d0 = new Vector(2);
                    d0[1] = -0.105;

                    Assert.AreEqual(d0.ToString(), round(target.delty[1]).ToString());

                    Vector d1 = new Vector(3);
                    d1[1] = 0.019;
                    d1[2] = -0.002;
                    Assert.AreEqual(d1.ToString(), round(target.delty[0]).ToString());


                    Console.WriteLine("Wagi pierwszego perceptronu po iteracji:");
                    Console.WriteLine(round(p0.Weights).ToString());
                    Console.WriteLine("Wagi drugiego perceptronu po iteracji:");
                    Console.WriteLine(round(p1.Weights).ToString());
                    Console.WriteLine("Wagi trzeciego perceptronu po iteracji:");
                    Console.WriteLine(round(p2.Weights).ToString());

                    for (int i = 0; i < 500; i++)
                    {
                        target.learnNetwork(iterations, 0.5);
                        Console.WriteLine("Blad: " + target.globalError());
                    }
                    return;
                }
            }

            Assert.Fail("Nie został zrzucony odpowiedni wyjątek");
        }

        /// <summary>
        /// Funkcja sigmoidalna
        /// </summary>
        private double s(double val)
        {
            double retval = 1 / (1 + Math.Exp(-val));
            Assert.IsTrue(retval <= 1 && retval >= 0);

            return retval;
        }

        private Vector round(Vector v, int places = 3)
        {
            Vector r = new Vector(v.Dimension);
            for (int i = 0; i < v.Dimension; i++)
            {
                r[i] = Math.Round(v[i], places);
            }

            return r;
        }

        [TestMethod()]
        public void serializationTest()
        {
            List<LearningExample> examples = new List<LearningExample>();
            List<int> dimensions = new List<int>();
            dimensions.Add(3);
            dimensions.Add(3);
            dimensions.Add(1);

            MLPNetwork network = new MLPNetwork(3, dimensions, null, null, examples);

            //weights[0] = new Vector(3);
            //weights[0][0] = 0.1;
            //weights[0][1] = 0.2;
            //weights[0][2] = 0.3;
            //weights[1] = new Vector(3);
            //weights[1][0] = 0.15;
            //weights[1][1] = 0.26;
            //weights[1][2] = 0.37;
            //weights[0] = new Vector(1);
            //weights[0][0] = 0.4;


        }
    }
}
