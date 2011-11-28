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
namespace RozpoznawanieTwarzy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog openDialog;
        private List<PerceptronLib.Vector> examples;
        public MainWindow()
        {
            InitializeComponent();
            openDialog = new FolderBrowserDialog();
            openDialog.Description = "Podaj katalog z przykładami";
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
            }
        }

        private List<PerceptronLib.Vector> createLearningExamples(List<FileInfo> files)
        {
            List<PerceptronLib.Vector> examples = new List<PerceptronLib.Vector>();

            foreach (FileInfo f in files)
            {
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(f.FullName);
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
                        v[index] = bitmap.GetPixel(i, j).R;
                        if (min > v[index]) min = v[index];
                        if (max < v[index]) max = v[index];
                    }
                }

                //printLine("Max = " + max + ", Min = " + min);
                examples.Add(v);
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

        private void printLine(string str)
        {
            konsola.Text += str + "\n";
        }
    }
}

