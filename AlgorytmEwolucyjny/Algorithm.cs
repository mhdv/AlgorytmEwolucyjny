using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class Algorithm
    {
        public int method;
        public int iterations;
        public int strategy;
        Population actualPopulation;
        Population nextPopulation;
        Population previousPopulation;
        static Random tmp = new Random(333);
        public int minmax;

        public void AlgorithmInit(int meth, int iter, int strat, int mm)
        {
            minmax = mm;
            strategy = strat;
            method = meth;
            iterations = iter;
        }

        public void ClearAlgorithm()
        {
            iterations = 0;
            actualPopulation = new Population();
            nextPopulation = new Population();
        }

        public Population RunAlgorithm(Population pop)
        {
            tmp = new Random(333);
            previousPopulation = actualPopulation;
            if(previousPopulation != null)
            {
                if (strategy == 1)
                    actualPopulation = pop;
                else
                {
                    if (minmax == 0)
                    {
                        if (previousPopulation.subjects[0].solution < pop.subjects[0].solution)
                            actualPopulation = previousPopulation;
                        else
                            actualPopulation = pop;
                    }
                    else
                    {
                        if (previousPopulation.subjects[0].solution > pop.subjects[0].solution)
                            actualPopulation = previousPopulation;
                        else
                            actualPopulation = pop;
                    }

                }
            }
            actualPopulation = pop;
            nextPopulation = new Population();
            nextPopulation.populationSize = actualPopulation.populationSize;
            for (int i = 0; i < actualPopulation.populationSize; i++)
            {
                Subject child = new Subject();
                Subject firstParent = new Subject();
                Subject secondParent = new Subject();
                double firstChance = tmp.NextDouble();
                int index = 0;

                if (firstChance <= 0.4 * actualPopulation.populationSize)
                {
                    index = 0;
                    firstParent = actualPopulation.subjects.ToArray()[0];
                }
                else if (firstChance <= 0.60 * actualPopulation.populationSize)
                {
                    index = 1;
                    firstParent = actualPopulation.subjects.ToArray()[1];
                }
                else if (firstChance <= 0.75 * actualPopulation.populationSize)
                {
                    index = 2;
                    firstParent = actualPopulation.subjects.ToArray()[2];
                }
                else if (firstChance <= 0.85 * actualPopulation.populationSize)
                {
                    index = 3;
                    firstParent = actualPopulation.subjects.ToArray()[3];
                }
                else if (firstChance > 0.85 * actualPopulation.populationSize)
                {
                    index = 4;
                    firstParent = actualPopulation.subjects.ToArray()[new Random().Next(4,actualPopulation.populationSize-1)];
                }
                secondParent = actualPopulation.subjects.ToArray()[index + 1];

                switch (method)
                {
                    case 0:
                        child = ReproduceDefault(firstParent, secondParent);
                        break;
                    case 1:
                        child = ReproduceMean(firstParent, secondParent);
                        break;
                    case 2:
                        child = ReproduceTwopoint(firstParent, secondParent);
                        break;
                    default:
                        child = ReproduceDefault(firstParent, secondParent);
                        break;
                }
                nextPopulation.subjects.Add(child);
            }
            return nextPopulation;
        }

        private Subject ReproduceDefault(Subject first, Subject second)
        {
            Subject child = new Subject();

            child.nGenes = first.nGenes;
            for (int i = 0; i < first.nGenes; i++)
            {
                double y = 0.6 * first.values.ToArray()[i] + 0.4 * second.values.ToArray()[i] + 0.01 * ((tmp.NextDouble() * 20) - 10);
                child.values.Add(y);
            }
            child.stringValues.Clear();
            child.valuesToString();
            return child;
        }

        private Subject ReproduceMean(Subject first, Subject second)
        {
            if (iterations % 2 == 0)
            {
                Subject child = new Subject();

                child.nGenes = first.nGenes;
                for (int i = 0; i < first.nGenes; i++)
                {
                    double y = first.values.ToArray()[i] + tmp.NextDouble() * (second.values.ToArray()[i] - first.values.ToArray()[i]) + 0.01 * ((tmp.NextDouble() * 20) - 10);
                    child.values.Add(y);
                }
                child.stringValues.Clear();
                child.valuesToString();
                return child;
            }
            else
            {
                Subject child = new Subject();

                child.nGenes = first.nGenes;
                for (int i = 0; i < first.nGenes; i++)
                {
                    double y = first.values.ToArray()[i] + tmp.NextDouble() * (second.values.ToArray()[i] - first.values.ToArray()[i]) + 0.01 * ((tmp.NextDouble() * 20) - 10);
                    double z = second.values.ToArray()[i] + first.values.ToArray()[i] - y;
                    child.values.Add(z);
                }
                child.stringValues.Clear();
                child.valuesToString();
                return child;

            }
        }

        private Subject ReproduceTwopoint(Subject first, Subject second)
        {
            Subject child = new Subject();

            if (tmp.NextDouble() < 0.5)
            {
                child.nGenes = first.nGenes;
                for (int i = 0; i < first.nGenes; i++)
                {
                    double y = first.values.ToArray()[i] + 0.01 * ((tmp.NextDouble() * 20) - 10);
                    child.values.Add(y);
                }
                child.stringValues.Clear();
                child.valuesToString();
                return child;
            }
            else
            {
                child.nGenes = first.nGenes;
                for (int i = 0; i < first.nGenes; i++)
                {
                    double y = second.values.ToArray()[i] + 0.01 * ((tmp.NextDouble() * 20) - 10);
                    child.values.Add(y);
                }
                child.stringValues.Clear();
                child.valuesToString();
                return child;
            }
        }

    }
}
