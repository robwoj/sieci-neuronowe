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
        private List<Thread> workingThreads;

        /// <summary>
        /// Wątek rysowania obrazu wyjściowego
        /// </summary>
        private Thread drawingThread;
        List<LearningExample> examples;
        // Wczytuje obraze z pliku
        Bitmap img;

        public MainWindow()
        {
            windowLoaded = false;
            InitializeComponent();
        }

        private bool windowLoaded;
        private DateTime lastTime;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            etaText.Text = (0.5).ToString();

            iteracjaText.Text = 10000000.ToString();
            lastTime = DateTime.Now;
            lastIterNum = 0;

            topologiaCombo.Items.Add("18-32-32-32-3");
            topologiaCombo.Items.Add("120-64-32-3");
            topologiaCombo.Items.Add("30-32-32-3");
            topologiaCombo.Items.Add("2-32-32-3");
            topologiaCombo.Items.Add("2-64-64-3");
            topologiaCombo.Items.Add("2-3-3-3");
            topologiaCombo.SelectedIndex = 0;
            workingThreads = new List<Thread>();
            workingThreads.Add(new Thread(startLearning));
            //workingThreads.Add(new Thread(startLearning));
            drawingThread = new Thread(startDrawing);
           
            img = (Bitmap)System.Drawing.Image.FromFile("cavalier.jpg");
            examples = new List<LearningExample>(img.Width * img.Height);
            setIterTextDelegate += setIterText;
            setImageSoureDelegate += setImageSource;
            setKonsolaTextDelegate += setKonsolaText;
            setErrorTextDelegate += setErrorText;
            setProgressValueDelegate += setProgressValue;
            printLineDelegate += printLine;
            getEtaDelegate += getEta;

            createdPerceptronsCount = 0;
            createdLayersCount = 0;

            windowLoaded = true;

            createNetwork();
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

        private int liczbaIteracji;
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            liczbaIteracji = 0;
            try
            {
                int.TryParse(iteracjaText.Text, out liczbaIteracji);

                progressBar.Minimum = 0;
                progressBar.Maximum = liczbaIteracji;
                progressBar.Value = progressBar.Minimum;

                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;

                lastTime = DateTime.Now;
                for (int i = 0; i < workingThreads.Count; i++ )
                {
                    workingThreads[i] = new Thread(startLearning);
                    workingThreads[i].Start(liczbaIteracji);
                }
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

            
            for (int i = 0; i < workingThreads.Count; i++ )
            {
                workingThreads[i].Abort();
            }

            drawingThread = new Thread(startDrawing);
            drawingThread.Start();
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

            network.learnNetwork((int)iterations,
                (double)Dispatcher.Invoke(getEtaDelegate));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < workingThreads.Count; i++)
            {
                workingThreads[i].Abort();
            }
            drawingThread.Abort();
        }

        private void setIterText(string str)
        {
            iteracjaText.Text = str;
        }

        private void setImageSource(string fileName)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.BeginInit();
            img.Source = new BitmapImage(new Uri(fileName, UriKind.Relative));
            //destImg.Source = new BitmapImage(new Uri(fileName, UriKind.Relative));
            img.EndInit();

            mainGrid.Children.Add(img);
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
        private voidatint setProgressValueDelegate;
        private delegate void voidatstring(string str);
        private delegate void voidatint(int val);


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
                NetworkLearningIterationEventArgs ea = (NetworkLearningIterationEventArgs)e;
                
                Dispatcher.Invoke(setIterTextDelegate, ea.IterationNumber.ToString());
                Dispatcher.Invoke(setErrorTextDelegate, ea.Network.globalError().ToString());
                Dispatcher.Invoke(setProgressValueDelegate, ea.IterationNumber);

                //Dispatcher.Invoke(printLineDelegate, ea.Network.delty[0].ToString());
                //Dispatcher.Invoke(printLineDelegate, ea.Network.ClassificationExamples.Last().ToString());
                //Dispatcher.Invoke(printLineDelegate, "Iloczyn skalarny ostatniej warstwy: " + 
                //    (ea.Network.ClassificationExamples[ea.Network.ClassificationExamples.Count - 2].Example
                //    * ea.Network.Layers[2].Perceptrons[0].Weights).ToString());

            }
        }

        private void setErrorText(string error)
        {
            errorText.Text = error;
        }

        private voidatstring setErrorTextDelegate;

        private void setProgressValue(int val)
        {
            DateTime now = DateTime.Now;
            //MessageBox.Show(((double)(now - lastTime).TotalSeconds).ToString());
            progressBar.Value = val;
            double v = Math.Round((val - lastIterNum) / (double)(now - lastTime).TotalSeconds, 2);
            lastIterNum = val;
            lastTime = now;

            timeLabel.Content = v.ToString() + "/s";
            progressBar.ToolTip = Math.Round(((double)val / progressBar.Maximum) * 100.0F, 2).ToString() + "%";
        }

        private int lastIterNum;

        private void topologiaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (windowLoaded == true)
            {
                createNetwork();
            }
        }

        private void createNetwork()
        {
            konsola.Text = "";
            List<int> lista = parseTopology((string)topologiaCombo.SelectedItem);
            int input = lista[0] + 1;
            lista.RemoveAt(0);
            createLearningExamples(input);
            network = new MLPNetwork(input, lista, perceptronCreated, layerCreated, examples);
            network.OnPerceptronCreated += new PerceptronEvent(perceptronCreated);
            network.OnNetworkLearned += networkLearned;
            networkLearnedDel = networkLearned;
            network.OnLearningIterationEnded += iterationEnded;
            //printLine("Błąd globalny: " + network.globalError());
            errorText.Text = network.globalError().ToString();
        }

        private delegate double doubleatvoid();
        private doubleatvoid getEtaDelegate;
        private double getEta()
        {
            return double.Parse(etaText.Text);
        }

    }
}
