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
        /// <summary>
        /// Funkcja przeznaczona do kontruowania zbioru przykładów uczących.
        /// Wczytuje obraz wejściowy i zamienia każdy piksel na odpowiedni 
        /// przykład wejściowy.
        /// </summary>
        private void createLearningExamples(int input)
        {
            //MessageBox.Show("Liczba przykładów uczących: " + img.Width * img.Height, "Tworzenie przykładów");
            examples = new List<LearningExample>();
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    PerceptronLib.Vector point = new PerceptronLib.Vector(input);
                    point[0] = 1;
                    point[1] = (double)i / ((double)img.Width * 5.0F);
                    point[2] = (double)j / ((double)img.Height * 5.0F);
                    for (int k = 0; k < (input - 3) / 2 - 1; k++)
                    {
                        point[2 * k + 3] = Math.Abs(Math.Sin((k + 1) * point[1]));
                        point[2 * k + 4] = Math.Abs(Math.Sin((k + 1) * point[2]));
                    }
                    //point[3] = Math.Sin(point[1]);
                    //point[4] = Math.Sin(point[2]);
                    //point[5] = Math.Sin(point[1] + point[2]);
                    //point[6] = Math.Sin(2 * point[1] + point[2]);
                    //point[7] = Math.Sin(point[1] + 2 * point[2]);
                    //point[8] = Math.Sin(point[1] * point[1]);
                    //point[9] = Math.Sin(point[2] * point[2]);
                    //point[10] = Math.Sin(2 * (point[1] + point[2]));
                    //point[11] = Math.Sin(2 * point[1]);
                    //point[12] = Math.Sin(3 * point[1]);
                    //point[13] = Math.Sin(2 * point[2]);
                    //point[14] = Math.Sin(3 * point[2]);

                    PerceptronLib.Vector value = new PerceptronLib.Vector(4);
                    value[1] = normalizeByte(img.GetPixel(i, j).R);
                    value[2] = normalizeByte(img.GetPixel(i, j).G);
                    value[3] = normalizeByte(img.GetPixel(i, j).B);
                    //value[1] = (img.GetPixel(i, j).R);
                    //value[2] = (img.GetPixel(i, j).G);
                    //value[3] = (img.GetPixel(i, j).B);

                    value.round();
                    point.round();
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
            return ((double)(b) / 256.0F) * 0.8F + 0.1;
        }

        /// <summary>
        /// Funkcja przeznaczona do denormalizacji koloru
        /// </summary>
        private byte deNormalizeDouble(double d)
        {
            return (byte)(((d - 0.1) / 0.8F) * 256.0F);
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
                    lock (network)
                    {
                        result = network.classify(examples[i * img.Height + j]);
                    }

                    //MessageBox.Show(result.ToString());
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(255, deNormalizeDouble(result[1]),
                        deNormalizeDouble(result[2]), deNormalizeDouble(result[3]));
                    //System.Drawing.Color c = System.Drawing.Color.FromArgb(255, (byte)(result[1]),
                    //    (byte)(result[2]), (byte)(result[3]));
                    img.SetPixel(i, j, c);

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

    }
}