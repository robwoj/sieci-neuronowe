﻿using System;
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
using System.Windows.Forms;
using System.IO;
using PerceptronLib;
using System.Drawing;
namespace RozpoznawanieTwarzy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog openDialog;
        private List<LearningExample> examples;
        private int iterations;
        private int examplesWidth;
        private int examplesHeight;
        public MainWindow()
        {
            InitializeComponent();
            openDialog = new FolderBrowserDialog();
            openDialog.Description = "Podaj katalog z przykładami";

            learnButton.IsEnabled = false;
            iterations = 10;
            //openDialog.Filter = "Plik JPEG (*.jpg)|*.jpg";
            //openDialog.FilterIndex = 1;
            
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            openDialog.ShowDialog();
            if (openDialog.SelectedPath != null)
            {
                printLine("Wybrano katalog " + openDialog.SelectedPath);
                DirectoryInfo di = null;
                try
                {
                    di = new DirectoryInfo(openDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    printLine("Nie można otworzyć katalogu " + openDialog.SelectedPath
                        + "\n\t" + ex.Message);
                    return;
                }

                List<FileInfo> files = new List<FileInfo>();
                foreach (FileInfo f in di.GetFiles())
                {
                    if (f.Extension.ToLower() == ".jpg")
                    {
                        //printLine(f.Name);
                        files.Add(f);
                    }
                }

                examples = createLearningExamples(files);

                if (examples != null)
                    learnButton.IsEnabled = true;
            }
        }

        private void saveImages(List<Perceptron> vectors, int width, int height)
        {
            for (int k = 0; k < vectors.Count; k++ )
            {
                Perceptron p = vectors[k];
                Bitmap img = new Bitmap(examplesWidth, examplesHeight);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = i * height + j;
                        byte color = (byte)(p.Weights[index] * 256.0F);
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(255, color, color, color);
                        img.SetPixel(i, j, c);
                    }
                }

                img.Save("output" + (k + 1) + ".jpg");
            }
        }

        private List<LearningExample> createLearningExamples(List<FileInfo> files)
        {
            List<LearningExample> examples = new List<LearningExample>();

            foreach (FileInfo f in files)
            {
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(f.FullName);
                examplesWidth = bitmap.Width;
                examplesHeight = bitmap.Height;

                printLine(f.Name + ": " + bitmap.Width + "x" + bitmap.Height);

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
                        p.Weights[index] = (double)bitmap.GetPixel(i, j).R / 256.0F;
                        //if (min > p.Weights[index]) min = p.Weights[index];
                        //if (max < p.Weights[index]) max = p.Weights[index];
                    }
                }

                printLine("Max = " + max + ", Min = " + min);
                examples.Add(new LearningExample(p.Weights, 0));
            }

            return examples;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void reduction(List<LearningExample> exampleList)
        {
            List<LearningExample> list = exampleList;
            int dimension = exampleList[0].Example.Dimension;
            int m = 6;
            List<Perceptron> principalComponents = new List<Perceptron>(m);
            for (int i = 0; i < m; i++)
            {
                printLine("i = " + i);
                //Perceptron p = new Perceptron(dimension);
                //for (int j = 0; j < dimension; j++)
                //{
                //    p.Weights[j] /= (double)dimension;
                //}

                //printLine("[" + p.Weights[0] + ";" + p.Weights[1] + ";" + p.Weights[2] + "]");
                principalComponents.Add(ojLearn(list));
                Perceptron p = principalComponents[i];
                printLine("[" + p.Weights[0] + ";" + p.Weights[1] + ";" + p.Weights[2] + "]");
                List<LearningExample> nextList = new List<LearningExample>();
                foreach (LearningExample ex in list)
                {
                    double val = p.Weights * p.Weights;
                    double activation = p.Weights * ex.Example;
                    if (activation == 0 || val == 0)
                        printLine("act = " + activation + ", val = " + val);
                    PerceptronLib.Vector nextExVector = new PerceptronLib.Vector(dimension);
                    for (int j = 0; j < dimension; j++)
                    {
                        nextExVector[j] = ex.Example[j] - p.Weights[j] * val / activation;
                    }
                    LearningExample nextEx = new LearningExample(nextExVector, 0);
                    nextList.Add(nextEx);
                }
                list = nextList;
            }

            saveImages(principalComponents, examplesWidth, examplesHeight);
        }

        private Perceptron ojLearn(List<LearningExample> exampleList)
        {
            Random r = new Random();
            double eta = 0.5;
            Perceptron perceptron = new Perceptron(exampleList[0].Example.Dimension);
            for (int i = 0; i < iterations; i++)
            {
                LearningExample ex = exampleList[r.Next(exampleList.Count)];
                perceptron = new Perceptron(ex.Example.Dimension);
                double activation = perceptron.Weights * ex.Example * eta;
                //printLine("oj-ex: [" + ex.Example[0] + ";" + ex.Example[1] + ";"
                //    + ex.Example[2] + "]");
                                              
                printLine("act: " + activation);
                for (int j = 0; j < perceptron.Dimension; j++)
                {
                    perceptron.Weights[j] += activation * (ex.Example[j] - 
                        activation * perceptron.Weights[j]);
                }
                printLine("oj[" + i + "]: [" + perceptron.Weights[0] + ";" + perceptron.Weights[1] + ";"
                    + perceptron.Weights[2] + "]");
            }

            return perceptron;
        }

        private void printLine(string str)
        {
            konsola.Text += str + "\n";
        }

        private void learnButton_Click(object sender, RoutedEventArgs e)
        {
            if (examples != null)
            {
                reduction(examples);
            }
        }

        private delegate void voidatstring(string str);

        private voidatstring printLineDelegate;

        //private void pl(string str)
        //{
        //    Dispatcher.Invoke();
        //}
    }
}
