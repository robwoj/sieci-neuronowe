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
        /// Tworzy listę przykładów uczących na podstawie obrazów wczytanych z dysku
        /// </summary>
        private List<LearningExample> createLearningExamples(List<FileInfo> files)
        {
            List<LearningExample> examples = new List<LearningExample>();
            double sum = 0.0F;
            foreach (FileInfo f in files)
            {
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(f.FullName);
                examplesWidth = bitmap.Width;
                examplesHeight = bitmap.Height;

                //printLine(f.Name + ": " + bitmap.Width + "x" + bitmap.Height);
                Dispatcher.Invoke(printLineDelegate, f.Name + ": " + bitmap.Width + "x" + bitmap.Height);
                Perceptron p = new Perceptron(bitmap.Width * bitmap.Height);

                int width = bitmap.Width;
                int height = bitmap.Height;
                double max = 0;
                double min = 255;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;
                        p.Weights[index] = (double)bitmap.GetPixel(i, j).R / (256.0F * width * height);
                        sum += p.Weights[index];
                        if (min > p.Weights[index]) min = p.Weights[index];
                        if (max < p.Weights[index]) max = p.Weights[index];


                    }
                }
                double medium = sum / (double)(width * height);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;
                        p.Weights[index] -= medium;
                    }
                }

                //printLine("Max = " + max + ", Min = " + min + ", Sum = " + sum
                //    + ", " + medium);
                examples.Add(new LearningExample(p.Weights, 0));
            }

            return examples;
        }
    }
}