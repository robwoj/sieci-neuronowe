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

namespace Perceptron2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Funkcja przeznaczona do narysowania linii na podstawie wektora wag nauczonego perceptronu
        /// </summary>
        private void drawLine()
        {
            Line l = new Line();

            // Drukuje współczynnik kierunkowy prostej
            //println("Wspołczynnik kierunkowy: " + perceptron.Weights[1] / perceptron.Weights[2]
            //    + ", przesunięcie: " + (-perceptron.Weights[0] / perceptron.Weights[2]));

            double w0 = perceptron.Weights[0];
            double w1 = perceptron.Weights[1];
            double w2 = perceptron.Weights[2];

            // Przypisuje współrzędne punktów początkowego i końcowego prostej
            l.X1 = 0;
            l.Y1 = (w1 * 100.0F - w0)/ w2
              + 100;
            l.X2 = 200;
            l.Y2 = -(w1 * 100.0F + w0) / w2
                + 100;

            // Ustawia kolor prostej
            l.Stroke = new SolidColorBrush(Colors.Green);

            // Ustawia grubość prostej
            l.StrokeThickness = 1;

            // Rysuje prostą
            canvas.Children.Add(l);
        }

    }
}