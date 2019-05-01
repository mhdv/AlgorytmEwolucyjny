using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.parsertokens;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

/*
PRZYKŁADY FUNKCJI

    FUNKCJA Z 4 MINIMAMI LOKALNYMI
RÓWNANIE: x1^4+x2^4-0.62*x1^2-0.62*x2^2
ROZWIĄZANIE: pkt(-0.56,-0.56), wartość = -0.19

    FUNKCJA ROSENBROCKA
RÓWNANIE: 100*(x2-x1^2)^2+(1-x1)^2
ROZWIĄZANIE: pkt(1,1), wartość = 0.0

    FUNKCJA ZANGWILLA
RÓWNANIE: (x1-x2+x3)^2+(-x1+x2+x3)^2+(x1+x2-x3)^2
ROZWIĄZANIE: pkt(0,0,0), wartość = 0.0

    FUNKCJA DO TESTOWANIA ALGORYTMÓW GENETYCZNYCH ????
RÓWNANIE: exp(-2*log(2)*((x1-0.08)^2)/((0.854)^2))*sin(5*(pi^(3/4))-0.05)
    
    FUNKCJA GEEMA
RÓWNANIE: 4*x1^2-2.1*x1^4+(1/3)*x1^6+x1*x2-4*x2^2+4*x2^4

    FUNKCJA GOLDSTEINA-PRICE'A
RÓWNANIE: (1+(((x1+x2+1)^2)*(19-14*x1+3*x1^2-14*x2+6*x1*x2+3*x2^2)))*((30+((2*x1-3*x2)^2)*(18-32*x1+12*x1^2+48*x2-36*x1*x2+27*x2^2)))

*/

/*

LISTA AKTUALNYCH PROBLEMÓW:
- calculate() nie zwraca wartości ujemnych?


*/

namespace AlgorytmEwolucyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string equationString;
        public org.mariuszgromada.math.mxparser.Expression eq;
        public List<string> argumentsString = new List<string>();
        ObservableCollection<Arguments> arguments = new ObservableCollection<Arguments>();
        Function f;
        Algorithm algorithm = new Algorithm();
        string commaSeparatedArguments;
        Population population = new Population();
        List<Subject> allSolutions = new List<Subject>();
        int p = 400;

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeOtherComponents();
            variablesGrid.CellEditEnding += myDG_CellEditEnding;
        }

        private PlotModel plot(org.mariuszgromada.math.mxparser.Expression equa)
        {
            var model = new PlotModel();

            List<Token> tokensList = eq.getCopyOfInitialTokens();
            foreach (Token t in tokensList)
            {
                if (t.tokenTypeId == Token.NOT_MATCHED)
                {
                    if (!argumentsString.Contains(t.tokenStr))
                    {
                        argumentsString.Add(t.tokenStr);
                    }
                }
            }
            commaSeparatedArguments = string.Join(", ", argumentsString);
            f = new Function("f(" + commaSeparatedArguments + ") = " + equationString);

            if (argumentsString.ToArray().Length > 1)
            {
                model.Title = "Warstwice: ";
                List<double[]> xy = new List<double[]>();
                foreach (var arg in arguments)
                {
                    xy.Add(ArrayBuilder.CreateVector(arg.Minimum, arg.Maximum, p));
                }
                var infunc = new string[p, p];

                for (int i = 0; i < p; ++i)
                {
                    for (int j = 0; j < p; ++j)
                    {
                        for (int k = 0; k < xy.ToArray().Length; ++k)
                        {
                            if (k == 0)
                                infunc[i, j] += xy[k][i].ToString().Replace(',', '.');
                            if (k == 1)
                                infunc[i, j] += "," + xy[k][j].ToString().Replace(',', '.');
                            if (k > 1)
                                infunc[i, j] += ", 0";
                        }
                    }
                }

                var data = new double[p, p];
                for (int i = 0; i < p; ++i)
                {
                    for (int j = 0; j < p; ++j)
                    {
                        equa = new org.mariuszgromada.math.mxparser.Expression("f(" + infunc[i, j] + ")", f);
                        data[i, j] = equa.calculate();
                    }
                }

                var cs = new ContourSeries
                {
                    Color = OxyColors.Black,
                    LabelBackground = OxyColors.White,
                    ColumnCoordinates = xy[0],
                    RowCoordinates = xy[1],
                    Data = data
                };
                model.Series.Add(cs);
            }
            else
            {
                p = p * 10;
                model.Title = "Wykres: ";
                LineSeries series1 = new LineSeries();
                List<double[]> xy = new List<double[]>();
                foreach (var arg in arguments)
                {
                    xy.Add(ArrayBuilder.CreateVector(arg.Minimum, arg.Maximum, p));
                }
                var infunc = new string[p];
                for (int i = 0; i < p; ++i)
                {
                    infunc[i] = xy[0][i].ToString().Replace(',', '.');
                }
                var data = new double[p];
                for (int i = 0; i < p; ++i)
                {
                    equa = new org.mariuszgromada.math.mxparser.Expression("f(" + infunc[i] + ")", f);
                    data[i] = equa.calculate();
                }
                List<DataPoint> pnts = new List<DataPoint>();
                for (int i = 0; i < p; ++i)
                {
                    series1.Points.Add(new DataPoint(xy[0][i], data[i]));
                    //pnts.Add(new DataPoint(xy[0][i], data[i]));
                }
                model.Series.Add(series1);
                p = p / 10;
            }

            argumentsString.Clear();

            //var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
            //scatterSeries.Points.Add(new ScatterPoint(0.56, 0.56, 5, 254));
            //model.Series.Add(scatterSeries);
            return model;
        }

        private void myDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    if (bindingPath == "Minimum")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        Regex regex = new Regex(@"-?\d+(?:\.\d+)?");
                        if(regex.Match(el.Text).Success)
                            arguments[rowIndex].Minimum = double.Parse(el.Text, CultureInfo.InvariantCulture);
                        else
                            MessageBox.Show("Wpisuj tylko liczby rzeczywiste (kropka zamiast przecinka)!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    if (bindingPath == "Maximum")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        Regex regex = new Regex(@"-?\d+(?:\.\d+)?");
                        if (regex.Match(el.Text).Success)
                            arguments[rowIndex].Maximum = double.Parse(el.Text, CultureInfo.InvariantCulture);
                        else
                            MessageBox.Show("Wpisuj tylko liczby rzeczywiste (kropka zamiast przecinka)!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                pltModel.Model = plot(eq);
            }
        }

        private void InitializeOtherComponents()
        {
            // Iitialize comboFunctions
            comboFunctions.Items.Add("Wybierz");
            comboFunctions.Items.Add("x1^4+x2^4-0.62*x1^2-0.62*x2^2");
            comboFunctions.Items.Add("100*(x2-x1^2)^2+(1-x1)^2");
            comboFunctions.Items.Add("(x1-x2+x3)^2+(-x1+x2+x3)^2+(x1+x2-x3)^2");
            comboFunctions.Items.Add("4*x1^2-2.1*x1^4+(1/3)*x1^6+x1*x2-4*x2^2+4*x2^4");
            comboFunctions.Items.Add("sin(5.1*pi*x1+0.5)^6");
            comboFunctions.Items.Add("(1+(((x1+x2+1)^2)*(19-14*x1+3*x1^2-14*x2+6*x1*x2+3*x2^2)))*((30+((2*x1-3*x2)^2)*(18-32*x1+12*x1^2+48*x2-36*x1*x2+27*x2^2)))");
            // Initialize comboReproductionMethod
            comboReproductionMethod.Items.Add("Domyślna");
            comboReproductionMethod.Items.Add("Krzyżowanie uśredniające");
            comboReproductionMethod.Items.Add("Krzyżowanie dwupunktowe");
        }

        //
        // Główna funkcja programu - po naciśnięciu zatwierdź - Taki main
        //
        private void btnEquation_Click(object sender, RoutedEventArgs e)
        {
            if (System.Int32.Parse(txtPopulationSize.Text) <= 5)
            {
                MessageBox.Show("Populacja powinna być większa niż 5!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Definicje
            algorithm = new Algorithm();
            commaSeparatedArguments = "";
            population = new Population();

            // Równanie w postaci łańcucha znaków
            equationString = txtEquation.Text;

            // Tymczasowe równanie - później jest nadpisywane
            eq = new org.mariuszgromada.math.mxparser.Expression(equationString);

            // Odczytywanie argumentów wpisanego równania
            List<Token> tokensList = eq.getCopyOfInitialTokens();
            tmpSolution.Text = "";
            foreach (Token t in tokensList)
            {
                if (t.tokenTypeId == Token.NOT_MATCHED)
                {
                    if (!argumentsString.Contains(t.tokenStr))
                        argumentsString.Add(t.tokenStr);
                }
            }
            //
            // Łapanie błędów
            //
            if (argumentsString.ToArray().Length < 1)
            {
                tmpSolution.Text = "Błędne równanie - brak argumentów";
                argumentsString.Clear();
                argumentsString = new List<string>();
                equationString = "";
                eq = new org.mariuszgromada.math.mxparser.Expression();
                System.GC.Collect(); // <- Garbage Collector
                algorithm.ClearAlgorithm();
                return;
            }
            if (!eq.checkLexSyntax())
            {
                tmpSolution.Text = "Błąd składni - używaj składni środowiska MATLAB/WOLFRAM";
                argumentsString.Clear();
                argumentsString = new List<string>();
                equationString = "";
                eq = new org.mariuszgromada.math.mxparser.Expression();
                System.GC.Collect(); // <- Garbage Collector
                algorithm.ClearAlgorithm();
                return;
            }
            foreach (var arg in argumentsString)
                if (arg.Length < 2 || !arg.Contains("x"))
                {
                    tmpSolution.Text = "Wprowadzaj tylko argumenty postaci x1,x2,x3...";
                    argumentsString.Clear();
                    argumentsString = new List<string>();
                    equationString = "";
                    eq = new org.mariuszgromada.math.mxparser.Expression();
                    System.GC.Collect(); // <- Garbage Collector
                    algorithm.ClearAlgorithm();
                    return;
                }

            // Oddzielanie argumentów przecinkami oraz utworzenie funkcji
            commaSeparatedArguments = string.Join(", ", argumentsString);
            f = new Function("f(" + commaSeparatedArguments + ") = " + equationString);

            // Tworzenie populacji i jej inicjalizacja / inicjalizacja algorytmu
            algorithm.AlgorithmInit(comboReproductionMethod.SelectedIndex, System.Int32.Parse(txtIterations.Text));
            population.initPopulation(arguments, System.Convert.ToInt32(txtPopulationSize.Text));
            pbProgress.Value = 0;

            btnEquation.IsEnabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(10000);
        }

        //
        //  Funkcja zapobiega wpisywaniu liter w wielkości populacji
        //
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ComboFunctions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboFunctions.SelectedIndex != 0)
                txtEquation.Text = comboFunctions.SelectedItem.ToString();
        }

        private void TxtEquation_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            arguments = new ObservableCollection<Arguments>();
            equationString = txtEquation.Text;
            // Tymczasowe równanie - później jest nadpisywane
            eq = new org.mariuszgromada.math.mxparser.Expression(equationString);
            // Odczytywanie argumentów wpisanego równania
            List<Token> tokensList = eq.getCopyOfInitialTokens();
            tmpSolution.Text = "";
            foreach (Token t in tokensList)
            {
                if (t.tokenTypeId == Token.NOT_MATCHED)
                {
                    if (!argumentsString.Contains(t.tokenStr))
                    {
                        argumentsString.Add(t.tokenStr);
                        arguments.Add(new Arguments { Argument = t.tokenStr, Minimum = -5, Maximum = 5});
                    }
                }
            }
            pltModel.Model = plot(eq);
            variablesGrid.AutoGenerateColumns = true;
            variablesGrid.ItemsSource = null;
            variablesGrid.ItemsSource = arguments;
            variablesGrid.Items.Refresh();
            argumentsString.Clear();
            tokensList.Clear();
            
        }
        
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int k = 0; k < algorithm.iterations; k++)
            {
                int progressPercentage = Convert.ToInt32(((double)k / algorithm.iterations) * 100);
                // Sortowanie według najlepszych rozwiązań
                population.subjects = population.subjects.OrderBy(o => o.solution).ToList();

                // Uruchomienie algorytmu
                population = algorithm.RunAlgorithm(population);
                org.mariuszgromada.math.mxparser.Expression equa = new org.mariuszgromada.math.mxparser.Expression();
                // Tworzenie wyrażeń oraz obliczanie rozwiązań aktualnej populacji
                foreach (var sub in population.subjects)
                {
                    equa = new org.mariuszgromada.math.mxparser.Expression("f(" + string.Join(", ", sub.stringValues) + ")", f);
                    sub.solution = equa.calculate();
                }
                (sender as BackgroundWorker).ReportProgress(progressPercentage, population.subjects[0]);
            }
            e.Result = population.subjects[0];
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            txtProgress.Text = e.ProgressPercentage.ToString() + "%";
            if (e.UserState != null)
            {
                var sol = (Subject)e.UserState;
                tmpSolution.Text = "#######################################\n";
                tmpSolution.Text += "ROZWIĄZANIE NIE JEST OSTATECZNE. ZACZEKAJ NA KONIEC PROCESU.\n";
                tmpSolution.Text += "Aktualne rozwiązanie:   " + sol.solution + "\n";
                tmpSolution.Text += "#######################################\n";
                allSolutions.Add(sol);
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbProgress.Value = 100;
            txtProgress.Text = "100%";
            var sol = (Subject)e.Result;
            allSolutions = allSolutions.OrderBy(o => o.solution).ToList();
            // tymczasowe rozwiązanie równania
            tmpSolution.Text = "Znalezione rozwiązania przy populacji wielkości " + txtPopulationSize.Text + ":\n";
            
                tmpSolution.Text += "#######################################\n";
                tmpSolution.Text += "(" + commaSeparatedArguments + ") = " + "(" + string.Join(", ", allSolutions[0].stringValues) + ")" + "\n";
                tmpSolution.Text += "Rozwiązanie:   " + allSolutions[0].solution + "\n";
                tmpSolution.Text += "#######################################\n";
            


            // Czyszczenie przed kolejnym wywołaniem
            argumentsString.Clear();
            argumentsString = new List<string>();
            equationString = "";
            eq = new org.mariuszgromada.math.mxparser.Expression();
            System.GC.Collect(); // <- Garbage Collector
            algorithm.ClearAlgorithm();
            population.subjects.Clear();
            allSolutions.Clear();

            btnEquation.IsEnabled = true;
            // MessageBox
            MessageBox.Show("Obliczono rozwiązania", "Ukończono", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TxtPlot_TextChanged(object sender, TextChangedEventArgs e)
        {
            p = System.Int32.Parse(txtPlot.Text);
        }
    }
}
