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

    public class UserInfo
    {
        private string login;
        private string imie;
        private string nazwisko;
        public string Login
        {
            get
            {
                return login;
            }
        }

        public string Imie
        {
            get
            {
                return imie;
            }
        }

        public string Nazwisko
        {
            get
            {
                return nazwisko;
            }
        }

        public UserInfo(string login, string imie, string nazwisko)
        {
            this.imie = imie;
            this.nazwisko = nazwisko;
            this.login = login;
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

            List<IUserInfo> userInfoList = new List<IUserInfo>(userInfo);
            outputDimension = dimension;
            ojIterations = iterations;

            reduction(examples);

            builDataBase(userInfoList, examples);
        }

        public FaceRecognitionEngine(string fileName)
        {
            loadDataBase(fileName);
        }

        public bool recogniseFace(IEnumerable<double> face, string login)
        {
            return login == dataBase.compareFace(new Vector(face));

            //IUserInfo userInfo = dataBase.compareFace(new Vector(face));

            //if (userInfo.Login == login)
            //{
            //    return userInfo;
            //}
            //else
            //{
            //    throw new Exception("Nie rozpoznano użytkownika");
            //}
        }
    }
}
