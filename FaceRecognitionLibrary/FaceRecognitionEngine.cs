using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;

namespace FaceRecognitionLibrary
{
    public class UserInfo
    {
        public string login;
    }

    public partial class FaceRecognitionEngine
    {
        private bool dataBaseCreated;
        private const int defaultDimension = 6;
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
            IEnumerable<UserInfo> userInfo, int dimension = defaultDimension)
        {
            List<LearningExample> examples = new List<LearningExample>();

            // Rzutowanie na pojedyncze wektory
            foreach (IEnumerable<double> array in inputVectors)
            {

            }
        }
    }
}
