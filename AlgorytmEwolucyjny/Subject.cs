using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class Subject
    {
        uint nGenes;
        uint nPairs;
        static Random tmp = new Random(10);
        public List<double> values = new List<double>();
        public double solution;
        //double parameters; ???? do sprawdzenia
        double adaptation;


        // Inicjalizacja osobników
        public void initSubject(int nValues)
        {
            // Do poprawy - to nie powinno być na sztywno + trzeba doczytać do czego to
            nGenes = 10;
            nPairs = 2;
            adaptation = 0;
            // Losowe wartości dla osobnika - doczytać z jakiego zakresu powinny być
            for(int i = 0; i < nValues; i++)
            {
                values.Add(tmp.Next(0, 100));
                System.Console.WriteLine(values[i]);
            }
        }
    }
}
