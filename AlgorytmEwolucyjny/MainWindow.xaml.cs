using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.parsertokens;

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

        public MainWindow()
        {
            InitializeComponent();
            InitializeOtherComponents();
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
            // Initialize comboReproductionMethod
            comboReproductionMethod.Items.Add("Domyślna");
        }

        //
        // Główna funkcja programu - po naciśnięciu zatwierdź - Taki main
        //
        private void btnEquation_Click(object sender, RoutedEventArgs e)
        {
            if(System.Int32.Parse(txtPopulationSize.Text) <= 5)
            {
                MessageBox.Show("Populacja powinna być większa niż 5!");
                return;
            }
            // Definiowanie algorytmu
            Algorithm algorithm = new Algorithm();

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
                    if (t.tokenStr.Contains("x") && t.tokenStr.Length > 1)
                    {
                        if (!argumentsString.Contains(t.tokenStr))
                            argumentsString.Add(t.tokenStr);
                    }
                }
            }

            // Oddzielanie argumentów przecinkami oraz utworzenie funkcji
            string commaSeparatedArguments = string.Join(", ", argumentsString);
            Function f = new Function("f(" + commaSeparatedArguments + ") = " + equationString);

            // Tworzenie populacji i jej inicjalizacja / inicjalizacja algorytmu
            Population population = new Population();
            algorithm.AlgorithmInit(comboReproductionMethod.Text, System.Int32.Parse(txtIterations.Text));
            population.initPopulation(argumentsString.ToArray().Length, System.Convert.ToInt32(txtPopulationSize.Text));

            for (int k = 0; k < System.Int32.Parse(txtIterations.Text); k++)
            {
                // Sortowanie według najlepszych rozwiązań
                population.subjects = population.subjects.OrderBy(o => o.solution).ToList();

                // Uruchomienie algorytmu
                population = algorithm.RunAlgorithm(population);

                // Tworzenie wyrażeń oraz obliczanie rozwiązań aktualnej populacji
                foreach(var sub in population.subjects)
                {
                    org.mariuszgromada.math.mxparser.Expression equa = new org.mariuszgromada.math.mxparser.Expression("f(" + string.Join(", ", sub.stringValues) + ")", f);
                    sub.solution = equa.calculate();
                }
            }
            population.subjects = population.subjects.OrderBy(o => o.solution).ToList();
            List<Subject> solutions = new List<Subject>();
            solutions.Add(population.subjects[0]);
            solutions.AddRange(CheckOtherSolutions(population.subjects[0]));

            // tymczasowe rozwiązanie równania
            tmpSolution.Text = "Znalezione rozwiązania przy populacji wielkości " + txtPopulationSize.Text + ":\n";
            
            foreach(var sol in solutions)
            {
                tmpSolution.Text += "#######################################\n";
                tmpSolution.Text += "(" + commaSeparatedArguments + ") = " + "(" + string.Join(", ", sol.stringValues) + ")" + "\n";
                tmpSolution.Text += "Rozwiązanie:   " + sol.solution.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + "\n";
                tmpSolution.Text += "#######################################\n";
            }
            
            
            // Czyszczenie przed kolejnym wywołaniem
            argumentsString = new List<string>();
            equationString = "";
            eq = new org.mariuszgromada.math.mxparser.Expression();
            System.GC.Collect(); // <- Garbage Collector
            algorithm.ClearAlgorithm();
            population.subjects.Clear();
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

        private List<Subject> CheckOtherSolutions(Subject best)
        {
            List<Subject> listOfAlternatives = new List<Subject>();
            string commaSeparatedArguments = string.Join(", ", argumentsString);
            Function f = new Function("f(" + commaSeparatedArguments + ") = " + equationString);
            Subject potentialAlternative = new Subject();
            potentialAlternative.values = best.values.Select(i => -1 * i).ToList();
            potentialAlternative.valuesToString();
            org.mariuszgromada.math.mxparser.Expression equa = new org.mariuszgromada.math.mxparser.Expression("f(" + string.Join(", ", potentialAlternative.stringValues) + ")", f);
            potentialAlternative.solution = equa.calculate();
            
            if(potentialAlternative.solution == best.solution)
            {
                listOfAlternatives.Add(potentialAlternative);
                return listOfAlternatives;
            }

            return new List<Subject>();
        }
    }
}
