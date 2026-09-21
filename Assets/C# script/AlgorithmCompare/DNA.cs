using System;


public class DNA<T> 
{
    public int[] Genes { get; private set; }
    public float Fitness { get; private set; }

    private System.Random random;
    private Func<int> getRandomGene;
    private Func<int, float> fitnessFunction;

    public DNA(int size, System.Random random, Func<int> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)  /* */
    {
        Genes = new int[size];
        this.random = random;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        if (shouldInitGenes)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = getRandomGene();
            }
        }
    }

    public float CalculateFitness(int index)
    {
        Fitness = fitnessFunction(index);
       // UnityEngine.Debug.Log("fitness in DNA class " + index + " is " + Fitness); 
      
        return Fitness;
    }

    public DNA<int> Crossover(DNA<int> otherParent)
    {
        DNA<int> child = new DNA<int>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

        for (int i = 0; i < Genes.Length; i++)
        {
            child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];  //
        }

        return child;
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < Genes.Length; i++)
        {
            if (random.NextDouble() < mutationRate)
            {
                Genes[i] = getRandomGene();
            }
        }
    }

    public DNA<int> CloneDNA()
    {
        DNA<int> person = new DNA<int>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);
        Genes.CopyTo(person.Genes,0);
        person.Fitness = Fitness; 
        return person;
    }
}