using System.Collections.Generic;
using System.Windows;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.parsertokens;

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

        private void btnEquation_Click(object sender, RoutedEventArgs e)
        {
            equationString = txtEquation.Text;
            //equationString = "f(x) = " + equationString;
            //Function eq = new Function(equationString);
            
            eq = new org.mariuszgromada.math.mxparser.Expression(equationString);
            List<Token> tokensList = eq.getCopyOfInitialTokens();
            List<Argument> arguments = new List<Argument>();
            tmpSolution.Text = "";
            foreach (Token t in tokensList)
            {
                if (t.tokenTypeId == Token.NOT_MATCHED)
                {
                    if (t.tokenStr.Contains("x"))
                    {
                        tmpSolution.Text += ("token = " + t.tokenStr + ", hint = " + t.looksLike) + "\n";
                        argumentsString.Add(t.tokenStr);
                    }
                }
            }

            Population population = new Population();
            population.initPopulation(argumentsString.ToArray().Length);


            int i = 0;
            //for(int j = 0; j < argumentsString.ToArray().Length; j++)
            //{
                foreach (var arg in argumentsString)
                {
                    Argument tmp = new Argument(arg + " = " + population.subjects[0].values[i]);
                    arguments.Add(tmp);
                    i++;
                }
            //}

            eq = new org.mariuszgromada.math.mxparser.Expression(equationString,arguments.ToArray());

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


            // tymczasowe rozwiązanie równania
            tmpSolution.Text = "Rozwiązanie: " + eq.calculate().ToString() + " DLA:\n"
                              + "x1 = " + population.subjects[9].values[0] + "\n"
                              + "x2 = " + population.subjects[9].values[1] + "\n";
        }
    }
}
