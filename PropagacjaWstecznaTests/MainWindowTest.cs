using PropagacjaWsteczna;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace PropagacjaWstecznaTests
{
    
    
    /// <summary>
    ///This is a test class for MainWindowTest and is intended
    ///to contain all MainWindowTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MainWindowTest
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
        ///A test for parseTopology
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PropagacjaWsteczna.exe")]
        public void parseTopologyTest()
        {
            MainWindow_Accessor target = new MainWindow_Accessor();
            string top = Topologie.top2x32x32x3;
            List<int> expected = new List<int>();
            expected.Add(2);
            expected.Add(32);
            expected.Add(32);
            expected.Add(3);

            List<int> actual;
            actual = target.parseTopology(top);
            Assert.AreEqual(true, listEquals(actual, expected));
        }

        /// <summary>
        /// Funkcja sprawdzająca, czy listy są równe
        /// </summary>
        private static bool listEquals<T>(List<T> a, List<T> b) where T : IComparable
        {
            if (a.Count != b.Count)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].CompareTo(b[i]) != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
