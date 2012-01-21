using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PerceptronLib;

namespace AutoasocjatorHopfielda
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int width;
        private int height;

        private int rectWidth;
        private Matrix input;
        private Matrix output;
        private List<LearningExample> examples;

        private bool windowLoaded;
        private int iterationBias;
        private int iteratoionBiasConstant = 50;
        public MainWindow()
        {
            windowLoaded = false;
            InitializeComponent();
#if ! DEBUG
            konsola.Visibility = System.Windows.Visibility.Hidden;
            this.Width = mainCanvas.Width;
            this.height = 470;
#endif

            width = 5;
            height = 5;

            rectWidth = 30;
            printLineDelegate += pl;

            examples = new List<LearningExample>();
            names = new List<string>();
            iterationBias = 50;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            input = new Matrix(width, height);
            output = new Matrix(width, height);
            drawBitmap(220, 50, false, input, "Obraz wyjściowy");
            drawBitmap(50, 50, true, output, "Obraz do zmiany");
            drawBitmap(390, 50, false, output, "Obraz odszumiony");
            runButton.IsEnabled = false;
            podstText.Text = "0,95";
            constText.Text = "0,1";
            iterText.Text = "1000";
            constLabel.Visibility = System.Windows.Visibility.Hidden;
            podstLabel.Visibility = System.Windows.Visibility.Hidden;
            constText.Visibility = System.Windows.Visibility.Hidden;
            podstText.Visibility = System.Windows.Visibility.Hidden;
            if (finnishCheckBox.IsChecked == true)
            {
                iterationBias = iteratoionBiasConstant;
            }
            else
            {
                iterationBias = int.Parse(iterText.Text);
            }
            windowLoaded = true;
        }

        private void rectangle_Click(object sender, MouseButtonEventArgs e)
        {

            if (sender is Rectangle == false)
            {
                throw new Exception("Obiekt nie jest prostokątem");
            }

            Rectangle r = (Rectangle)sender;
            RectangleTag tag = (RectangleTag)r.Tag;

            if (tag.CanChange)
            {
                tag.State = !tag.State;
                if (tag.State == true)
                {
                    r.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                }
                else
                {
                    r.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                }

            }
            printLine(tag.ToString());
        }

        private void drawRect(double left, double top, RectangleTag tag)
        {
            Rectangle r = new Rectangle();

            r.Margin = new Thickness(left, top, 0, 0);
            r.Height = rectWidth;
            r.Width = rectWidth;

            r.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGray);
            r.Tag = tag;
            r.ToolTip = r.Tag.ToString();

            if (tag.State == true)
            {
                r.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            }
            else
            {
                r.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            }

            r.MouseDown += rectangle_Click;

            mainCanvas.Children.Add(r);
        }

        private void drawBitmap(double left, double top, bool canChange, Matrix mat, string name)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    drawRect(i * rectWidth + left, j * rectWidth + top,
                        new RectangleTag(i, j, canChange, mat, name));
                }
            }
        }

        private void mainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private delegate void voidatstring(string str);
        private voidatstring printLineDelegate;
        private void pl(string str)
        {
            konsola.Text += str + "\n";
        }

        private void printLine(string str)
        {
#if DEBUG
            Dispatcher.Invoke(printLineDelegate, str);
#endif
        }

        private List<string> names;
        private void addExamplesButtonClick(object sender, RoutedEventArgs e)
        {
            examples.Add(new LearningExample((PerceptronLib.Vector)output, 0));
            names.Add("Obraz " + examples.Count);
            sourceList.Items.Add(names.Last());
            if (sourceList.SelectedIndex == -1)
            {
                sourceList.SelectedIndex = 0;
                runButton.IsEnabled = true;
            }
            //printLine(output.ToString());
        }

        private void runButtonClick(object sender, RoutedEventArgs e)
        {
            int n = examples.Count;
            int m = height * width;
            printLine("Uruchamianie odszumiania");

            printLine("Tworzenie wag");
            Matrix w = new Matrix(m, m);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    w[i, j] = 0;
                    for (int k = 0; k < n; k++)
                    {
                        PerceptronLib.Vector ex = examples[k].Example;
                        w[i, j] += ex[i] * ex[j];
                    }
                    w[i, j] /= (double)m;
                }
            }

            printLine("Po utworzeniu wag");

            printLine("Uruchamianie dynamiki Hopfielda");
            PerceptronLib.Vector s = (PerceptronLib.Vector)output;

            if (simpleRadio.IsChecked == true)
            {
                simpleDynamic(s, w);
            }
            else
            {
                simulatedDynamic(s, w);
            }

            printLine("Zakończono odszumianie");

            printLine(((Matrix)s).ToString());
            drawBitmap(390, 50, false, (Matrix)s, "Obraz odszumiony");
        }

        private void simulatedDynamic(PerceptronLib.Vector s, Matrix w)
        {
            int m = height * width;
            Random r = new Random();
            double t = 1;

            double a = 0;
            if (double.TryParse(podstText.Text, out a) == false)
            {
                throw new ArgumentException("Podstawa musi być liczbą typu double");
            }
            double c = 0;
            if (double.TryParse(constText.Text, out c) == false)
            {
                throw new ArgumentException("Stała wyżarzania musi być liczbą typu double");
            }
            int iter = 0;
            if (int.TryParse(iterText.Text, out iter) == false)
            {
                throw new ArgumentException("Stała wyżarzania musi być liczbą typu int");
            }

            int i = 0;
            int withoutChange = 0;
            for (i = 0; i < iter; i++)
            {
                double E = 0;
                double E2 = 0;
                int ind = r.Next(s.Dimension);
                for (int j = 0; j < m; j++)
                {
                    for (int k = 0; k < m; k++)
                    {
                        if (k != j)
                            E += w[j, k] * s[j] * s[k];
                    }
                }
                E /= -2;
                s[ind] = -s[ind];
                for (int j = 0; j < m; j++)
                {
                    for (int k = 0; k < m; k++)
                    {
                        if (k != j)
                            E2 += w[j, k] * s[j] * s[k];
                    }
                }
                E2 /= -2;
                s[ind] = -s[ind];

                double dE = E2 - E;

                if (dE < 0)
                {
                    s[ind] = -s[ind];
                    withoutChange++;
                    if (withoutChange > iterationBias) break;
                }
                else
                {
                    withoutChange = 0;
                    double p = Math.Exp(-dE / t);
                    if (p < 0 || p > 1)
                        throw new Exception("Zle p-stwo");
                    if (p > r.NextDouble())
                    {
                        s[ind] = -s[ind];
                    }
                    if (i % 100 == 0)
                    {
                        t = c * Math.Pow(a, i / 100);
                        printLine(t.ToString() + ", de = " + dE + ", p = " + p.ToString());
                    }
                }
            }
            printLine("Koniec wyżarzania, liczba iteracji: " + i);
        }
        private void simpleDynamic(PerceptronLib.Vector s, Matrix w)
        {
            int m = height * width;
            Random r = new Random();
            int iter = 0;
            if (int.TryParse(iterText.Text, out iter) == false)
            {
                throw new ArgumentException("Stała wyżarzania musi być liczbą typu int");
            }

            int i = 0;
            int withoutChange = 0;
            for (i = 0; i < iter; i++)
            {

                int ind = r.Next(s.Dimension);

                double sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += w[ind, j] * s[j];
                }

                int sign = Math.Sign(sum);
                if(sign != s[ind])
                {
                    withoutChange = 0;
                }
                else
                {
                    withoutChange++;
                    if(withoutChange > iterationBias) break;
                }
                s[ind] = sign;

            }

            printLine("Koniec prostej dynamiki, liczba iteracji: " + i);
        }

        private void sourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int index = names.IndexOf((string)e.AddedItems[0]);
            int index = sourceList.SelectedIndex;
            if (index != -1)
            {
                printLine(index.ToString());
                printLine(((Matrix)examples[index].Example).ToString());
                drawBitmap(220, 50, false, (Matrix)examples[index].Example, "Obraz wyjściowy");
                removeButton.IsEnabled = true;
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            output = new Matrix(width, height);
            drawBitmap(50, 50, true, output, "Obraz do zmiany");
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            int i = sourceList.SelectedIndex;
            if (i != -1)
            {
                names.RemoveAt(i);
                examples.RemoveAt(i);
                sourceList.Items.RemoveAt(i);
                drawBitmap(220, 50, false, new Matrix(width, height), "Obraz wyjściowy");

                if (sourceList.Items.Count == 0)
                {
                    removeButton.IsEnabled = false;
                }
            }
        }

        private void simulatedRadio_Checked(object sender, RoutedEventArgs e)
        {
            constLabel.Visibility = System.Windows.Visibility.Visible;
            podstLabel.Visibility = System.Windows.Visibility.Visible;
            constText.Visibility = System.Windows.Visibility.Visible;
            podstText.Visibility = System.Windows.Visibility.Visible;
        }

        private void simpleRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (windowLoaded)
            {
                constLabel.Visibility = System.Windows.Visibility.Hidden;
                podstLabel.Visibility = System.Windows.Visibility.Hidden;
                constText.Visibility = System.Windows.Visibility.Hidden;
                podstText.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void finnishCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (windowLoaded)
            {
                iterationBias = iteratoionBiasConstant;
            }
        }

        private void finnishCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (windowLoaded)
            {
                int iterations = 0;
                if(int.TryParse(iterText.Text, out iterations) == false)
                {
                    throw new ArgumentException("Liczba iteracji powinna być liczbą typu int");
                }
                iterationBias = iterations;               
            }
        }
    }
    class RectangleTag
    {
        private Matrix matrix;
        public int i;
        public int j;
        public string name;
        private bool canChange;
        public bool State
        {

            get
            {
                return matrix[i, j] == -1 ? false : true;
            }
            set
            {
                if (value == true)
                    matrix[i, j] = 1;
                else
                    matrix[i, j] = -1;
            }
        }

        public bool CanChange
        {
            get
            {
                return canChange;
            }
        }

        public RectangleTag(int j, int i, bool canChange, Matrix mat, string name = "")
        {
            this.i = i;
            this.j = j;
            this.name = name;
            this.canChange = canChange;
            matrix = mat;
        }

        public override string ToString()
        {
            return name + " (" + i + "," + j + "}";
        }
    }

}
