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
           
            printLine("Zapisywanie wyników...");
            for (int l = 0; l < examples.Count; l++)
            {
                LearningExample ex = examples[l];
                int dimension = examples[0].Example.Dimension;
                for (int k = 0; k < outputDimension; k++)
                {
                    Perceptron p = vectors[k];
                    Bitmap img = new Bitmap(examplesWidth, examplesHeight);

                    double val = p.Weights * p.Weights;
                    double activation = p.Weights * ex.Example;
                    PerceptronLib.Vector nextExVector = new PerceptronLib.Vector(dimension);
                    for (int j = 0; j < dimension; j++)
                    {
                        nextExVector[j] = ex.Example[j] - p.Weights[j] * activation / val;
                    }
                    ex = new LearningExample(nextExVector, 0);

                    //printLine("Min = " + min + ", Max = " + max);
                    //printLine("Min = " + min + ", Max = " + max);

                    normalizeRange(ex, 256.0F, width, height);


                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            int index = i * height + j;
                            byte color = (byte)(ex.Example[index]);
                            System.Drawing.Color c = System.Drawing.Color.FromArgb(255, color, color, color);
                            img.SetPixel(i, j, c);
                        }
                    }

                    img.Save("output" + (l + 1) + "-" + (k + 1) + ".jpg");
                }
            }
        }

        /// <summary>
        /// Normalizuje wektor na przedział [0,ceiling]
        /// </summary>
        /// <param name="ceiling">
        /// Górna granica przedziału
        /// </param>
        private void normalizeRange(LearningExample ex, double ceiling, int width, int height)
        {
            // Pętla mająca na celu znalezienie minimów i maksimów
            double min = Double.MaxValue;
            double max = Double.MinValue;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i * height + j;

                    if (ex.Example[index] < min) min = ex.Example[index];
                    if (ex.Example[index] > max) max = ex.Example[index];
                }
            }

            // Górna granica przedziału
            double ceil = max - min;

            // Mnożnik - dzielimy przez górną część otrzymując przedział [0,1],
            // a następnie mnożymy razy 256 żeby wartości były z całego zakresu
            // skali szarości
            double mult = ceiling / ceil;

            modifyEx(ex, min, mult, width, height);
        }


        /// <summary>
        /// Modyfikuje wartości wektora w celu normalizacji
        /// </summary>
        private void modifyEx(LearningExample ex, double min, double mult, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i * height + j; 
                    ex.Example[index] = (ex.Example[index] - min) * mult;
                }
            }
        }
    }

}