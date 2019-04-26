using System.Windows;
using org.mariuszgromada.math.mxparser;

namespace AlgorytmEwolucyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string equationString;
        public org.mariuszgromada.math.mxparser.Expression equation;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEquation_Click(object sender, RoutedEventArgs e)
        {
            equationString = txtEquation.Text;
            equation = new org.mariuszgromada.math.mxparser.Expression(equationString);
            tmpSolution.Text = "Rozwiązanie: " + equation.calculate().ToString();
        }
    }
}
