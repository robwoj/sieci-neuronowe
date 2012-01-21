using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;

namespace FaceRecognitionLibrary
{
    public interface IUserInfo
    {
        string Login
        {
            get;
        }
    }

    public partial class FaceRecognitionEngine
    {
        private bool dataBaseCreated;
        private const int defaultDimension = 6;
        private const int defaultIterations = 100;

        public bool DataBaseCreated
        {
            get
            {
                return dataBaseCreated;
            }
        }

        public FaceRecognitionEngine()
        {
            dataBaseCreated = false;
        }

        public FaceRecognitionEngine(IEnumerable<IEnumerable<double>> inputVectors, 
            IEnumerable<IUserInfo> userInfo, int dimension = defaultDimension, int iterations = defaultIterations)
        {
            dataBaseFileName = "D:\\sieci-neuronowe\\database.db";

            List<LearningExample> examples = new List<LearningExample>();

            // Rzutowanie na pojedyncze wektory
            foreach (IEnumerable<double> array in inputVectors)
            {
                examples.Add(new LearningExample(new Vector(array), 0));
            }

            outputDimension = dimension;
            ojIterations = iterations;

            reduction(examples);


        }
    }
}
