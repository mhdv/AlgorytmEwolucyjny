using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class Population
    {
        public int populationSize;
        public List<Subject> subjects = new List<Subject>();

        //
        // Inicjalizacja populacji
        //
        public void initPopulation(int nValues, int popSize)
        {
            // na sztywno wielkość populacji - do wyboru w programie potem
            populationSize = popSize;
            for (int i = 0; i < populationSize; i++)
            {
                Subject newSubject = new Subject();
                newSubject.initSubject(nValues);
                subjects.Add(newSubject);
            }
        }
    }
}
