using System;
using System.Collections.Generic;
using System.IO;


public class GeneticAlgorithm<T>
{
    public List<DNA<int>> Population { get; private set; } //list of levels
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public int[] BestGenes { get; private set; }

    public int Elitism;
    public float MutationRate;

    private List<DNA<int>> newPopulation;
    private System.Random random;
    private float fitnessSum;
    private int dnaSize;
    private Func<int> getRandomGene;
    private Func<int, float> fitnessFunction;

    public GeneticAlgorithm(int populationSize, int dnaSize, System.Random random, Func<int> getRandomGene, Func<int, float> fitnessFunction,
        int elitism, float mutationRate = 0.01f)
    {
        Generation = 1;
        Elitism = elitism;
        MutationRate = mutationRate;
        Population = new List<DNA<int>>(populationSize);
        newPopulation = new List<DNA<int>>(populationSize);
        this.random = random;
        this.dnaSize = dnaSize;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        BestGenes = new int[dnaSize];

        for (int i = 0; i < populationSize; i++)
        {
            Population.Add(new DNA<int>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
        }
    }

    public void NewGeneration()  //assume the population is sorted before 
    {


        if (Population.Count <= 0)
        {
          //  UnityEngine.Debug.Log("Population is less or equal to  zero!");
            return;
        }


        newPopulation.Clear();

        for (int i = 0; i < Population.Count; i = i + 2)
        {
            if (i < Elitism && i < Population.Count)  //first elistists
            {
                //first make a copy of 
                newPopulation.Add(Population[i].CloneDNA());  //0
                newPopulation.Add(Population[i + 1].CloneDNA());  //1
            }
            else
            {
                SelectionReproductionMethod(); //methods are crossover, mutation; 
            }

        }
       // UnityEngine.Debug.Log("Population Size" + newPopulation.Count);
        List<DNA<int>> tmpList = Population;
        Population = newPopulation;
        newPopulation = tmpList;

        Generation++;
    }

    public int CompareDNA(DNA<int> a, DNA<int> b)  //
    {
        if (a.Fitness > b.Fitness)
        {
            return 1;
        }
        else if (a.Fitness < b.Fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public void CalculateFitness()  /*???????*/
    {
        fitnessSum = 0;
        DNA<int> best = Population[0]; //later find the best dna
                                       // UnityEngine.Debug.Log("GA class, population[0] fitness is "+ best.CalculateFitness(0));

        for (int i = 0; i < Population.Count; i++)  // i is the ith level in the population
        {

            fitnessSum += Population[i].CalculateFitness(i);  //calculate the ith level fitness sum of cost);

            if (Population[i].Fitness < best.Fitness)
            {

            //    UnityEngine.Debug.Log("best fitness in GA is " + Population[i].Fitness);
                best = Population[i].CloneDNA();
            //    UnityEngine.Debug.Log("best fitness in best is " + best.Fitness);
            }
        }

        BestFitness = best.Fitness;
       
        best.Genes.CopyTo(BestGenes, 0);
    }

    private DNA<int> ChooseParent()
    {
        double randomNumber = random.NextDouble(); // 
       

         for (int i = 0; i < Population.Count; i++)
         {
             if (randomNumber < Population[i].Fitness)
             {
               // UnityEngine.Debug.Log("return parent");
                return Population[i];
             }

             randomNumber -= Population[i].Fitness;
         }
         
        //double select = 0.1;
        

        //for(int i = 0; i < Population.Count; i++)
        //{
        //    double randomNumber = random.NextDouble();
        //    if(randomNumber < 0.1)
        //    {
        //        return Population[i];
        //    }
        //    

        //}

      //  UnityEngine.Debug.Log("return null");
        return null;
    }

    public void SortingPopulation()
    {
        
        Population.Sort(CompareDNA);

    }

    public void SelectionReproductionMethod()  /* 0.5 percatage crossover, */
    {
        double randomNumber = random.NextDouble();
        if (randomNumber > 0.5)  //crossover
        {
            //UnityEngine.Debug.Log("crossover");
            CrossOver();
            CrossOver();

        }
        else //mutated
        {
            //UnityEngine.Debug.Log("mutation");
            Mutation();
            Mutation();
        }
    }

    public void CrossOver()  /*choose two diff parents, cross over and put into new population*/
    {
        DNA<int> parent1 = ChooseParent();
        DNA<int> parent2 = ChooseParent();
        if (parent1.Genes.Equals(parent2.Genes)) { parent2 = ChooseParent(); }  //choose a new parent if two parents are the same
        DNA<int> child = parent1.Crossover(parent2);
        newPopulation.Add(child);
    }

    public void Mutation()  /* create a new copy of parent, muate the new copy and add to population*/
    {
        DNA<int> parent1 = ChooseParent();
        DNA<int> child1 = new DNA<int>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: false);
        parent1.Genes.CopyTo(child1.Genes, 0);
        child1.Mutate(MutationRate);
        newPopulation.Add(child1);


    }
}