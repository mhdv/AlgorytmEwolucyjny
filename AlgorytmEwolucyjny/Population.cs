using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class Population
    {
        uint PopulationSize;
        public List<Subject> subjects = new List<Subject>();

        public void initPopulation(int nValues)
        {
            // na sztywno wielkość populacji - do wyboru w programie potem
            PopulationSize = 10;
            for (int i = 0; i < PopulationSize; i++)
            {
                Subject newSubject = new Subject();
                newSubject.initSubject(nValues);
                subjects.Add(newSubject);
            }
        }
    }
}
