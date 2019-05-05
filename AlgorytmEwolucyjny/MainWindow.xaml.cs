using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
- BRAK WYKRYTYCH PROBLEMÓW

*/

namespace AlgorytmEwolucyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Definicje zmiennych globalnych
        public string equationString;   // równanie w postaci łańcucha znaków
        public org.mariuszgromada.math.mxparser.Expression eq;  // równanie jako zmienna globalna
        public List<string> argumentsString = new List<string>();   // lista argumentów w postaci łańcucha znaków
        ObservableCollection<Arguments> arguments = new ObservableCollection<Arguments>();  // lista argumentów w postaci obiektów (kolekcja do wypisania)
        Function f; // pomocnicza zmienna funkcji wpisanej do równania
        Algorithm algorithm = new Algorithm();  // obiekt algorytmu
        string commaSeparatedArguments; // argumenty rozdzielone przecinkami w postaci łańcucha znaków
        Population population = new Population();   // obiekt populacji
        List<Subject> allSolutions = new List<Subject>();   // lista wszystkich znalezionych rozwiązań
        int p = 400;    // zmienna pomocnicza określająca dokładność wykresu
        PlotModel model = new PlotModel();  // model do plotowania
        bool plotBusy = false;  // czy model jest aktualnie plotowany?
        int minmax = 0;
        
        //
        // Inicjalizacja głównego okna programu
        //
        public MainWindow()
        {
            InitializeComponent();
            InitializeOtherComponents();
            variablesGrid.CellEditEnding += myDG_CellEditEnding;
        }

        //
        // Funkcja wywołująca plotowanie modelu w osobnym wątku
        //
        private void plotChart()
        {
            // Czy wątek rysowania jest aktualnie zajęty?
            if (!plotBusy)
            {
                // Na czas rysowania wyłączamy wszystkie przyciski
                btnEquation.IsEnabled = false;
                plotRefresh.IsEnabled = false;
                plotOnlyBest.IsEnabled = false;
                txtEquation.IsEnabled = false;
                comboFunctions.IsEnabled = false;
                variablesGrid.IsEnabled = false;

                model = new PlotModel();
                BackgroundWorker worker2 = new BackgroundWorker();
                worker2.WorkerReportsProgress = true;
                worker2.DoWork += worker_DoWork2;
                worker2.ProgressChanged += worker_ProgressChanged2;
                worker2.RunWorkerCompleted += worker_RunWorkerCompleted2;
                worker2.RunWorkerAsync();
            }
            else
                return;
        }

        //
        // Funkcja wywoływana po wykonaniu wątku rysowania
        //
        void worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            plotPb.Value = 100;
            pltModel.Model = (PlotModel)e.Result;
            // Włączamy ponownie przyciski
            btnEquation.IsEnabled = true;
            plotRefresh.IsEnabled = true;
            plotOnlyBest.IsEnabled = true;
            txtEquation.IsEnabled = true;
            comboFunctions.IsEnabled = true;
            variablesGrid.IsEnabled = true;
            // Ustawianie maxymalnego zoomout wykresów/warstwic
            if(pltModel.Model != null)
            {
                if (arguments.ToArray().Length > 1)
                {
                    pltModel.Model.DefaultXAxis.AbsoluteMaximum = arguments[0].Maximum;
                    pltModel.Model.DefaultXAxis.AbsoluteMinimum = arguments[0].Minimum;
                    pltModel.Model.DefaultYAxis.AbsoluteMinimum = arguments[1].Minimum;
                    pltModel.Model.DefaultYAxis.AbsoluteMaximum = arguments[1].Maximum;
                    pltModel.InvalidatePlot(true);
                }
                else
                {
                    pltModel.Model.DefaultXAxis.AbsoluteMaximum = arguments[0].Maximum;
                    pltModel.Model.DefaultXAxis.AbsoluteMinimum = arguments[0].Minimum;
                    pltModel.Model.DefaultYAxis.AbsoluteMinimum = arguments[0].Minimum;
                    pltModel.Model.DefaultYAxis.AbsoluteMaximum = arguments[0].Maximum;
                    pltModel.InvalidatePlot(true);
                }
                pltModel.Model.DefaultXAxis.MajorGridlineStyle = LineStyle.Dot;
                pltModel.Model.DefaultYAxis.MajorGridlineStyle = LineStyle.Dot;
            }

            plotBusy = false;
        }

        //
        // Zmiana postępu rysowania
        //
        void worker_ProgressChanged2(object sender, ProgressChangedEventArgs e)
        {
            // Ustawiamy informacje o pasku postępu
            plotPb.Value = e.ProgressPercentage;
        }

        //
        // Rysowanie punktu na wykresie
        //
        void plotPoint(Subject point)
        {
            // Rysowanie punktu można wykonać tylko na narysowanym wykresie (wątek rysowania nie może być zajęty)
            if (!plotBusy)
            {
                // Jeśli liczba argumentów jest > 1 to rysujemy dla x1 i x2 (przypadek warstwic)
                if(arguments.ToArray().Length > 1)
                {
                    var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
                    scatterSeries.Points.Add(new ScatterPoint(point.values[0], point.values[1], 3, 254));
                    model.Series.Add(scatterSeries);
                    pltModel.InvalidatePlot(true);
                }
                // Jeśli liczba argumentów = 1 (inny przypadek nie jest już możliwy) to rysujemy dla x1 i rozwiązania (przypadek wykresu)
                else
                {
                    var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
                    scatterSeries.Points.Add(new ScatterPoint(point.values[0], point.solution, 3, 254));
                    model.Series.Add(scatterSeries);
                    pltModel.InvalidatePlot(true);
                }
            }

        }

        //
        // Funkcja wywoływana przez wątek do rysowania wykresów - tutaj plotujemy model
        //
        void worker_DoWork2(object sender, DoWorkEventArgs e)
        {
            plotBusy = true; // wątek rysowania jest zajęty

            // Tymczasowe równanie - jest nadpisywane w różnych miejscach kodu
            eq = new org.mariuszgromada.math.mxparser.Expression(equationString);

            // Lista tokenów (argumentów) dodawana do listy łańcuchów znaków argumentów
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

            // Rozdzielenie argumentów przecinkami
            commaSeparatedArguments = string.Join(", ", argumentsString);

            // Utworzenie funkcji na podstawie argumentów i równania
            f = new Function("f(" + commaSeparatedArguments + ") = " + equationString);

            // Wyłapywanie błędów równania
            if (argumentsString.ToArray().Length < 1)
            {
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
                argumentsString.Clear();
                argumentsString = new List<string>();
                equationString = "";
                eq = new org.mariuszgromada.math.mxparser.Expression();
                System.GC.Collect(); // <- Garbage Collector
                algorithm.ClearAlgorithm();
                return;
            }
            if (eq.checkSyntax())
            {
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
                    argumentsString.Clear();
                    argumentsString = new List<string>();
                    equationString = "";
                    eq = new org.mariuszgromada.math.mxparser.Expression();
                    System.GC.Collect(); // <- Garbage Collector
                    algorithm.ClearAlgorithm();
                    return;
                }
            
            // Jeśli liczba argumentów > 1 to rysujemy warstwice
            if (arguments.ToArray().Length > 1)
            {
                model.Title = "Warstwice: "; // tytuł modelu to warstwice
                List<double[]> xy = new List<double[]>(); // lista tablic zmiennych double przechowująca minimum i maximum konkretnych argumentów

                // Zapisywanie tych zmiennych jest potrzebne do określenia granic wykresów
                foreach (var arg in arguments)
                {
                    xy.Add(ArrayBuilder.CreateVector(arg.Minimum, arg.Maximum, p));
                }

                // Pomocnicza tablica łańcuchów znaków argumentów funkcji (trochę czarów się tutaj dzieje)
                // Finalnie infunc powinien zawierać łańcuchy znaków postaci:
                //      Przykład dla 2 argumentów: 0.125, 1.216
                //      Przykład dla 3 argumentów: 0.125, 1.216, 0
                //      Przykład dla 4 argumentów: 0.125, 1.216, 0, 0
                // Zera na końcu są wstawiane dlatego, że spłaszczamy wielowymiarowy wykres do postaci warstwic
                var infunc = new string[p, p];
                for (int i = 0; i < p; ++i)
                {
                    for (int j = 0; j < p; ++j)
                    {
                        for (int k = 0; k < xy.ToArray().Length; ++k)
                        {
                            // Jeśli jest to pierwszy argument to dodajemy go po prostu do łańcucha znaków
                            if (k == 0)
                                infunc[i, j] += xy[k][i].ToString().Replace(',', '.');
                            // Jeżeli jest to drugi argument to dodajemy go do łańcucha znaków z przecinkiem przed wartością
                            if (k == 1)
                                infunc[i, j] += "," + xy[k][j].ToString().Replace(',', '.');
                            // Jeżeli jest to każdy kolejny argument to dodajemy 0 do łańcucha znaków z przecinkiem przed
                            if (k > 1)
                                infunc[i, j] += ", 0";
                        }
                    }
                }

                // Tablica zawierająca wyrażenia do obliczenia - korzysta z poprzedniej tablicy pomocniczej do określenia tych wyrażeń
                var data = new double[p, p];
                for (int i = 0; i < p; ++i)
                {
                    for (int j = 0; j < p; ++j)
                    {
                        int progressPercentage = Convert.ToInt32(((double)i / p) * 100);
                        org.mariuszgromada.math.mxparser.Expression equa = new org.mariuszgromada.math.mxparser.Expression("f(" + infunc[i, j] + ")", f);
                        data[i, j] = equa.calculate();
                        (sender as BackgroundWorker).ReportProgress(progressPercentage);
                    }
                }

                // Tworzenie zmiennej kontur - wykorzystywana do rysowania warstwic
                var cs = new ContourSeries
                {
                    Color = OxyColors.Red,
                    LabelBackground = OxyColors.White,
                    ColumnCoordinates = xy[0],
                    RowCoordinates = xy[1],
                    Data = data
                };
                // Dodanie kontur do modelu
                model.Series.Add(cs);
            }
            // Jeśli liczba argumentów = 1 to rysujemy wykres
            else
            {
                p = p * 30; // zmieniamy dokładność na 30-krotnie większą od ustalonej ze względu na mniejszą złożoność obliczeniową
                model.Title = "Wykres: ";   // tytuł modelu to wykres
                LineSeries series1 = new LineSeries();  // zmienna lineseries używana do rysowania wykresów
                List<double[]> xy = new List<double[]>();   // lista tablic tak jak w poprzednim przypadku - tutaj przechowuje tylko jeden argument, ale zachowanie konwencji ułatwiło dalsze zadania
                foreach (var arg in arguments)
                {
                    xy.Add(ArrayBuilder.CreateVector(arg.Minimum, arg.Maximum, p));
                }

                // Tak samo jak powyżej
                var infunc = new string[p];
                for (int i = 0; i < p; ++i)
                {
                    infunc[i] = xy[0][i].ToString().Replace(',', '.');
                }

                // Tak samo jak powyżej
                var data = new double[p];
                for (int i = 0; i < p; ++i)
                {
                    int progressPercentage = Convert.ToInt32(((double)i / p) * 100);
                    org.mariuszgromada.math.mxparser.Expression equa = new org.mariuszgromada.math.mxparser.Expression("f(" + infunc[i] + ")", f);
                    data[i] = equa.calculate();
                    (sender as BackgroundWorker).ReportProgress(progressPercentage);
                }

                // Lista wygenerowanych punktów dodawana do zmiennej typu lineseries w celu wygenerowania wykresu
                List<DataPoint> pnts = new List<DataPoint>();
                for (int i = 0; i < p; ++i)
                {
                    series1.Points.Add(new DataPoint(xy[0][i], data[i]));
                }
                // Kolor wykresu ustalamy na czerwony - tak żeby wykresy i warstwice były tego samego koloru
                series1.Color = OxyColors.Red;
                model.Series.Add(series1);
                p = p / 30; // przywracamy domyślną wartość zmiennej odpowiadającej za dokładność wykresu
            }

            // Czyścimy listę argumentów w postaci łańcuchów znaków
            argumentsString.Clear();
            // Czyścimy wyrażenie
            eq = new org.mariuszgromada.math.mxparser.Expression();
            // Zapisujemy utworzony model w wyniku wątku rysowania
            e.Result = model;
        }

        //
        // Event zamykania okna (wyjście z programu)
        //
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        //
        // Edycja listy argumentów wyświetlanej w oknie programu
        //
        private void myDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    // Edycja minimów
                    if (bindingPath == "Minimum")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        // Wyrażenie regularne które sprawdza czy wpisywana wartość nie zawiera niedozwolonych znaków
                        Regex regex = new Regex(@"-?\d+(?:\.\d+)?");
                        if(regex.Match(el.Text).Success)
                            arguments[rowIndex].Minimum = double.Parse(el.Text, CultureInfo.InvariantCulture);
                        else
                            MessageBox.Show("Wpisuj tylko liczby rzeczywiste (kropka zamiast przecinka)!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    // Edycja maximów
                    if (bindingPath == "Maximum")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        // Wyrażenie regularne które sprawdza czy wpisywana wartość nie zawiera niedozwolonych znaków
                        Regex regex = new Regex(@"-?\d+(?:\.\d+)?");
                        if (regex.Match(el.Text).Success)
                            arguments[rowIndex].Maximum = double.Parse(el.Text, CultureInfo.InvariantCulture);
                        else
                            MessageBox.Show("Wpisuj tylko liczby rzeczywiste (kropka zamiast przecinka)!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        //
        // Inicjalizacja innych komponentów - comboFunctions oraz comboReproductionMethod
        //
        private void InitializeOtherComponents()
        {
            // Inicjalizacja comboFunctions
            comboFunctions.Items.Add("Wybierz");
            comboFunctions.Items.Add("x1^4+x2^4-0.62*x1^2-0.62*x2^2");
            comboFunctions.Items.Add("100*(x2-x1^2)^2+(1-x1)^2");
            comboFunctions.Items.Add("(x1-x2+x3)^2+(-x1+x2+x3)^2+(x1+x2-x3)^2");
            comboFunctions.Items.Add("4*x1^2-2.1*x1^4+(1/3)*x1^6+x1*x2-4*x2^2+4*x2^4");
            comboFunctions.Items.Add("sin(5.1*pi*x1+0.5)^6");
            comboFunctions.Items.Add("(x1+3)^2+(x2-4)^2");
            comboFunctions.Items.Add("(x1^2+x2-11)^2+(x1+x2^2-7)^2-200");
            comboFunctions.Items.Add("(1+(((x1+x2+1)^2)*(19-14*x1+3*x1^2-14*x2+6*x1*x2+3*x2^2)))*((30+((2*x1-3*x2)^2)*(18-32*x1+12*x1^2+48*x2-36*x1*x2+27*x2^2)))");
            // Inicjalizacja comboReproductionMethod
            comboReproductionMethod.Items.Add("Domyślna");
            comboReproductionMethod.Items.Add("Krzyżowanie uśredniające");
            comboReproductionMethod.Items.Add("Tylko mutacje");
            // Inicjalizacja comboFind
            comboFind.Items.Add("Minimum");
            comboFind.Items.Add("Maximum");
        }

        //
        // Główna funkcja programu - po naciśnięciu zatwierdź - Taki main
        //
        private void btnEquation_Click(object sender, RoutedEventArgs e)
        {
            // Czyścimy listę wszystkich rozwiązań
            allSolutions.Clear();

            // Sprawdzamy czy populacja jest większa niż 5 - dla mniejszych program nie działa poprawnie
            if (System.Int32.Parse(txtPopulationSize.Text) <= 5)
            {
                MessageBox.Show("Populacja powinna być większa niż 5!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Inicjalizacja zmiennych
            algorithm = new Algorithm();
            commaSeparatedArguments = "";
            population = new Population();

            // Równanie w postaci łańcucha znaków
            equationString = txtEquation.Text;
            // Tymczasowe równanie - jest nadpisywane w różnych miejscach kodu
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

            // Łapanie błędów
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

            // Progressbar algorytmu przyjmuje początkową wartość 0
            pbProgress.Value = 0;

            // Na czas wykonywania algorytmu wyłączamy wszystkie przyciski
            btnEquation.IsEnabled = false;
            plotRefresh.IsEnabled = false;
            plotOnlyBest.IsEnabled = false;

            minmax = comboFind.SelectedIndex;

            // Tworzymy nowy wątek do wykonania obliczeń algorytmu
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

        //
        // Zmiana zaznaczenia przykładowej funkcji
        //
        private void ComboFunctions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboFunctions.SelectedIndex != 0)
                txtEquation.Text = comboFunctions.SelectedItem.ToString();
            equationString = comboFunctions.SelectedItem.ToString();
        }

        //
        // Zmiana wpisanego równania
        //
        private void TxtEquation_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Inicjowanie zmiennych
            arguments = new ObservableCollection<Arguments>();
            equationString = txtEquation.Text;

            // Tymczasowe równanie - jest nadpisywane w różnych miejscach kodu
            eq = new org.mariuszgromada.math.mxparser.Expression(equationString);

            // Odczytywanie argumentów wpisanego równania
            List<Token> tokensList = eq.getCopyOfInitialTokens();
            tmpSolution.Text = "";
            arguments.Clear();
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

            // Ustawienia listy argumentów
            variablesGrid.AutoGenerateColumns = true;
            variablesGrid.ItemsSource = null;
            variablesGrid.ItemsSource = arguments;
            variablesGrid.Items.Refresh();
            
            // Czyszczenie po wykonaniu funkcji
            argumentsString.Clear();
            tokensList.Clear();
        }
        
        //
        // Wątek odpowiadający za działanie algorytmu
        //
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Główna pętla wątku - wykonuje się tyle razy ile iteracji wprowadził użytkownik
            for (int k = 0; k < algorithm.iterations; k++)
            {
                // Zmienna definiująca postęp w postaci procentowej
                int progressPercentage = Convert.ToInt32(((double)k / algorithm.iterations) * 100);

                // Sortowanie według najlepszych rozwiązań
                if(minmax == 0)
                    population.subjects = population.subjects.OrderBy(o => o.solution).ToList();
                else
                    population.subjects = population.subjects.OrderByDescending(o => o.solution).ToList();

                // Uruchomienie algorytmu
                population = algorithm.RunAlgorithm(population);
                org.mariuszgromada.math.mxparser.Expression equa = new org.mariuszgromada.math.mxparser.Expression();

                // Tworzenie wyrażeń oraz obliczanie rozwiązań aktualnej populacji
                foreach (var sub in population.subjects)
                {
                    equa = new org.mariuszgromada.math.mxparser.Expression("f(" + string.Join(", ", sub.stringValues) + ")", f);
                    sub.solution = equa.calculate();
                }

                // Reportowanie postępu do aktualizacja progressbara
                (sender as BackgroundWorker).ReportProgress(progressPercentage, population.subjects[0]);
            }
            // Po wykonaniu algorytmu zapisujemy najlepszy obiekt w wyniku wątku
            e.Result = population.subjects[0];
        }

        //
        // Zmiana postępu wykonywania algorytmu
        //
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Ustawiamy informacje o pasku postępu
            pbProgress.Value = e.ProgressPercentage;
            txtProgress.Text = e.ProgressPercentage.ToString() + "%";

            // Ustawiamy aktualne rozwiązanie algorytmu (to w konkretnej iteracji - nie najlepsze)
            if (e.UserState != null)
            {
                var sol = (Subject)e.UserState;
                tmpSolution.Text = "#######################################\n";
                tmpSolution.Text += "ROZWIĄZANIE NIE JEST OSTATECZNE. ZACZEKAJ NA KONIEC PROCESU.\n";
                tmpSolution.Text += "Aktualne rozwiązanie:   " + sol.solution + "\n";
                tmpSolution.Text += "#######################################\n";

                // Dodanie tego rozwiązania do listy wszystkich rozwiązań
                allSolutions.Add(sol);

                // Rysowanie tego konkretnego rozwiązania na wykresie
                plotPoint(sol);
            }
        }

        //
        // Funkcja wywoływana po wykonaniu wątku algorytmu
        //
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Działania na progressbar
            pbProgress.Value = 100;
            txtProgress.Text = "100%";

            // Zapisanie najlepszego rozwiązania do nowej zmiennej
            var sol = (Subject)e.Result;

            // Sortowanie według najlepszych rozwiązań
            if (minmax == 0)
            {
                population.subjects = population.subjects.OrderBy(o => o.solution).ToList();
                allSolutions = allSolutions.OrderBy(o => o.solution).ToList();
            }
            else
            {
                population.subjects = population.subjects.OrderByDescending(o => o.solution).ToList();
                allSolutions = allSolutions.OrderByDescending(o => o.solution).ToList();
            }

            // Rysowanie najlepszego rozwiązania
            plotPoint(allSolutions[0]);

            // Wypisanie najlepszego znalezionego rozwiązania
            tmpSolution.Text = "Znalezione rozwiązania przy populacji wielkości " + txtPopulationSize.Text + ":\n";
            tmpSolution.Text += "#######################################\n";
            tmpSolution.Text += "(" + commaSeparatedArguments + ") = " + "(" + string.Join(", ", allSolutions[0].stringValues) + ")" + "\n";
            tmpSolution.Text += "Rozwiązanie:   " + allSolutions[0].solution + "\n";
            tmpSolution.Text += "#######################################\n";

            // Czyszczenie przed kolejnym wywołaniem
            argumentsString.Clear();
            argumentsString = new List<string>();
            eq = new org.mariuszgromada.math.mxparser.Expression();
            System.GC.Collect(); // <- Garbage Collector
            algorithm.ClearAlgorithm();
            population.subjects.Clear();

            // Aktywowanie ponownie przycisków
            btnEquation.IsEnabled = true;
            plotRefresh.IsEnabled = true;
            plotOnlyBest.IsEnabled = true;

            // MessageBox informujący o ukończeniu działania algorytmu
            MessageBox.Show("Obliczono rozwiązania", "Ukończono", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //
        // Zmiana wartości dokładności rysowania wykresów/warstwic
        //
        private void TxtPlot_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!plotBusy)
            {
                if (txtPlot.Text != "")
                    p = System.Int32.Parse(txtPlot.Text);
                if (p < 10)
                {
                    p = 10;
                }
            }

        }

        //
        // Przycisk "Wymuś odświeżenie" - wywołuje rysowanie modelu
        //
        private void PlotRefresh_Click(object sender, RoutedEventArgs e)
        {
            while (plotBusy)
            {
                Thread.Sleep(100);
            }
            plotChart();
        }

        //
        // Przycisk który ponownie zaznaczy na wykresie tylko najlepsze rozwiązanie
        //
        private void PlotOnlyBest_Click(object sender, RoutedEventArgs e)
        {
            if(allSolutions.ToArray().Length>0)
                plotPoint(allSolutions[0]);
        }
    }
}
