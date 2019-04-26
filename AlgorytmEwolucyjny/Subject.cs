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
        public List<double> values = new List<double>();
        //double parameters; ????
        double adaptation;

        public void initSubject(int nValues)
        {
            // póki co sztywno - potem ustawianie w programie
            nGenes = 10;
            nPairs = 2;
            adaptation = 0;
            //
            Random tmp = new Random();
            for(int i = 0; i < nValues; i++) // doczytać wartości
            {
                values.Add(tmp.Next(0, 100));
            }
        }
    }
}
