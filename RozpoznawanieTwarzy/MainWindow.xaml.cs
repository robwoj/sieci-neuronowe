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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[assembly:InternalsVisibleTo("RozpoznawanieTwarzyTests")]
namespace RozpoznawanieTwarzy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog openFolderDialog;
        private OpenFileDialog openFileDialog;
        private List<LearningExample> examples;
        internal int ojIterations;
        internal int outputDimension;
        private int examplesWidth;
        private int examplesHeight;
        
        Thread creatingExamplesThread;
        Thread savingImagesThread;
        Thread reducingThread;
        List<FileInfo> files;

        private string dataBaseFileName;

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
            openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.Description = "Podaj katalog z przykładami";
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*JPEG|*.jpg|Wszystkie pliki|*.*";
            openFileDialog.Title = "Podaj plik przeznaczony do rozponania";

            ojIterations = 10;
            iterationsText.Text = ojIterations.ToString();
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
            compareButton.IsEnabled = false;
            outputDimension = 7;
            dimensionText.Text = outputDimension.ToString();
            
            dataBaseFileName = "C:\\sieci-neuronowe\\database.db";
            dataBasePathText.Text = dataBaseFileName;

            examplesHeight = 0;
            examplesWidth = 0;
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            openFolderDialog.ShowDialog();
            if (openFolderDialog.SelectedPath != null)
            {
                printByDispatcher("Wybrano katalog " + openFolderDialog.SelectedPath);
                DirectoryInfo di = null;
                try
                {
                    di = new DirectoryInfo(openFolderDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    printByDispatcher("Nie można otworzyć katalogu " + openFolderDialog.SelectedPath
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
            compareButton.IsEnabled = false;
            openButton.IsEnabled = false;
        }

        private void reductionFinished(object sender, EventArgs e)
        {
            learnButton.IsEnabled = true;
            openButton.IsEnabled = true;
            compareButton.IsEnabled = true;
        }
        private void stateChange(object sender, EventArgs e)
        {
            if (examples != null)
            {
                openButton.IsEnabled = true;
                learnButton.IsEnabled = true;
                if(File.Exists(dataBaseFileName))
                {
                    compareButton.IsEnabled = true;
                }
            }
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

        private delegate void imageSaveFunc(List<PerceptronLib.Vector> vectors, int width, int height);

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
            bool success = true;
            success = int.TryParse(iterationsText.Text, out ojIterations);
            if(success == false || ojIterations <= 0)
            {
                printLine("Niepoprawna liczba iteracji");
                return;
            }

            success = int.TryParse(dimensionText.Text, out outputDimension);
            if (success == false || outputDimension <= 0)
            {
                printLine("Niepoprawny wymiar");
                return;
            }

            if (examples != null)
            {
                printLine("Rozpoczynanie analizy składowych głównych, "
                    + outputDimension + " wymiarów, " + ojIterations + " iteracji algorytmu Oja");
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

        private void compareButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(dataBasePathText.Text) == false)
            {
                printLine("Nie istnieje plik bazy danych");
                return;
            }

            try
            {
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName != null)
                {
                    string fn = openFileDialog.FileName;
                    printLine("Porównywanie pliku: " + fn);
                    Bitmap bitmap = (Bitmap)Bitmap.FromFile(fn);
                    if (bitmap.Width == examplesWidth && bitmap.Height == examplesHeight)
                    {
                        printLine("Wymiary OK");

                        PerceptronLib.Vector v = new PerceptronLib.Vector(bitmap.Width * bitmap.Height);

                        int width = bitmap.Width;
                        int height = bitmap.Height;
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                int index = i * height + j;
                                //v[index] = (double)bitmap.GetPixel(i, j).R / (256.0F * width * height) * 3000;
                                v[index] = (double)bitmap.GetPixel(i, j).R;
                            }
                        }

                        BinaryFormatter formatter = new BinaryFormatter();

                        FileStream fs = new System.IO.FileStream(dataBasePathText.Text, FileMode.Open, System.IO.FileAccess.Read);
                        EigenFacesDB db = (EigenFacesDB)formatter.Deserialize(fs);

                        printLine("Rozpoznany obraz: " + db.compareFace(v));

                    }
                    else
                    {
                        printLine("Niezgodne wymiary wejścia");
                    }
                }
            }
            catch (Exception ex)
            {
                printLine(ex.Message + " [ " + ex.StackTrace + " ]");
            }
        }

    }
}

