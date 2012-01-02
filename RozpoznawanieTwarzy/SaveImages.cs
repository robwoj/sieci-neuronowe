using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.IO;
using PerceptronLib;
using System.Drawing;
using System.Threading;
namespace RozpoznawanieTwarzy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Zapisuje utworzone obrazy na dysku
        /// </summary>
        private void saveImages(List<Perceptron> vectors, int width, int height)
        {
            for (int k = 0; k < vectors.Count; k++)
            {
                Perceptron p = vectors[k];
                Bitmap img = new Bitmap(examplesWidth, examplesHeight);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;
                        byte color = (byte)(p.Weights[index] * 256.0F * width * height);
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(255, color, color, color);
                        img.SetPixel(i, j, c);
                        if (i == 0 && j < 50)
                            printLine("Kolor: " + color);
                    }
                }

                img.Save("output" + (k + 1) + ".jpg");
            }
        }
        private void saveImages2(List<LearningExample> vectors, int width, int height)
        {
            printLine("Zapuisywanie wyników...");
            for (int k = 0; k < vectors.Count; k++)
            {
                LearningExample p = vectors[k];
                Bitmap img = new Bitmap(examplesWidth, examplesHeight);


                // Pętla mająca na celu znalezienie minimów i maksimów
                double min = Double.MaxValue;
                double max = Double.MinValue;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;

                        if (p.Example[index] < min) min = p.Example[index];
                        if (p.Example[index] > max) max = p.Example[index];
                    }
                }
                //printLine("Min = " + min + ", Max = " + max);
                printLine("Min = " + min + ", Max = " + max);

                // Górna granica przedziału
                double ceiling = max - min;

                // Mnożnik - dzielimy przez górną część otrzymując przedział [0,1],
                // a następnie mnożymy razy 256 żeby wartości były z całego zakresu
                // skali szarości
                double mult = 256.0F / ceiling;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;
                        byte color = (byte)((p.Example[index] - min) * mult);
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(255, color, color, color);
                        img.SetPixel(i, j, c);
                    }
                }

                img.Save("output" + (k + 1) + ".jpg");
            }
        }
    }

}