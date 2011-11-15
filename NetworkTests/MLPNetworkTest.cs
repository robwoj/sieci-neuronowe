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
                    target.learnNetwork(iterations);
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

                    Console.WriteLine(p2.Weights.ToString());
                    examples.Add(new LearningExample(v, expected));
                    Console.WriteLine("Wyjście pierwszego perceptronu:");
                    Console.WriteLine(p0.outputFunction(v).ToString());

                    Console.WriteLine("Wyjście drugiego perceptronu:");
                    Console.WriteLine(p1.outputFunction(v).ToString());

                    Vector v0 = new Vector(3);
                    v0[0] = 1;
                    v0[1] = p0.outputFunction(v);
                    v0[2] = p1.outputFunction(v);

                    Console.WriteLine("Wyjście trzeciego perceptronu:");
                    Console.WriteLine(p2.outputFunction(v0).ToString());

                    target.learnNetwork(iterations);
                    Console.WriteLine("Wyjścia kolejnych warstw:");
                    foreach (LearningExample e in target.ClassificationExamples)
                    {
                        Console.WriteLine(e.ToString());
                    }

                    Console.WriteLine("Wartości delta po iteracji:");
                    foreach (Vector d in target.delty)
                    {
                        Console.WriteLine(d.ToString());
                    }

                    Console.WriteLine("Wagi pierwszego perceptronu po iteracji:");
                    Console.WriteLine(p0.Weights.ToString());
                    Console.WriteLine("Wagi drugiego perceptronu po iteracji:");
                    Console.WriteLine(p1.Weights.ToString());
                    Console.WriteLine("Wagi trzeciego perceptronu po iteracji:");
                    Console.WriteLine(p2.Weights.ToString());
                    return;
                }
            }

            Assert.Fail("Nie został zrzucony odpowiedni wyjątek");
        }
    }
}
