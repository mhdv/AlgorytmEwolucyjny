using System.Windows;

namespace AlgorytmEwolucyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string equationString;
        public Expression equation;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEquation_Click(object sender, RoutedEventArgs e)
        {
            equationString = txtEquation.Text;
            equation = new Expression(equationString);
        }
    }
}
