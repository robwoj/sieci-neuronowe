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
        private Thread workingThread;
        List<LearningExample> examples;
        // Wczytuje obraze z pliku
        Bitmap img;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            topologiaCombo.Items.Add(Topologie.top2x32x32x3);
            topologiaCombo.SelectedItem = Topologie.top2x32x32x3;
            workingThread = new Thread(startLearning);
            img = (Bitmap)System.Drawing.Image.FromFile("cavalier.jpg");
            examples = new List<LearningExample>(img.Width * img.Height);
            setIterTextDelegate += setIterText;
            setImageSoureDelegate += setImageSource;
            setKonsolaTextDelegate += setKonsolaText;
            printLineDelegate += printLine;

            createdPerceptronsCount = 0;
            createdLayersCount = 0;
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
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;

            List<int> lista = parseTopology((string)topologiaCombo.SelectedItem);
            int input = lista[0] + 1;
            lista.RemoveAt(0);
            network = new MLPNetwork(input, lista, perceptronCreated, layerCreated);
            network.OnPerceptronCreated += new PerceptronEvent(perceptronCreated);
            createLearningExamples();

            workingThread = new Thread(startLearning);
            workingThread.Start(0);
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

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            workingThread.Abort();
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

            MessageBox.Show("Rozmiar wejścia: " + img.Width + "x" + 
                img.Height, "Rozpoczynanie odczytywania");
            for (int i = 0; i < img.Width; i++)
            {
                
                for (int j = 0; j < img.Height; j++)
                {
                    //PerceptronLib.Vector point = new PerceptronLib.Vector(3);
                    //point[1] = i;
                    //point[2] = j;

                    //LearningExample ex = new LearningExample(point, new PerceptronLib.Vector());

                    PerceptronLib.Vector result = network.classify(examples[i * img.Height + j]);

                    //MessageBox.Show(result.ToString());
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(255, deNormalizeDouble(result[1]),
                        deNormalizeDouble(result[2]), deNormalizeDouble(result[3]));
                    img.SetPixel(i, j, c);

                    //System.Drawing.Color c = img.GetPixel(i, j);
                    Dispatcher.Invoke(printLineDelegate, "R: " + c.R + " G: " + c.G + " B: " + c.B);
                    Dispatcher.Invoke(setIterTextDelegate, (i * img.Height + j).ToString()
                        + " / " + img.Width * img.Height);
                }
            }

            MessageBox.Show("Zapisywanie pliku");
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
            Dispatcher.Invoke(printLineDelegate, createdLayersCount.ToString() + ": Utworzono warstwę");
        }
    }
}
