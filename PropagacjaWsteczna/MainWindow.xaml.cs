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

namespace PropagacjaWsteczna
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MLPNetwork network;
        private Thread workingThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            topologiaCombo.Items.Add(Topologie.top2x32x32x3);
            topologiaCombo.SelectedItem = Topologie.top2x32x32x3;
            workingThread = new Thread(startLearning);
            
        }

        private List<int> parseTopology(string top)
        {
            List<int> lista = new List<int>();
            int last = 0;

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
            network = new MLPNetwork(input, lista);

            createLearningExamples();

            workingThread = new Thread(startLearning);
            workingThread.Start(0);
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;

            workingThread.Abort();

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
            // Wczytuje obraze z pliku
            Bitmap img = (Bitmap)System.Drawing.Image.FromFile("cavalier.jpg");
            List<LearningExample> examples = new List<LearningExample>(img.Width * img.Height);

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
            Bitmap source = (Bitmap)System.Drawing.Image.FromFile("cavalier.jpg");
            Bitmap img = new Bitmap(source.Width, source.Height);

            MessageBox.Show("Rozpoczynanie odczytywania");
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    PerceptronLib.Vector point = new PerceptronLib.Vector(3);
                    point[1] = i;
                    point[2] = j;

                    LearningExample ex = new LearningExample(point, new PerceptronLib.Vector());

                    PerceptronLib.Vector result = network.classify(ex);

                    //MessageBox.Show(result.ToString());
                    img.SetPixel(i, j, System.Drawing.Color.FromArgb(255, deNormalizeDouble(result[1]),
                        deNormalizeDouble(result[2]), deNormalizeDouble(result[3])));

                    //System.Drawing.Color c = img.GetPixel(i, j);
                    //MessageBox.Show("R: " + c.R + " G: " + c.G + " B: " + c.B);
                        
                }
            }

            img.Save("output.bmp");

            //BitmapImage src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = new Uri("output.bmp", UriKind.Relative);
            //src.EndInit();
            //destImg.Source = src;
            //destImg.Stretch = Stretch.Uniform;

            destImg.Source = new BitmapImage(new Uri("utput.bmp", UriKind.Relative));
        }
    }
}
