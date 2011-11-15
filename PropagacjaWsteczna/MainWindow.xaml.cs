#region usings

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
using MLPNetworkLib;
using System.Resources;
using System.Threading;
using System.Drawing;
using PerceptronLib;

#endregion 

namespace PropagacjaWsteczna
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MLPNetwork network;
        
        /// <summary>
        /// Wątek uczenia sieci
        /// </summary>
        private Thread workingThread;

        /// <summary>
        /// Wątek rysowania obrazu wyjściowego
        /// </summary>
        private Thread drawingThread;
        List<LearningExample> examples;
        // Wczytuje obraze z pliku
        Bitmap img;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            iteracjaText.Text = 100.ToString();
            
            topologiaCombo.Items.Add("2-3-3-3");
            topologiaCombo.SelectedIndex = 0;
            workingThread = new Thread(startLearning);
            drawingThread = new Thread(startDrawing);
           
            img = (Bitmap)System.Drawing.Image.FromFile("cavalier.jpg");
            examples = new List<LearningExample>(img.Width * img.Height);
            setIterTextDelegate += setIterText;
            setImageSoureDelegate += setImageSource;
            setKonsolaTextDelegate += setKonsolaText;
            setErrorTextDelegate += setErrorText;
            printLineDelegate += printLine;

            createdPerceptronsCount = 0;
            createdLayersCount = 0;

            List<int> lista = parseTopology((string)topologiaCombo.SelectedItem);
            int input = lista[0] + 1;
            lista.RemoveAt(0);

            createLearningExamples();
            network = new MLPNetwork(input, lista, perceptronCreated, layerCreated, examples);
            network.OnPerceptronCreated += new PerceptronEvent(perceptronCreated);
            network.OnNetworkLearned += networkLearned;
            networkLearnedDel = networkLearned;
            network.OnLearningIterationEnded += iterationEnded;

        }

        private List<int> parseTopology(string top)
        {
            List<int> lista = new List<int>();

            string[] arr = top.Split("-".ToCharArray());
            foreach (string str in arr)
            {
                try
                {
                    int liczba = 0;
                    int.TryParse(str, out liczba);
                    Console.Write(liczba.ToString() + " ");
                    lista.Add(liczba);
                }
                catch (ArgumentException e)
                {
                    throw new Exception("Nie można parsować topologii", e);
                }

                Console.WriteLine();
            }

            return lista;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            int liczba = 0;
            try
            {
                int.TryParse(iteracjaText.Text, out liczba);

                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;

                workingThread = new Thread(startLearning);    
                workingThread.Start(liczba);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Zła liczba iteracji");
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;

            workingThread.Abort();

            workingThread = new Thread(startDrawing);
            workingThread.Start();
        }

        private void startDrawing()
        {
            printLearnedImage();
        }

        /// <summary>
        /// Funkcja wykonywana przez wątek
        /// </summary>
        /// <param name="iterations">
        /// Maksymalna liczba iteracji. Musi zostać podany typ int
        /// </param>
        public void startLearning(object iterations)
        {
            if (iterations is int == false)
            {
                throw new InvalidCastException("Liczba iteracji musi być liczbą typu int");
            }

            network.learnNetwork((int)iterations);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            workingThread.Abort();
            drawingThread.Abort();
        }

        /// <summary>
        /// Funkcja przeznaczona do kontruowania zbioru przykładów uczących.
        /// Wczytuje obraz wejściowy i zamienia każdy piksel na odpowiedni 
        /// przykład wejściowy.
        /// </summary>
        private void createLearningExamples()
        {

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    PerceptronLib.Vector point = new PerceptronLib.Vector(3);
                    point[1] = i;
                    point[2] = j;

                    PerceptronLib.Vector value = new PerceptronLib.Vector(4);
                    value[1] = normalizeByte(img.GetPixel(i, j).R);
                    value[2] = normalizeByte(img.GetPixel(i, j).G);
                    value[3] = normalizeByte(img.GetPixel(i, j).B);
                    //value[1] = (double)(img.GetPixel(i, j).R);
                    //value[2] = (double)(img.GetPixel(i, j).G);
                    //value[3] = (double)(img.GetPixel(i, j).B);
                    LearningExample ex = new LearningExample(point, value);

                    examples.Add(ex);
                }
            }
        }

        /// <summary>
        /// Funkcja przeznaczona do normalizacji koloru
        /// </summary>
        private double normalizeByte(byte b)
        {
            return (double)(b) / 256.0F;
        }

        /// <summary>
        /// Funkcja przeznaczona do denormalizacji koloru
        /// </summary>
        private byte deNormalizeDouble(double d)
        {
            return (byte)(d * 256.0F);
        }

        /// <summary>
        /// Funkcja przeznaczona do drukowania obrazu przedstawianego przez nauczoną sieć
        /// </summary>
        private void printLearnedImage()
        {
            //Bitmap source = (Bitmap)System.Drawing.Image.FromFile("cavalier.jpg");
            //img = new Bitmap(source.Width, source.Height);

            //MessageBox.Show("Rozmiar wejścia: " + img.Width + "x" + 
            //    img.Height, "Rozpoczynanie odczytywania");

            Dispatcher.Invoke(printLineDelegate, "Rysowanie obrazu...");
            for (int i = 0; i < img.Width; i++)
            {
                
                for (int j = 0; j < img.Height; j++)
                {
                    //PerceptronLib.Vector point = new PerceptronLib.Vector(3);
                    //point[1] = i;
                    //point[2] = j;

                    //LearningExample ex = new LearningExample(point, new PerceptronLib.Vector());
                    PerceptronLib.Vector result;
                    lock(network)
                    {
                        result = network.classify(examples[i * img.Height + j]);
                    }

                    //MessageBox.Show(result.ToString());
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(255, deNormalizeDouble(result[1]),
                        deNormalizeDouble(result[2]), deNormalizeDouble(result[3]));
                    //System.Drawing.Color c = System.Drawing.Color.FromArgb(255, (byte)(result[1]),
                    //    (byte)(result[2]), (byte)(result[3]));
                    img.SetPixel(i, j, c);

                    if (i * img.Height + j < 5)
                    {
                        Dispatcher.Invoke(printLineDelegate, result[1].ToString() + ", "
                            + result[2] + ", " + result[3]);
                    }
                    //System.Drawing.Color c = img.GetPixel(i, j);
                    //Dispatcher.Invoke(printLineDelegate, "R: " + c.R + " G: " + c.G + " B: " + c.B);
                    //Dispatcher.Invoke(setIterTextDelegate, (i * img.Height + j).ToString()
                    //    + " / " + img.Width * img.Height);
                }
            }

            Dispatcher.Invoke(printLineDelegate, "Zapisywanie pliku...");
            img.Save("output.bmp");
            
            //BitmapImage src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = new Uri("output.bmp", UriKind.Relative);
            //src.EndInit();
            //destImg.Source = src;
            //destImg.Stretch = Stretch.Uniform;

            Dispatcher.Invoke(setImageSoureDelegate, "output.bmp");
        }

        private void setIterText(string str)
        {
            iteracjaText.Text = str;
        }

        private void setImageSource(string fileName)
        {
            destImg.BeginInit();
            //destImg.Source = new BitmapImage(new Uri(fileName, UriKind.Relative));
            destImg.EndInit();
        }

        private void setKonsolaText(string str)
        {
            konsola.Text = str;
        }

        private void printLine(string str)
        {
            konsola.Text += str + "\n";
        }

        private voidatstring setIterTextDelegate;
        private voidatstring setImageSoureDelegate;
        private voidatstring setKonsolaTextDelegate;
        private voidatstring printLineDelegate;
        private delegate void voidatstring(string str);

        private int createdPerceptronsCount;
        private int createdLayersCount;
        private void perceptronCreated(object sender, PerceptronEventArgs e)
        {
            createdPerceptronsCount++;
            Dispatcher.Invoke(printLineDelegate, createdPerceptronsCount.ToString() + ": Utworzono perceptron: " + e.Perceptron);
        }

        private void layerCreated(object sender, LayerEventArgs e)
        {
            createdLayersCount++;
            Dispatcher.Invoke(printLineDelegate, "Utworzono warstwę " + createdLayersCount.ToString());
        }

        private void networkCreated(object sender, NetworkEventArgs e)
        {

        }

        private void networkLearned(object sender, NetworkEventArgs e)
        {
            Dispatcher.Invoke(networkLearnedDel);
        }

        private delegate void voidatvoid();

        private voidatvoid networkLearnedDel;

        private void networkLearned()
        {
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;

            drawingThread = new Thread(startDrawing);
            drawingThread.Start();
        }

        private void iterationEnded(object sender, NetworkEventArgs e)
        {
            if (e is NetworkLearningIterationEventArgs)
            {
                //Dispatcher.Invoke(printLineDelegate, ((NetworkLearningIterationEventArgs)e).IterationNumber.ToString()
                //    + ": " + e.Network.globalError());
                NetworkLearningIterationEventArgs ea = (NetworkLearningIterationEventArgs)e;
                Dispatcher.Invoke(setIterTextDelegate, ea.IterationNumber.ToString());
                Dispatcher.Invoke(setErrorTextDelegate, ea.Network.globalError().ToString());
            }
        }

        private void setErrorText(string error)
        {
            errorText.Text = error;
        }

        private voidatstring setErrorTextDelegate;
    }
}
