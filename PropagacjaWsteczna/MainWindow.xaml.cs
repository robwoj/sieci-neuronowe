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
            int input = lista[0];
            lista.RemoveAt(0);
            network = new MLPNetwork(input, lista);

            workingThread.Start(0);
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;

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
    }
}
