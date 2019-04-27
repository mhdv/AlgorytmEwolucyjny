using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class Algorithm
    {
        public string method;
        public int iterations;
        Population actualPopulation;
        Population nextPopulation;
        static Random tmp = new Random(10);

        public void AlgorithmInit(string meth, int iter)
        {
            if (meth == "Domyślna")
                method = "default";
            else
                method = meth;
            iterations = iter;
        }

        public void ClearAlgorithm()
        {
            method = "";
            iterations = 0;
            actualPopulation = new Population();
            nextPopulation = new Population();
        }

        public Population RunAlgorithm(Population pop)
        {
            tmp = new Random(10);
            actualPopulation = pop;
            nextPopulation = new Population();
            nextPopulation.populationSize = actualPopulation.populationSize;
            for(int i = 0; i<actualPopulation.populationSize; i++)
            {
                Subject child = new Subject();
                Subject firstParent = new Subject();
                Subject secondParent = new Subject();
                double firstChance = tmp.NextDouble() * actualPopulation.populationSize;
                double secondChance = tmp.NextDouble() * actualPopulation.populationSize;
                int index = 0;

                if(firstChance <= 0.4 * actualPopulation.populationSize)
                {
                    index = 0;
                    firstParent = actualPopulation.subjects.ToArray()[0];
                }
                else if(firstChance <= 0.60 * actualPopulation.populationSize)
                {
                    index = 1;
                    firstParent = actualPopulation.subjects.ToArray()[1];
                }
                else if(firstChance <= 0.75 * actualPopulation.populationSize)
                {
                    index = 2;
                    firstParent = actualPopulation.subjects.ToArray()[2];
                }
                else if(firstChance <= 0.85 * actualPopulation.populationSize)
                {
                    index = 3;
                    firstParent = actualPopulation.subjects.ToArray()[3];
                }
                else if(firstChance > 0.85 * actualPopulation.populationSize)
                {
                    index = 4;
                    firstParent = actualPopulation.subjects.ToArray()[4];
                }
                secondParent = actualPopulation.subjects.ToArray()[index + 1];

                if (method == "default")
                    child = ReproduceDefault(firstParent, secondParent);
                nextPopulation.subjects.Add(child);
            }
            iterations++;
            actualPopulation = new Population();
            return nextPopulation;
        }

        private Subject ReproduceDefault(Subject first, Subject second)
        {
            Subject child = new Subject();
            if ((tmp.NextDouble() * actualPopulation.populationSize) < 0.2)
            {
                child.initSubject(first.nGenes);
                return child;
            }
            else
            {
                child.nGenes = first.nGenes;
                for (int i = 0; i < first.nGenes; i++)
                {
                    child.values.Add(0.6 * first.values.ToArray()[i] + 0.4 * second.values.ToArray()[i] + 0.01 * ((tmp.NextDouble() * 20) - 10));
                }
                child.stringValues.Clear();
                child.valuesToString();
                return child;
            }
        }
    }
}
