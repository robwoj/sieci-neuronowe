using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerceptronLib;

namespace FaceRecognitionLibrary
{
    /// <summary>
    /// Interfejs danych użytkownika. Musi posiadać przynajmniej login
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// Login użytkownika
        /// </summary>
        string Login
        {
            get;
        }
    }

    /// <summary>
    /// Klasa przykładowa implementująca interfejs IUserInfo. Rozszerza go o imię i nazwisko.
    /// Może być użyta do testowania aplikacji
    /// </summary>
    [Serializable]
    public class UserInfo : IUserInfo
    {
        private string login;
        private string imie;
        private string nazwisko;

        /// <summary>
        /// Login użytkownika
        /// </summary>
        public string Login
        {
            get
            {
                return login;
            }
        }

        /// <summary>
        /// Imię użytkownika
        /// </summary>
        public string Imie
        {
            get
            {
                return imie;
            }
        }

        /// <summary>
        /// Nazwisko użytkownika
        /// </summary>
        public string Nazwisko
        {
            get
            {
                return nazwisko;
            }
        }

        /// <summary>
        /// Konstruktor. Tworzy nowego użytkownika systemu.
        /// </summary>
        /// <param name="login">
        /// Login użytkownika
        /// </param>
        /// <param name="imie">
        /// Imię użytkownika
        /// </param>
        /// <param name="nazwisko">
        /// Nazwisko użytkownika
        /// </param>
        public UserInfo(string login, string imie, string nazwisko)
        {
            this.imie = imie;
            this.nazwisko = nazwisko;
            this.login = login;
        }
    }

    /// <summary>
    /// Silnik rozpoznawania twarzy. W celu użycia wystarczy stworzyć instancję tej klasy
    /// oraz wywołać metodę RecogniseFace
    /// </summary>
    public partial class FaceRecognitionEngine
    {
        private const int defaultDimension = 6;
        private const int defaultIterations = 100;

        /// <summary>
        /// Zdarzenie wywoływane po utworzeniu nowej bazy danych
        /// </summary>
        public event DataBaseEventHandler OnDataBaseCreated;

        /// <summary>
        /// Zdarzenie wywoływane po wczytaniu istniejącej bazy danych
        /// </summary>
        public event DataBaseEventHandler OnDataBaseLoaded;

        /// <summary>
        /// Zdarzenie wywoływane po udanym rozpoznaniu twarzy użytkownika
        /// </summary>
        public event FaceRecognitionSuccessEventHandler OnFaceRecognitionSuccess;

        /// <summary>
        /// Zdarzenie wywoływane po nieudanym rozpoznaniu twarzy użytkownika
        /// </summary>
        public event FaceRecognitionFailEventHandler OnFaceRecognitionFailed;

        /// <summary>
        /// Tworzy silnik rozpoznawania twarzy budując jednocześnie nową bazę
        /// </summary>
        /// <param name="inputVectors">
        /// Kolekcja wektorów odpowiadających obrazom twarzy.
        /// </param>
        /// <param name="userInfoCollection">
        /// Kolekcja danych o użytkowniku
        /// </param>
        /// <param name="dimension">
        /// Wymiar bazy przestrzeni twarzy w bazie danych. Domyślnie 6.
        /// </param>
        /// <param name="iterations">
        /// Liczba iteracji używana przy budowaniu bazy danych. Domyślnie 100.
        /// </param>
        public FaceRecognitionEngine(IEnumerable<IEnumerable<double>> inputVectors, 
            IEnumerable<IUserInfo> userInfoCollection, string dataBasePath, int dimension = defaultDimension, int iterations = defaultIterations)
        {
            checkDimensions(inputVectors, userInfoCollection);

            dataBaseFileName = dataBasePath;

            List<LearningExample> examples = new List<LearningExample>();

            // Rzutowanie na pojedyncze wektory
            foreach (IEnumerable<double> array in inputVectors)
            {
                examples.Add(new LearningExample(new Vector(array), 0));
            }

            List<IUserInfo> userInfoList = new List<IUserInfo>(userInfoCollection);
            outputDimension = dimension;
            ojIterations = iterations;

            reduction(examples);

            builDataBase(userInfoList, examples);
            if(OnDataBaseCreated != null)
                OnDataBaseCreated(this, new DataBaseEventArgs(dataBasePath));
        }

        /// <summary>
        /// Tworzy silnik rozpoznawania twarzy wczytując bazę z pliku
        /// </summary>
        /// <param name="fileName">
        /// Ścieżka do pliku bazy danych
        /// </param>
        public FaceRecognitionEngine(string fileName)
        {
            loadDataBase(fileName);
            if (OnDataBaseLoaded != null)
                OnDataBaseLoaded(this, new DataBaseEventArgs(fileName));
        }

        /// <summary>
        /// Rozpoznaje, czy twarz podana jako wejściowa odpowiada podanemu loginowi.
        /// Jeśli tak, zwracane są dane użytkownika.
        /// </summary>
        /// <param name="face">
        /// Wektor odpowiadający obrazowi twarzy.
        /// </param>
        /// <param name="login">
        /// Login użytkownika.
        /// </param>
        /// <param name="throwException">
        /// Jeśli prawda, w przypadku nie rozponania twarzy zrzucany jest wyjątek 
        /// (zamiast zwrócenia null).
        /// </param>
        /// <returns>
        /// Zwraca dane użytkownika jeśli podany login odpowiada przesłanemu obrazowi 
        /// lub null w przeciwnym wypadku.
        /// </returns>
        public IUserInfo RecogniseFace(IEnumerable<double> face, string login, bool throwException = false)
        {
            IUserInfo userInfo = dataBase.compareFace(new Vector(face));
            if (userInfo.Login != login)
                userInfo = null;

            if(throwException == true && userInfo == null)
            {
                throw new Exception("Nie rozpoznano twarzy");
            }

            if (userInfo != null)
            {
                if (OnFaceRecognitionSuccess != null)
                {
                    OnFaceRecognitionSuccess(this, new FaceRecognitionSuccessEventArgs(userInfo));
                }
            }
            else
            {
                if (OnFaceRecognitionFailed != null)
                {
                    OnFaceRecognitionFailed(this, new FaceRecognitionFailEventArgs());
                }
            }
                    
            return userInfo;
        }
    }
}
