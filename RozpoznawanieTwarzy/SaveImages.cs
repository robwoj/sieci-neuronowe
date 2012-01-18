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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        private void saveImages(List<PerceptronLib.Vector> vectors, int width, int height)
        {
            // Otwiera plik bazy danych i nadpisuje go
            FileStream stream = new System.IO.FileStream(dataBaseFileName, FileMode.Create, System.IO.FileAccess.Write);
            EigenFacesDB db = new EigenFacesDB(vectors);
            BinaryFormatter formatter = new BinaryFormatter();

            printLine("Zapisywanie wyników...");
            int dimension = examples[0].Example.Dimension;
            for (int l = 0; l < examples.Count; l++)
            {
                LearningExample ex = examples[l];

                // Tworzy nowy element bazy danych
                EigenNode node = new EigenNode("Przykład" + (l+1));
                PerceptronLib.Vector v = new PerceptronLib.Vector(dimension);

                //printLine("outputDim = " + outputDimension + ", vectors.count = " + vectors.Count);
                for (int k = 0; k < outputDimension; k++)
                {
                    PerceptronLib.Vector p = vectors[k];
                    Bitmap img = new Bitmap(examplesWidth, examplesHeight);
                    if (l == 0)
                    {
                        Bitmap eigenImg = new Bitmap(examplesWidth, examplesHeight);
                        LearningExample eigenEx = new LearningExample(vectors[k], 0);
                        normalizeRange(eigenEx, 256.0F, width, height);

                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                int index = i * height + j;
                                byte color = (byte)(eigenEx.Example[index]);
                                System.Drawing.Color c = System.Drawing.Color.FromArgb(255, color, color, color);
                                eigenImg.SetPixel(i, j, c);
                            }
                        }

                        eigenImg.Save("eigenVector-" + (k + 1) + ".jpg");
                    }
                    double val = p * p;
                    double activation = p * ex.Example;

                    node.Coordinates.Add(activation);

                    v += p * (activation / val);
                    LearningExample newEx = new LearningExample(v, 0);
                    normalizeRange(newEx, 256.0F, width, height);
                    

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            int index = i * height + j;
                            byte color = (byte)(newEx.Example[index]);
                            System.Drawing.Color c = System.Drawing.Color.FromArgb(255, color, color, color);
                            img.SetPixel(i, j, c);
                        }
                    }

                    img.Save("output" + (l + 1) + "-" + (k + 1) + ".jpg");
                }

                db.add(node);
            }

            //foreach (EigenNode n in nodes)
            //{
            //    printLine("n.coordinates: " + n.Coordinates.Count);
            //    db.add(n);
            //}

            printLine("Wymiar zapisywanej bazy: " + db.Dimension);
            try
            {
                formatter.Serialize(stream, db);
            }
            catch (Exception ex)
            {

                printLine("Wujątek: " + ex.Message + " [ " + ex.StackTrace + " ]");
            }
            stream.Close();
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