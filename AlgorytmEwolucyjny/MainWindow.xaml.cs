using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.parsertokens;

/*

PRZYKŁADOWE RÓWNANIE: x1^4+x2^4-0.62*x1^2-0.62*x2^2
ROZWIĄZANIE: pkt(-0.56,-0.56), wartość = -0.19

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
        }

        //
        // Główna funkcja programu - po naciśnięciu zatwierdź - Taki main
        //
        private void btnEquation_Click(object sender, RoutedEventArgs e)
        {
            // Równanie w postaci łańcucha znaków
            equationString = txtEquation.Text;
            // Tymczasowe równanie - później jest nadpisywane
            eq = new org.mariuszgromada.math.mxparser.Expression(equationString);

            // Odczytywanie argumentów wpisanego równania
            List<Token> tokensList = eq.getCopyOfInitialTokens();
            List<List<Argument>> allarguments = new List<List<Argument>>();
            tmpSolution.Text = "";
            foreach (Token t in tokensList)
            {
                if (t.tokenTypeId == Token.NOT_MATCHED)
                {
                    if (t.tokenStr.Contains("x") && t.tokenStr.Length > 1)
                    {
                        if(!argumentsString.Contains(t.tokenStr))
                            argumentsString.Add(t.tokenStr);
                    }
                }
            }

            // Tworzenie populacji i jej inicjalizacja
            Population population = new Population();
            population.initPopulation(argumentsString.ToArray().Length, System.Convert.ToInt32(txtPopulationSize.Text));

            // Tworzenie listy równań
            int i = 0;
            for (int j = 0; j < population.populationSize; j++)
            {
                List<Argument> arguments = new List<Argument>();
                foreach (var arg in argumentsString)
                {
                    Argument tmp = new Argument(arg + " = " + population.subjects[j].values[i]);            
                    arguments.Add(tmp);
                    i++;
                }
                allarguments.Add(arguments);
                i = 0;
            }

            // Oddzielanie argumentów przecinkami
            string commaSeparatedArguments = string.Join(", ", argumentsString);
            // Tworzenie zagnieżdżonej listy wyrażeń
            List<org.mariuszgromada.math.mxparser.Expression> equations = new List<org.mariuszgromada.math.mxparser.Expression>();
            for (int j = 0; j < population.populationSize; j++)
            {
                Function f = new Function("f(" + commaSeparatedArguments + ") = " + equationString);
                org.mariuszgromada.math.mxparser.Expression tmp = new org.mariuszgromada.math.mxparser.Expression("f("+ string.Join(", ", population.subjects[j].stringValues) + ")", f);
                equations.Add(tmp);

                //
                //  DEBUG
                //
                //System.Console.WriteLine(tmp.getExpressionString());
                //tmp.setDescription("Example - Debug");
                //tmp.setVerboseMode();
            }


            // Sprawdzanie składni do parsera
            foreach (var eq in equations)
            {
                if (!eq.checkLexSyntax())
                {
                    tmpSolution.Text = "Błąd składni - używaj składni środowiska MATLAB/WOLFRAM";
                    return;
                }
                if (!eq.checkSyntax())
                {
                    tmpSolution.Text = "Wprowadzaj tylko zmienne x1,x2,x3...";
                    return;
                }
            }

            // Zapisywanie rozwiązania danego osobnika w jego klasie
            i = 0;
            foreach (var eq in equations.ToArray())
            {
                population.subjects[i].solution = eq.calculate();
                i++;
            }

            // Sortowanie rosnąco
            population.subjects = population.subjects.OrderBy(o => o.solution).ToList();

            // tymczasowe rozwiązanie równania
            tmpSolution.Text = "Rozwiązania dla poszczególnych osobników przy populacji wielkości " + txtPopulationSize.Text + ":\n";
            for(int j = 0; j<population.populationSize; j++)
            {
                tmpSolution.Text += "#######################################\n";
                tmpSolution.Text += "(" + commaSeparatedArguments + ") = " + "(" + string.Join(", ", population.subjects[j].stringValues) + ")" + "\n";
                tmpSolution.Text += "Rozwiązanie " + (j + 1).ToString() + "-go osobnika:   " + population.subjects[j].solution.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) + "\n";
            }
            
            // Czyszczenie przed kolejnym wywołaniem
            argumentsString = new List<string>();
            equationString = "";
            eq = new org.mariuszgromada.math.mxparser.Expression();
            System.GC.Collect(); // <- Garbage Collector
        }

        //
        //  Funkcja zapobiega wpisywaniu liter w wielkości populacji
        //
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
