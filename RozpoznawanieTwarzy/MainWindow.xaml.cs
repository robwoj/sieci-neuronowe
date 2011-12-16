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
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("RozpoznawanieTwarzyTests")]
namespace RozpoznawanieTwarzy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog openDialog;
        private List<LearningExample> examples;
        internal int ojIterations;
        internal int outputDimension;
        private int examplesWidth;
        private int examplesHeight;

        Thread creatingExamplesThread;
        Thread savingImagesThread;
        Thread reducingThread;
        List<FileInfo> files;

        /// <summary>
        /// Konstruktor przeznaczony dla testu jednostkowego
        /// </summary>
        internal MainWindow(int ojIter, int outputDim) : this()
        {
            ojIterations = ojIter;
            outputDimension = outputDim;
        }

        public MainWindow()
        {
            InitializeComponent();
            openDialog = new FolderBrowserDialog();
            openDialog.Description = "Podaj katalog z przykładami";

            learnButton.IsEnabled = false;
            ojIterations = 1000;
            printLineDelegate = printByDispatcher;
            saveImagesDelegate = saveImages;
            createLearningExamplesDelegate = createLearningExamples;
            getFilesDelegate = getFiles;
            setExamplesDelegate = setExamples;
            OnStateChange = stateChange;
            OnReductionFinished = reductionFinished;
            OnReductionStarted = reductionStarted;
            files = null;
            learnButton.IsEnabled = false;
            outputDimension = 1;
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            openDialog.ShowDialog();
            if (openDialog.SelectedPath != null)
            {
                printByDispatcher("Wybrano katalog " + openDialog.SelectedPath);
                DirectoryInfo di = null;
                try
                {
                    di = new DirectoryInfo(openDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    printByDispatcher("Nie można otworzyć katalogu " + openDialog.SelectedPath
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
                learnButton.IsEnabled = false;

                creatingExamplesThread = new Thread(startCreatingExamples);
                creatingExamplesThread.Start();
                
                //creatingExamplesThread.Join();
                //examples = createLearningExamples(files);

            }
        }

        private delegate void stateEvent(object sender, EventArgs e);
        private event stateEvent OnReductionFinished;
        private event stateEvent OnReductionStarted;

        private void reductionStarted(object sender, EventArgs e)
        {
            learnButton.IsEnabled = false;
        }

        private void reductionFinished(object sender, EventArgs e)
        {
            learnButton.IsEnabled = true;
        }
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



        private delegate List<LearningExample> examplesCreatingFunc(List<FileInfo> files);

        private examplesCreatingFunc createLearningExamplesDelegate;


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


        private void printVectorLength(PerceptronLib.Vector v)
        {
            //printLine("Długość wektora: " + v.Length);
            printLine("Długość wektora: " + v.Length);
        }

        /// <summary>
        /// Drukuje linię na konsoli
        /// </summary>
        private void printByDispatcher(string str)
        {
            konsola.Text += str + "\n";
        }

        private void printLine(string str)
        {
            Dispatcher.Invoke(printLineDelegate, str);
        }

        /// <summary>
        /// Obsługa przycisku uczenia
        /// </summary>
        private void learnButton_Click(object sender, RoutedEventArgs e)
        {
            if (examples != null)
            {
                reducingThread = new Thread(startReducing);
                reducingThread.Start(examples);
                //reduction(examples);
            }
        }

        private void startReducing(object exaplesObj)
        {
            if (exaplesObj is List<LearningExample>)
            {
                reduction((List<LearningExample>)exaplesObj);
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

        /// <summary>
        /// Zapewnia zatrzymanie wątków roboczych
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                reducingThread.Abort();
            }
            catch (Exception)
            { }
            try
            {
                savingImagesThread.Abort();
            }
            catch (Exception)
            {
            }

            try
            {
                creatingExamplesThread.Abort();
            }
            catch (Exception)
            {
                
            }
        }

    }
}

