using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class Subject
    {
        public int nGenes;
        //uint nPairs;
        static Random tmp = new Random(333);
        public List<double> values = new List<double>();
        public List<string> stringValues = new List<string>();
        public ObservableCollection<Arguments> arguments = new ObservableCollection<Arguments>();
        public double solution;
        //double parameters; ???? do sprawdzenia
        //double adaptation; ???? do sprawdzenia


        // Inicjalizacja osobników
        public void initSubject(ObservableCollection<Arguments> args)
        {
            // Do poprawy - to nie powinno być na sztywno + trzeba doczytać do czego to
            arguments = args;
            nGenes = args.ToArray().Length;
            // Losowe wartości dla osobnika - doczytać z jakiego zakresu powinny być
            //for(int i = 0; i < nValues; i++)
            //{
            //    values.Add((tmp.NextDouble() * 20) - 10);

            //    // DO TESTÓW DLA PRZYKŁADU
            //    //values.Add(-0.56);
            //}
            foreach(var arg in args)
            {
                values.Add((tmp.NextDouble() * (arg.Maximum - arg.Minimum) + arg.Minimum));
            }
            valuesToString();
        }

        public void valuesToString()
        {
            foreach(var val in values)
            {
                stringValues.Add(val.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture));
            }
        }
    }
}
