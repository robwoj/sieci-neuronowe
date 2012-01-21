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
        private List<PerceptronLib.Vector> createLearningExamples(List<FileInfo> files)
        {
            List<PerceptronLib.Vector> examples = new List<PerceptronLib.Vector>();
            foreach (FileInfo f in files)
            {
                double sum = 0.0F;
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(f.FullName);
                examplesWidth = bitmap.Width;
                examplesHeight = bitmap.Height;

                //printLine(f.Name + ": " + bitmap.Width + "x" + bitmap.Height);
                printLine(f.Name + ": " + bitmap.Width + "x" + bitmap.Height);
                PerceptronLib.Vector v = new PerceptronLib.Vector(bitmap.Width * bitmap.Height);

                int width = bitmap.Width;
                int height = bitmap.Height;
                double max = 0;
                double min = 255;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;
                        //v[index] = (double)bitmap.GetPixel(i, j).R / (256.0F * width * height) * 3000;
                        v[index] = (double)bitmap.GetPixel(i, j).R;
                        sum += v[index];
                        if (min > v[index]) min = v[index];
                        if (max < v[index]) max = v[index];


                    }
                }
                double medium = sum / (double)(width * height);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;
                        v[index] -= medium;
                    }
                }

                v.normalizeWeights();
                //printLine("Max = " + max + ", Min = " + min + ", Sum = " + sum
                //    + ", medium = " + medium);
                examples.Add(v);
            }

            return examples;
        }
    }
}