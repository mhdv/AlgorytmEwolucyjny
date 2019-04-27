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
            Algorithm algorithm = new Algorithm();
            // Równanie w postaci łańcucha znaków
            equationString = txtEquation.Text;
            // Tymczasowe równanie - później jest nadpisywane
            eq = new org.mariuszgromada.math.mxparser.Expression(equationString);

            // Odczytywanie argumentów wpisanego równania
            List<Token> tokensList = eq.getCopyOfInitialTokens();
            //List<List<Argument>> allarguments = new List<List<Argument>>();
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

            // Oddzielanie argumentów przecinkami
            string commaSeparatedArguments = string.Join(", ", argumentsString);
            Function f = new Function("f(" + commaSeparatedArguments + ") = " + equationString);
            // Tworzenie populacji i jej inicjalizacja
            Population population = new Population();
            algorithm.AlgorithmInit(comboReproductionMethod.Text, System.Int32.Parse(txtIterations.Text));
            population.initPopulation(argumentsString.ToArray().Length, System.Convert.ToInt32(txtPopulationSize.Text));
            for (int k = 0; k < System.Int32.Parse(txtIterations.Text); k++)
            {
                //// Tworzenie listy równań
                //int i = 0;
                //for (int j = 0; j < population.populationSize; j++)
                //{
                //    List<Argument> arguments = new List<Argument>();
                //    foreach (var arg in argumentsString)
                //    {
                //        Argument tmp = new Argument(arg + " = " + population.subjects[j].values[i]);
                //        arguments.Add(tmp);
                //        i++;
                //    }
                //    allarguments.Add(arguments);
                //    i = 0;
                //}

                // Tworzenie zagnieżdżonej listy wyrażeń
                List<org.mariuszgromada.math.mxparser.Expression> equations = new List<org.mariuszgromada.math.mxparser.Expression>();
                for (int j = 0; j < population.populationSize; j++)
                {
                    org.mariuszgromada.math.mxparser.Expression tmp = new org.mariuszgromada.math.mxparser.Expression("f(" + string.Join(", ", population.subjects[j].stringValues) + ")", f);
                    equations.Add(tmp);


                    ////DEBUG
                    //System.Console.WriteLine(tmp.getExpressionString());
                    //tmp.setDescription("Example - Debug");
                    //tmp.setVerboseMode();
                }


                // Sprawdzanie składni do parsera
                //foreach (var eq in equations)
                //{
                //    if (!eq.checkLexSyntax())
                //    {
                //        tmpSolution.Text = "Błąd składni - używaj składni środowiska MATLAB/WOLFRAM";
                //        return;
                //    }
                //    if (!eq.checkSyntax())
                //    {
                //        tmpSolution.Text = "Wprowadzaj tylko zmienne x1,x2,x3...";
                //        return;
                //    }
                //}

                // Zapisywanie rozwiązania danego osobnika w jego klasie
                int i = 0;
                foreach (var equa in equations)
                {
                    population.subjects[i].solution = equa.calculate();
                    i++;
                }

                // Sortowanie rosnąco
                population.subjects = population.subjects.OrderBy(o => o.solution).ToList();
                
                population = algorithm.RunAlgorithm(population);
                i = 0;
                foreach (var equa in equations)
                {
                    population.subjects[i].solution = equa.calculate();
                    i++;
                }
                equations.Clear();
            }
            population.subjects = population.subjects.OrderBy(o => o.solution).ToList();
            //Console.WriteLine(population.subjects.ToArray()[5].solution);

            // tymczasowe rozwiązanie równania
            tmpSolution.Text = "Znalezione rozwiązanie przy populacji wielkości " + txtPopulationSize.Text + ":\n";
            
            tmpSolution.Text += "#######################################\n";
            tmpSolution.Text += "(" + commaSeparatedArguments + ") = " + "(" + string.Join(", ", population.subjects[0].stringValues) + ")" + "\n";
            tmpSolution.Text += "Rozwiązanie:   " + population.subjects[0].solution.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) + "\n";
            tmpSolution.Text += "#######################################\n";
            
            
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
    }
}
