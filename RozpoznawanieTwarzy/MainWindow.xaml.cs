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
        private FolderBrowserDialog openDialog;
        private List<LearningExample> examples;
        private int iterations;
        private int examplesWidth;
        private int examplesHeight;

        Thread creatingExamplesThread;
        Thread savingImagesThread;
        Thread reducingThread;
        List<FileInfo> files;

        public MainWindow()
        {
            InitializeComponent();
            openDialog = new FolderBrowserDialog();
            openDialog.Description = "Podaj katalog z przykładami";

            learnButton.IsEnabled = false;
            iterations = 100;
            printLineDelegate = printLine;
            saveImagesDelegate = saveImages;
            createLearningExamplesDelegate = createLearningExamples;
            getFilesDelegate = getFiles;
            setExamplesDelegate = setExamples;
            OnStateChange = stateChange;    
            files = null;
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

                files = new List<FileInfo>();
                foreach (FileInfo f in di.GetFiles())
                {
                    if (f.Extension.ToLower() == ".jpg")
                    {
                        //printLine(f.Name);
                        files.Add(f);
                    }
                }

                creatingExamplesThread = new Thread(startCreatingExamples);
                creatingExamplesThread.Start();
                
                //creatingExamplesThread.Join();
                //examples = createLearningExamples(files);

            }
        }

        private delegate void stateEvent(object sender, EventArgs e);

        private void stateChange(object sender, EventArgs e)
        {
            if (examples != null)
                learnButton.IsEnabled = true;
        }

        private event stateEvent OnStateChange;
        
        private List<FileInfo> getFiles()
        {
            if (files == null)
            {
                throw new Exception("Nie wczytano plików");
            }
            else
            {
                return files;
            }
        }

        private void setExamples(List<LearningExample> examplesList)
        {
            examples = examplesList;
        }

        private delegate void voidatexamplelist(List<LearningExample> exampleList);
        private voidatexamplelist setExamplesDelegate;
        private delegate List<FileInfo> fileinfolistatvoid();
        private fileinfolistatvoid getFilesDelegate; 

        private void startCreatingExamples()
        {
            List<LearningExample> exList = 
                createLearningExamples((List<FileInfo>)Dispatcher.Invoke(getFilesDelegate));

            Dispatcher.Invoke(setExamplesDelegate, exList);

            Dispatcher.Invoke(OnStateChange, this, new EventArgs());
        }

        private delegate void imageSaveFunc(List<Perceptron> vectors, int width, int height);

        private imageSaveFunc saveImagesDelegate;

        /// <summary>
        /// Zapisuje utworzone obrazy na dysku
        /// </summary>
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
                        byte color = (byte)(p.Weights[index] * 256.0F * width * height);
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(255, color, color, color);
                        img.SetPixel(i, j, c);
                    }
                }

                img.Save("output" + (k + 1) + ".jpg");
            }
        }


        private delegate List<LearningExample> examplesCreatingFunc(List<FileInfo> files);

        private examplesCreatingFunc createLearningExamplesDelegate;

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

        /// <summary>
        /// Reakcja na zdarzenie naciśnięcia przycisku, przeznaczona do szybkiego zamykania aplikacji
        /// za pomocą Escape
        /// </summary>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        /// <summary>
        /// Algorytm redukcji slładowych głównych
        /// </summary>
        private void reduction(List<LearningExample> exampleList)
        {
            List<LearningExample> list = exampleList;
            int dimension = exampleList[0].Example.Dimension;
            int m = 2;
            List<Perceptron> principalComponents = new List<Perceptron>(m);
            for (int i = 0; i < m; i++)
            {
                //printLine("i = " + i);
                //Perceptron p = new Perceptron(dimension);
                //for (int j = 0; j < dimension; j++)
                //{
                //    p.Weights[j] /= (double)dimension;
                //}

                //printLine("[" + p.Weights[0] + ";" + p.Weights[1] + ";" + p.Weights[2] + "]");
                principalComponents.Add(ojLearn(list));
                Perceptron p = principalComponents[i];
                //printLine("[" + p.Weights[0] + ";" + p.Weights[1] + ";" + p.Weights[2] + "]");
                List<LearningExample> nextList = new List<LearningExample>();
                foreach (LearningExample ex in list)
                {
                    double val = p.Weights * p.Weights;
                    double activation = p.Weights * ex.Example;
                    //if (activation == 0 || val == 0)
                    //    printLine("act = " + activation + ", val = " + val);
                    PerceptronLib.Vector nextExVector = new PerceptronLib.Vector(dimension);
                    for (int j = 0; j < dimension; j++)
                    {
                        nextExVector[j] = ex.Example[j] - p.Weights[j] * activation / val;
                    }
                    LearningExample nextEx = new LearningExample(nextExVector, 0);
                    nextList.Add(nextEx);
                }
                list = nextList;
            }

            saveImages(principalComponents, examplesWidth, examplesHeight);
        }

        /// <summary>
        /// Algorytm Oja 
        /// </summary>
        private Perceptron ojLearn(List<LearningExample> exampleList)
        {
            Random r = new Random();
            double eta = 0.5;
            Perceptron perceptron = new Perceptron(exampleList[0].Example.Dimension);
            perceptron.Weights.normalizeWeights();
            printVectorLength(perceptron.Weights);

            for (int i = 0; i < iterations; i++)
            {
                LearningExample ex = exampleList[r.Next(exampleList.Count)];
                double activation = perceptron.Weights * ex.Example * eta;
                //printLine("oj-ex: [" + ex.Example[0] + ";" + ex.Example[1] + ";"
                //    + ex.Example[2] + "]");
                                              
                //printLine("act: " + activation);
                for (int j = 0; j < perceptron.Dimension; j++)
                {
                    perceptron.Weights[j] += activation * (ex.Example[j] - 
                        activation * perceptron.Weights[j]);
                }
                //printLine("oj[" + i + "]: [" + perceptron.Weights[0] + ";" + perceptron.Weights[1] + ";"
                //    + perceptron.Weights[2] + "]");
                printLine("[" + i + "]: Długość wektora: " + perceptron.Weights.Length);
            }

            return perceptron;
        }

        private void printVectorLength(PerceptronLib.Vector v)
        {
            printLine("Długość wektora: " + v.Length);
        }

        /// <summary>
        /// Drukuje linię na konsoli
        /// </summary>
        private void printLine(string str)
        {
            konsola.Text += str + "\n";
        }

        /// <summary>
        /// Obsługa przycisku uczenia
        /// </summary>
        private void learnButton_Click(object sender, RoutedEventArgs e)
        {
            if (examples != null)
            {
                reduction(examples);
            }
        }

        /// <summary>
        /// Delegacja do funkcji pobierającej argument string oraz zwracającej void
        /// </summary>
        private delegate void voidatstring(string str);

        /// <summary>
        /// Delegacja do funkcji printLine
        /// </summary>
        private voidatstring printLineDelegate;

    }
}

