using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlgorytmEwolucyjny
{
    /// <summary>
    /// Logika interakcji dla klasy wykres.xaml
    /// </summary>
    public partial class wykres : Window
    {
        public wykres()
        {
            this.MyModel = new PlotModel { Title = "Example 1" };
            this.MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            InitializeComponent();
        }
        public PlotModel MyModel { get; private set; }
    }
}
