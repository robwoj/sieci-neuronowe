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
                    }
                }

                img.Save("output" + (k + 1) + ".jpg");
            }
        }
    }

}