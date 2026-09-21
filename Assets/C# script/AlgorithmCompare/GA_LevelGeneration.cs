using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GA_LevelGeneration : MonoBehaviour
{
    public int levelSize = 20;
    public int prefabSize = 10;
    [SerializeField] int populationSize = 20;
    [SerializeField] float mutationRate = 0.01f;
    [SerializeField] int elitism = 5;  // 0 to 5


 

    [Header("stop condition")]
    public float e = 0.001f;
    private int stopcounter = 0;


    private GeneticAlgorithm<int> ga;
    private System.Random random;
    //WavesDefinition wd;
    GAHelper chunkDefScript;
    private string path = "/C# script/AlgorithmCompare/GA.txt";
    private StreamWriter writer;
    private DateTime start;
    private int it;

    void Start()
    {
        chunkDefScript = GAHelper.Instance;
        if (chunkDefScript == null) { Debug.LogError("No script assign");  }
        writer = new StreamWriter(Application.dataPath + path, true);
        start = DateTime.Now;
        //wd = new WavesDefinition(targetDuration, targetAdVar, targetCal, targetMeanIntensity, targetIntensityVar, levelSize, Gender, Weight, Age);
        random = new System.Random();

        ga = new GeneticAlgorithm<int>(populationSize, levelSize, random, GetRandomWave, FitnessFunction, elitism, mutationRate);
        ga.CalculateFitness();
        ga.SortingPopulation();
        StartCoroutine(MyCoroutine(writer));
    }

    //void Update()
    //{

    //    ga.NewGeneration();
    //    ga.CalculateFitness(); //also find the best fitness

    //    int tenth = (int)(populationSize * 0.1);

    //    ga.SortingPopulation();


    //    if ((ga.Population[0].Fitness - ga.Population[tenth].Fitness) < e)
    //    {
    //        stopcounter++;
    //    }

    //    if (stopcounter == 10)
    //    {
    //        print("total generation" + ga.Generation);
    //        print("best fitness" + ga.BestFitness);
    //        print("the best gene is  ");

    //        // for (int i = 0; i < levelSize; i++) { print(ga.Population[0].Genes[i]); print("popu zero fitness is " + ga.Population[0].Fitness ); }
    //        //print("\n");
    //        for (int i = 0; i < levelSize; i++) { print(ga.BestGenes[i]); }

    //        this.enabled = false; }
    //}

    IEnumerator MyCoroutine(StreamWriter wr)
    {
        it = 0; //iteration
        while (stopcounter != 40)
        {
            it++;
            ga.NewGeneration();
            ga.CalculateFitness(); //also find the best fitness
            //print("pop size" + ga.);
            int tenth = (int)(populationSize * 0.1f);
            //print(tenth);
            if(populationSize < 10)
            {
                tenth = 5;
            }
            ga.SortingPopulation();


            if ((ga.Population[0].Fitness - ga.Population[tenth].Fitness) < e)
            {
                stopcounter++;
            }
            else { stopcounter = 0; }
            if (it % 10 == 0) { wr.WriteLine("iteration is " + it); yield return null; }



        }
        //convert
        List<int> dnaList = new List<int>(ga.BestGenes.Length);
        for (int i = 0; i < ga.BestGenes.Length; i++)
        {
            dnaList.Add(ga.BestGenes[i]);
        }

        DateTime end = DateTime.Now;
        writer.WriteLine("start time " + start.ToString("G"));
        wr.WriteLine("total generation" + ga.Generation);
        wr.WriteLine("best fitness" + ga.BestFitness);
        wr.Write("best gene is ");
        for (int i = 0; i < ga.BestGenes.Length; i++) { wr.Write(ga.BestGenes[i]+ " "); }
        wr.WriteLine();
       
        writer.WriteLine("best cost is " + chunkDefScript.CalculateCost(ref dnaList));
        writer.WriteLine("total duration " + chunkDefScript.CurrentDur);
        writer.WriteLine("Adjacent Variation cost" + chunkDefScript.CurrentAdjVarCost);
        writer.WriteLine("Rot cost" + chunkDefScript.currentLevelRotCost);
        writer.WriteLine("Rot cost * weight" + chunkDefScript.currentLevelRotCost * chunkDefScript.rotWeight);

        writer.WriteLine("cm cost" + chunkDefScript.currentLevelCMCost);
        writer.WriteLine("cm cost * weight" + chunkDefScript.currentLevelCMCost* chunkDefScript.cmWeight);

        //writer.WriteLine("level size " + chunkDefScript.LevelSize);

        printEachJointLevelRotation(ref wr, ref dnaList);

        double d = (end - start).TotalMinutes;
        double s = (end - start).TotalSeconds;

        string minute = d.ToString("g");
        string second = s.ToString("g");

        writer.WriteLine("total minute " + minute);
        writer.WriteLine("total second " + second);


        writer.WriteLine("end time is " + end.ToString("G"));

        writer.Close();
        print("Reached the target");

        print("Coroutine done");


        this.enabled = false;
    }

    

    //private WavesDefinition.WaveType GetRandomWave()   //generate a random wave represented by a index #
    private int GetRandomWave()
{ 
        int i = random.Next(prefabSize);  //max prefabsize  

        return i;
    }

    private float FitnessFunction(int index)
    {
        float score = 0;

        DNA<int> dna = ga.Population[index];

        List<int> dnaList = new List<int>(dna.Genes.Length);
        for (int i = 0; i < dna.Genes.Length; i++)
        {
            dnaList.Add(dna.Genes[i]);
        }

        score = chunkDefScript.CalculateCost(ref dnaList); // return 1- cost = score; the loweset cost has highest score 


        return score;
    }
    void printEachJointLevelRotation(ref StreamWriter writer,ref List<int> currentLevel)
    {
        float[] sumrot = new float[chunkDefScript.nRotJoint];
        //calculate sum of dist for each joint in a level

        for (int i = 0; i < currentLevel.Count - 1; i++)
        {
            int j = i + 1;
            int prechunk = currentLevel[i];
            int curentchunk = currentLevel[j];

            chunkDefScript.RotDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            for (int k = 0; k < chunkDefScript.nRotJoint; k++)
            {
                sumrot[k] += chunkDefScript.RotDataFile.Get_Float(k.ToString());
            }
        }

       
        
        float sumError = 0f;

        for (int i = 0; i < 25; i++)
        {
            if (chunkDefScript.ImportantJointRot[i] != 0)
            {
                sumError += Mathf.Abs(chunkDefScript.targetRot[i] - sumrot[i]);
                writer.WriteLine("sumROt for Joint " + i + " " + sumrot[i]);

            }

        }
        writer.WriteLine();
        sumError /= (1f * chunkDefScript.numImportantJointRot);
        writer.WriteLine("avg sumerror " + sumError );

    }

        // private int numCharsPerTextObj;
        //private List<Text> textList = new List<Text>();

        //void Awake()
        //{
        //    numCharsPerTextObj = numCharsPerText / validCharacters.Length;
        //    if (numCharsPerTextObj > populationSize) numCharsPerTextObj = populationSize;

        //    int numTextObjects = Mathf.CeilToInt((float)populationSize / numCharsPerTextObj);

        //    for (int i = 0; i < numTextObjects; i++)
        //    {
        //        textList.Add(Instantiate(textPrefab, populationTextParent));
        //    }
        //}

        //    private void UpdateText(WavesDefinition.WaveType[] bestGenes, float bestFitness, int generation, int populationSize, Func<int, char[]> getGenes)
        //    {
        //        bestText.text = ArrayToString(bestGenes);
        //        bestFitnessText.text = bestFitness.ToString();

        //         numGenerationsText.text = generation.ToString();

        //    //     for (int i = 0; i < textList.Count; i++)
        //    //     {
        //    //         var sb = new StringBuilder();
        //    //         int endIndex = i == textList.Count - 1 ? populationSize : (i + 1) * numCharsPerTextObj;
        //    //         for (int j = i * numCharsPerTextObj; j < endIndex; j++)
        //    //         {
        //    //             foreach (var c in getGenes(j))
        //    //             {
        //    //                 sb.Append(c);
        //    //             }
        //    //             if (j < endIndex - 1) sb.AppendLine();
        //    //         }

        //    //         textList[i].text = sb.ToString();
        //    //     }
        //    // }

        //     private string CharArrayToString(char[] charArray)
        //     {
        //         var sb = new StringBuilder();
        //         foreach (var c in charArray)
        //        {
        //             sb.Append(c);
        //        }

        //         return sb.ToString();
        //     }



    }