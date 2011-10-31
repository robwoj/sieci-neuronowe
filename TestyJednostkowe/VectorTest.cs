using Perceptron1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Perceptron1;

namespace TestyJednostkowe
{
    
    
    /// <summary>
    ///This is a test class for VectorTest and is intended
    ///to contain all VectorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VectorTest
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
        ///A test for Dimension
        ///</summary>
        [TestMethod()]
        public void DimensionTest()
        {
            int dimension = 0;
            Vector target = null;
            try
            {
                target = new Vector(dimension);
            }
            catch (SizeException se)
            {
                Console.WriteLine("Zrzucenie wyjątku w przypadku zerowego wymiaru - OK");
            }
            target = new Vector(45);
            int actual;
            actual = target.Dimension;
            Assert.AreEqual(actual, target.Dimension);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod()]
        public void op_MultiplyTest()
        {
            Vector a = new Perceptron1.Vector(3);
            Vector b = new Perceptron1.Vector(3);
            
            for (int i = 0; i < 3; i++)
            {
                a[i] = i;
                b[i] = i;
            }

            double expected = 5F;
            double actual;
            actual = (a * b);
            Assert.AreEqual(expected, actual);

            for (int i = 0; i < 3; i++)
            {
                a[i] = i + 1;
                b[i] = i + 2;
            }

            expected = 20F;
            
            actual = (a * b);
            Assert.AreEqual(expected, actual);
        
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod()]
        public void op_AdditionTest()
        {
            Vector a = new Perceptron1.Vector(3);
            Vector b = new Perceptron1.Vector(a.Dimension);
            for (int i = 0; i < a.Dimension; i++)
            {
                a[i] = i;
                b[i] = i;
            }

            Vector expected = new Perceptron1.Vector(a.Dimension);
            for (int i = 0; i < a.Dimension; i++)
            {
               expected[i] = 2 * i;
            }

            Vector actual;
            actual = (a + b);
            Assert.AreEqual(expected, actual);

            for (int i = 0; i < a.Dimension; i++)
            {
                a[i] = i + 1;
                b[i] = i + 2;
            }

            expected = new Perceptron1.Vector(a.Dimension);
            for (int i = 0; i < a.Dimension; i++)
            {
                expected[i] = 2 * (i + 1) + 1;
            }

            actual = (a + b);
            Assert.AreEqual(expected, actual);
        }
    }
}
