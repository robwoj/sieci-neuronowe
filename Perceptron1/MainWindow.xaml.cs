using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Perceptron1;
using PerceptronLib;
using System.Threading;
using MLPNetworkLib;

namespace Perceptron2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread networkThread;
        private bool isNetworkCreated;
        private delegate void voidF();
        private delegate void voidFstr(string str);
        private voidF setLPDel;
        private voidFstr printFDel;
        private MLPNetwork network;

        private Perceptron perceptron;
        private List<LearningExample> learningExamples;

        private getDoubleF getStalaDel;
        private getDoubleF getLiczbaDel;

        private delegate double getDoubleF();
        private delegate void perceptronEventHandler();

        public MainWindow()
        {
            InitializeComponent();
            perceptron = new Perceptron(3);
            learningExamples = new List<LearningExample>();

            liczbaScroll.Value = 100;
            stalaScroll.Value = 0.5;
            isNetworkCreated = false;

        }

        /// <summary>
        /// Funkcja przeznaczona do drukowania linii
        /// </summary>
        private void println(string str)
        {
            console.Text += str + "\n";
        }

        /// <summary>
        /// Funkcja przeznaczona do rysowania punktu
        /// </summary>
        private void drawEllipse(Point p, double exClass)
        {
            // Tworzy elipsę
            Ellipse e = new Ellipse();

            // Ustawia wymiary elipsy
            e.Height = 5;
            e.Width = 5;

            // Ustawia położenie elipsy
            e.Margin = new Thickness(p.X + 100.0F, p.Y + 100.0F, 0, 0);

            // Ustawia kolor elipsy w zależności od klasy
            if (exClass == 1)
            {
                e.Fill = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Fill = new SolidColorBrush(Colors.Blue);
            }

            // Rysuje elipsę
            canvas.Children.Add(e);
            
        }

        /// <summary>
        /// Funkcja przeznaczona do dodawania przykładu uczącego do listy 
        /// </summary>
        private void addLearningExample(Point p, double exClass)
        {
            // Tworzy nowy wektor
            PerceptronLib.Vector v = new PerceptronLib.Vector(3);
            v[0] = 1.0F;
            v[1] = p.X;
            v[2] = p.Y;

            // Tworzy przykład uczący
            learningExamples.Add(new LearningExample(v, exClass));
        }

        /// <summary>
        /// Funkcja przeznaczona do drukowania punktu
        /// </summary>
        private void printPoint(Point p)
        {
            println("(" + p.X + "," + p.Y + ")");
        }

        /// <summary>
        /// Funkcja przezaczona do dodawanie prykładów uczących do listy
        /// </summary>
        private void addExample(bool isLeft, Point p)
        {
            // Przpisuje oczekiwane wartości do punktu
            double expectedValue = isLeft ? 1 : -1;

            // Przesuwa środek układu współrzędnych
            p.X -= 100.0F;
            p.Y -= 100.0F;

            // Dodaje przykład
            addLearningExample(p, expectedValue);

            // Rysuje punkt
            drawEllipse(p, expectedValue);

            // Drukuje współrzędne punktu 
            printPoint(p);

        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie naciśnięciaa lewego przycisku myszy, 
        /// dodaje nowy przykład uczący
        /// </summary>
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            addExample(true, e.GetPosition(canvas));

        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie naciśnięcia prawego przycisku myszy, 
        /// dodaje nowy przykład uczący
        /// </summary>
        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            addExample(false, e.GetPosition(canvas));
        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie naciśnięcia przycisku uczenia perceptronu
        /// </summary>
        private void pocketButton_Click(object sender, RoutedEventArgs e)
        {
            perceptron = new Perceptron(3);
            println(perceptron.Weights.ToString());

            // Uczy perceptron
            println(perceptron.pocketLearn((int)liczbaScroll.Value, stalaScroll.Value, learningExamples).ToString());

            // Rysuje linie na podstawie nauczonego perceptronu
            drawLine();
        }

        /// <summary>
        /// Funkcja przeznaczona przywrócenia domyślnych wartości interfejsu
        /// </summary>
        private void resetAll()
        {
            learningExamples = new List<LearningExample>();
            perceptron = new Perceptron(3);
            canvas.Children.Clear();
            console.Text = "";
            network = new MLPNetwork(3, learningExamples);
            isNetworkCreated = false;
            liczbaPerceptronowText.Text = "0";
        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie naciśnięcia przycisku wyczyść
        /// </summary>
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie zmiany wartości paska 
        /// </summary>
        private void liczbaScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            liczbaText.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie zmiany wartości paska 
        /// </summary>
        private void stalaScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            stalaText.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie utworzenia sieci.
        /// </summary>
        public void networkCreated(object sender, NetworkEventArgs e)
        {
            isNetworkCreated = true;

            // Wypisuje komunkiat
            Dispatcher.Invoke(printFDel, "Utworzono siec, " + e.Network.LayersCount + " warstw");
        }

        /// <summary>
        /// Funkcja przeznaczona do tworzenia nowej sieci.
        /// </summary>
        private void createNetwork()
        {
            // Tworzy nowy obiekt i przekazuje przykłady uczące
            network = new MLPNetwork(3, learningExamples);

            // Przypisanie obsługi zdarzeń
            network.OnNetworkCreated += networkCreated;
            network.OnPerceptronCreated += perceptronCreated;

            // Tworzy sieć
            network.createNetwork((int)((double)Dispatcher.Invoke(getLiczbaDel)),
                (double)Dispatcher.Invoke(getStalaDel));

            // Wypisuje kolejne przykłady uczące i ich klasyfikacje
            foreach (LearningExample l in learningExamples)
            {
                Dispatcher.Invoke(printFDel, l.Example.ToString() + " => " + network.classify(l) +
                    " (Wartość oczekiwana: " + l.ExpectedDoubleValue + ")");
            }
        }


        /// <summary>
        /// Przeznacznona dla dispatchera
        /// </summary>
        private double getStala()
        {
            return stalaScroll.Value;
        }

        /// <summary>
        /// Przeznacznona dla dispatchera
        /// </summary>
        private double getLiczba()
        {
            return liczbaScroll.Value;
        }

        /// <summary>
        /// Zdarzenie przechwytywane w monecie naciśnięcia przycisku tworzenia nowej sieci.
        /// </summary>
        private void networkButton_Click(object sender, RoutedEventArgs e)
        {
            // Inicjuje delegaty 
            getStalaDel += getStala;
            getLiczbaDel += getLiczba;
            printFDel += println;
            setLPDel += setLiczbaPerceptronowText;

            // Tworzy i wywołuje wątek liczenia sieci
            networkThread = new Thread(createNetwork);
            networkThread.Start();
        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie dodania nowego perceptronu, po zakończeniu uczenia
        /// </summary>
        private void perceptronCreated(object sender, PerceptronEventArgs e)
        {
            Dispatcher.Invoke(setLPDel);
        }

        /// <summary>
        /// Funkcja przeznaczona do inkrementacji pola zawierającego liczbę utworzonych perceptronów.
        /// Zawarte w funkcji żeby można było wykorzystać dispatcher
        /// </summary>
        private void setLiczbaPerceptronowText()
        {
            liczbaPerceptronowText.Text = (Double.Parse(liczbaPerceptronowText.Text) + 1).ToString();
        }

        /// <summary>
        /// Zdarzenie przechwytywane w momencie poruszania myszą na canvasie.
        /// Ma na celu pokacywanie, jak klasyfikowane są odpowiednie punkty na płaszczyźnie
        /// przez stworzoną sieć.
        /// </summary>
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Sprawdzamy, czy sieć została już utworzona
            if (isNetworkCreated)
            {
                Point p = e.GetPosition(canvas);

                // Tworzy wektor przeznaczony do przeliczenia
                PerceptronLib.Vector v = new PerceptronLib.Vector(3);
                v[0] = 1;
                v[1] = p.X - 100;
                v[2] = p.Y - 100;

                // Tworzy obiekt przykładu
                LearningExample ex = new LearningExample(v, 0);

                // Sprawdza, na jaki kolor należy pomalować prostokąt
                if (network.classify(ex)[1] == 1)
                {
                    // Koloruje na czerwono
                    classRect.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    // Koloruje na niebiesko
                    classRect.Fill = new SolidColorBrush(Colors.Blue);
                }
            }
        }

        /// <summary>
        /// Zdarzenie mające na celu usunięcie ewentualnie rozpoczętego i niezakończonego wątku
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (networkThread != null)
            {
                // Kończy wątek tworzenia sieci neuronowej
                networkThread.Abort();
            }
        }
    }

}
