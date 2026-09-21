using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiaoRJMCMC : MonoBehaviour
{
    enum Selection { Add, Remove, Modify };
    Selection curSelection = Selection.Add;

    public int PrefabSize = 10; //size of prefab;  counting the origin pose
    int[] chunkOccurrence;
    public List<int> currentLevel;
    public List<int> currentLevelIndex; //store level array index;
    public RJMCMCHelper helperScript;
    public Output outputScript;
    //public Output outputScript;

    public float bestCost;
    public float curCost;
    float newValue, bestValue;
    public int TargetIteration;
    public int Iteration; //user input
    float temperature;
    public float MaxTemperature = 0.5f;
    float beta;
    //float acceptance; //posibility of acceptance
    public bool begin = false;

    int oldchunk;
    int oldIndex;
    int oldIndexNo;
    float oldcost; // save old cost for less calculation

    int bestLength;

    public int Wi = 5;  // can change
    int W;

    float pa = 0.4f;
    float pr = 0.2f;
    float pm = 0.4f;


    [Header("UserInput")]
    public float targetDuration;
    public float targetAdVar;


    [Header("Weight:  Adajacent, Duration")]
    public float adVarWeight = 0.3f;
    public float DurationWeight = 0.3f;

    [Header("Fixed part")]
    public bool fixedPartEnable = false;
    public int[] fixedSequence = { 1, 6, 1, 6, 1 };
    [Header("Other Section/Output")]
    public string levelFilename = "biao/test";
    //public float CurrentCal = 0f;
    public float currentLevelDistCost = 0f;
    public float currentLevelRotCost = 0f;
    public float currentLevelCMCost = 0f;
    public float CurrentDur = 0f;
    public float CurrentAdjVar = 0f;
    public float CurrentDurCost = 0f;
    public float CurrentAdjVarCost = 0f;
    bool GameInitial = false;

    public AnimationCurve curve;

    //output only

    //public RJMCMCController controller;
    public float[] defaultWeights = { 1f, 1f, 1f };
    public float[] highDistWeight = { 1, 0.3f, 0.3f };
    public float[] highRotWeight = { 0.3f, 1, 0.3f };
    public float[] highcmWeight1 = { 0.3f, 0.3f, 1 };
    public int levelcounter = 0;
    // Use this for initialization
    void Start()
    {
        helperScript = RJMCMCHelper.Instance;
        outputScript = Output.Instance;
        if (helperScript == null) { Debug.LogError("Cannot find optimization helper Script!!"); }

    }
    void StartRJMCMC()
    {
        GameInitial = false;
        //one array store the object; one array store Rnd Pick #;
        currentLevel = new List<int>();
        currentLevelIndex = new List<int>();
        chunkOccurrence = new int[PrefabSize];


        W = Wi * (PrefabSize - 1);
        bestLength = 0;

        Iteration = TargetIteration;
        temperature = MaxTemperature;
        curSelection = Selection.Add;


        //Add  a chunk; 
        //We fix the first chunk to be type 0
        //!!
        int index = 0;
        chunkOccurrence[index] += 1;
        oldchunk = index;
        oldIndex = 0;
        currentLevel.Insert(oldIndex, oldchunk);
        currentLevelIndex.Insert(oldIndex, index);
        print(curSelection + "  " + currentLevel.Count + levelFilename);

        if (fixedPartEnable)
        {
            for (int i = 1; i <= fixedSequence.Length; i++)
            {
                currentLevel.Insert(i, fixedSequence[i - 1]); // add firxed sequece chunk to current level
                currentLevelIndex.Insert(i, fixedSequence[i - 1]);
                chunkOccurrence[fixedSequence[i - 1]] += 1;
            }

        }

        bestCost = Mathf.Abs(CalculateCost());
        // print(Mathf.Abs(CalculateCost()));
        bestValue = UseFormula(bestCost);

        float possibleDuration = W * 10f; //Max possible duration 
        if (targetDuration > possibleDuration) { Debug.Log("target duration value is too large!"); targetDuration = possibleDuration; } //if  target duration is greater than possible duration
        curve = new AnimationCurve();

        begin = true;


    }

    // optimazation
    void Update()
    {
        if (begin)
        {
            if (!GameInitial)
            {
                for (int i = 0; i < 5000; i++)
                {
                    Optimization();
                    if (GameInitial)
                    {
                        break;
                    }
                }
            }
        }
    }


    //void Update()
    //{
    //    if (!GameInitial)
    //    {
    //        Optimization();
    //    }

    //}
    //return true if it is ends
    void Optimization()
    {
        if (Iteration <= 0 && !GameInitial)
        {
            begin = false;
            GameInitial = true;
            CalculateCost();
            PrintRJMCMCResult();
            clearEverything();
            levelcounter++;
            ReInitilizeMCMC();

            // this.enabled = false;


        }
        else
        {
            Iteration--;
            SelectMethod();


            //calculate the cur cost
            curCost = Mathf.Abs(CalculateCost());

            //put it in formula and update accetance rate
            newValue = UseFormula(curCost);

            float rnd = UnityEngine.Random.value;
            float acceptance = UpdateAcceptance();
            acceptance = Mathf.Pow(acceptance, 10);
            // Debug.Log("cur cos" + curCost + "  best cost " + bestCost + "  accetance  " + acceptance + "rnd" + rnd + "\n curValue " + newValue + "  bestvalue" + bestValue +"  current temp " + temperature + "  current length " + currentLevel.Count + " best length " + bestLength);




            if (fixedPartEnable)
            {
                if ((rnd < acceptance) && CheckFixedPartExist())
                {
                    // Debug.Log("Accept "+currentLevel.Count );
                    //  print("accept " + curSelection);

                    //

                    bestCost = curCost;
                    bestValue = newValue;
                    bestLength = currentLevel.Count;
                    //   Debug.Log("Accept" + bestCost );
                }
                else
                {
                    // Debug.Log("Reject");

                    Reverse();

                    // print("reverse " + s);
                }
            }
            else
            {
                if ((rnd < acceptance))
                {
                    // Debug.Log("Accept "+currentLevel.Count );
                    //  print("accept " + curSelection);

                    //

                    bestCost = curCost;
                    bestValue = newValue;
                    bestLength = currentLevel.Count;
                    //   Debug.Log("Accept" + bestCost );
                }
                else
                {
                    // Debug.Log("Reject");

                    Reverse();

                    // print("reverse " + s);
                }
            }



            //make temp drops faster
            temperature = ((float)Iteration / TargetIteration) * MaxTemperature + 0.001f;
            if ((targetAdVar - Iteration) % 1000 == 0)
            {
                curve.AddKey((1f - 1f * Iteration / TargetIteration), bestCost);
            }

        }

    }

    public void ReInitilizeMCMC()
    {
        if (levelcounter == 0)
        {

            levelFilename = "biao/medianrot";
            helperScript.targetfile = "biao/medianrottarget";
            helperScript.LoadTargetsFile();
            StartRJMCMC();

            print("begin median dist");

        }
        else if (levelcounter == 1)
        {
            
            print("finish median rot" + Iteration);
            levelFilename = "biao/highrot";
            helperScript.targetfile = "biao/highrottarget";
            helperScript.LoadTargetsFile();
            StartRJMCMC();
            print("begin high rot");
        }
        else if (levelcounter == 2)
        {
            //enabled = true;
            print("finish high rot" + Iteration + "begin" + begin);
            levelFilename = "biao/highcm";
            helperScript.targetfile = "biao/mediancmtarget";
            helperScript.LoadTargetsFile();
            print("start median cm");
            StartRJMCMC();


        }
        else if (levelcounter == 3)
        {
            print("finish median cm" + Iteration);
            levelFilename = "biao/highcm";
            helperScript.targetfile = "biao/highcmtarget";
            helperScript.LoadTargetsFile();
            print("start high cm");
            StartRJMCMC();
        }

        //else if (levelcounter == 3)//gen upper importance joint
        //{
        //    print("finish cm" + Iteration + "begin" + begin);
        //    levelFilename = "upper";
        //    helperScript.targetfile = "upperonlytarget";
        //    helperScript.LoadTargetsFile();
        //    helperScript.distWeight = defaultWeights[0];
        //    helperScript.rotWeight = defaultWeights[1];
        //    helperScript.cmWeight = defaultWeights[2];
        //    print("start generate upper");
        //    StartRJMCMC();


        //}
        //else if (levelcounter == 4)
        //{
        //    print("finish upper only" + Iteration + "begin" + begin);
        //    levelFilename = "lower"; //output filename
        //    helperScript.targetfile = "loweronlytarget";
        //    helperScript.LoadTargetsFile();
        //    print("start generate lower");
        //    StartRJMCMC();
        //}
        //else if (levelcounter == 5)
        //{
        //    print("finish lower only " + Iteration + "begin" + begin);
        //    levelFilename = "shortduration";
        //    helperScript.targetfile = "target";
        //    helperScript.LoadTargetsFile();
        //    helperScript.distWeight = defaultWeights[0];
        //    helperScript.rotWeight = defaultWeights[1];
        //    helperScript.cmWeight = defaultWeights[2];
        //    targetDuration = 30;
        //    print("start generate short duration");
        //    StartRJMCMC();
        //}
        //else if (levelcounter == 6)
        //{
        //    print("finish short duration " + Iteration + "begin" + begin);
        //    levelFilename = "longduration";
        //    helperScript.targetfile = "lsdurtarget";
        //    helperScript.LoadTargetsFile();
        //    helperScript.distWeight = defaultWeights[0];
        //    helperScript.rotWeight = defaultWeights[1];
        //    helperScript.cmWeight = defaultWeights[2];
        //    targetDuration = 60;
        //    DurationWeight = 2;
        //    print("start generate long duration");
        //    StartRJMCMC();
        //}
        //else if (levelcounter == 7)
        //{
        //    print("finish long duration " + Iteration + "begin" + begin);
        //    levelFilename = "fixted";
        //    fixedPartEnable = true;
        //    fixedSequence = new int[] { 4, 7, 4, 7, 4, 7 };
        //    targetDuration = 72;
        //    print("start generate short duration");
        //    StartRJMCMC();
        //}
        //else if (levelcounter == 8)
        //{

        //}
        else
        {
            levelFilename = "extra";

            print("finish everything");

            begin = false;
        }

    }
    private float CalculateCost()
    {
        float cost = 0f;
        CurrentDurCost = CalculateChunkDurationCost();
        cost += CurrentDurCost * DurationWeight;
        CurrentAdjVarCost = CalculateAdVariationCost();
        cost += CurrentAdjVarCost * adVarWeight;

        //currentLevelDistCost = helperScript.CalculateDistCost(currentLevel);
        //cost += currentLevelDistCost * helperScript.distWeight;

        currentLevelRotCost = helperScript.CalculateRotCost(currentLevel);
        cost += currentLevelRotCost * helperScript.rotWeight;

        currentLevelCMCost = helperScript.CalculateCMCost(currentLevel);
        cost += currentLevelCMCost * helperScript.cmWeight;
        return cost;
    }
    private float CalculateAdVariationCost()
    {
        float c = 0f;
        if (currentLevel.Count <= 1)
        {
            return c;
        }

        for (int i = 0; i < (currentLevel.Count - 1); i++)
        {
            if (currentLevel[i] == currentLevel[i + 1])
            {
                c += 1;
            }
        }

        c = c * (1f / (currentLevel.Count - 1));
        CurrentAdjVar = c;
        c = Mathf.Abs(c - targetAdVar);  //if target Var is 1, it means many same chunks
                                         // print("Adjacent cost " + c);
        return c;
    }

    float CalculateChunkDurationCost()
    {
        float levelduration = 0f; //level duratioin
        float sigma = targetDuration;

        levelduration = CalculateChunkDuration();
        CurrentDur = levelduration; //update in inspector

        float cost = (levelduration - targetDuration) * (levelduration - targetDuration);
        cost = -1f * cost / (2f * sigma * sigma);
        cost = 1f - Mathf.Exp(cost);
        //  print("totaltime" + c + "  durationC " + answer +"  best  cost   "+bestCost + "iteration" + Iteration);
        return cost;
    }
    float CalculateChunkDuration()
    {
        float sumDur = 0f;
        for (int i = 1; i < currentLevel.Count; i++)
        {
            if (currentLevel[i] == 1|| currentLevel[i] == 2 || currentLevel[i] == 3 ||
                currentLevel[i] == 4)
            {
                sumDur += 4f; //hold chunk are 4s
            }
            else
            {
                sumDur += 2f;
            }
        }
        return sumDur;
    }
    //random function 1~prefabsize
    int Rnd()
    {
        return UnityEngine.Random.Range(1, PrefabSize);
    }

    void SelectMethod()
    {
        float select = UnityEngine.Random.value;
        //print("select " + select);
        if (currentLevel.Count <= 1)
        {  //cannot remove the level if it is too short
            curSelection = Selection.Add;

            Add();
            return;

        }
        //add a wave; 
        if (select <= 0.4f)
        {
            curSelection = Selection.Add;
            Add();

        }
        else if (select > 0.4f && select <= 0.8f)
        {
            curSelection = Selection.Modify;
            Replace();
            //  Debug.Log(" Action: Replace " + Iteration);
            //replace a wave; 

        }
        else
        {
            curSelection = Selection.Remove;
            Remove();
            //   Debug.Log(" Action: REMOVE " + Iteration);
            //remove a wave; 

        }

    }

    float UpdateAcceptance()
    {
        if (curSelection == Selection.Modify) //replace
        {
            float a1 = Mathf.Min(newValue / bestValue, 1.0f);
            //Debug.Log(" acceptance rate modify ");
            //      Debug.Log("replace" + "acceptance rate " + a1);
            return a1;
        }
        else if (curSelection == Selection.Remove) //remove
        {
            //  Debug.Log(" acceptance rate remove ");
            //Debug.Log("remove" + "bestlength"+ bestLength + "cur length" + currentLevel.Count);
            float v1 = pa / pr;
            float v2 = newValue / bestValue;
            float v3 = (bestLength * 1.0f) / (W - currentLevel.Count);
            float a2 = v1 * v2 * v3;
            //    Debug.Log("v1  " + v1 + "  v2  " + v2 + "  v3  " + v3);
            a2 = Mathf.Min(1, a2);
            return a2;

        }
        else //if (method.Equals("add"))
        {
            float v1 = pr / pa;
            float v2 = newValue / bestValue;
            float v3 = ((W - bestLength) * 1.0f / currentLevel.Count);
            //  Debug.Log("add" + "bestlength" + bestLength + "cur length" + currentLevel.Count);
            float a3 = v1 * v2 * v3;
            a3 = Mathf.Min(a3, 1);

            // Debug.Log(" acceptance rate add ");
            //   Debug.Log("add"+ "bestlength" + bestLength + "cur length" + currentLevel.Count);
            return a3;
        }

    }
    float UseFormula(float c1)
    {
        beta = 1f / temperature;
        // beta = beta * 1.8f;
        //   if (beta > 200) { beta = 200; }

        float exp = Mathf.Exp(-1.0f * beta * c1);

        return exp;
    }
    bool CheckFixedPartExist()
    {
        if (currentLevel.Count < fixedSequence.Length) { return true; } //if there is not enough to compare, skip it 

        int counter = 0;

        for (int i = 0; i < (currentLevel.Count - fixedSequence.Length); i++)
        {
            for (int j = 0; j < fixedSequence.Length; j++)
            {
                // print((i + j) + " " + j);
                if (currentLevel[i + j] == fixedSequence[j])
                {

                    counter++;
                }
                else
                {
                    counter = 0;
                }
            }
            if (counter == fixedSequence.Length)
            {
                // print("Find it");
                return true;
            }

        }
        return false;
    }
    void Replace()
    {
        // Debug.Log("replace");
        //randomly select a number as index to delete from current level
        //we fix the first chunk
        oldIndex = UnityEngine.Random.Range(1, currentLevel.Count);
        oldchunk = currentLevel[oldIndex];     //save the old chunk
                                               // print("old speed" + oldOne.speedCost + "old wave" + oldOne.Name);
        oldIndexNo = currentLevelIndex[oldIndex];  //OldIndexNo is waveNO

        int pick = Rnd(); //randomly pick a chunk
        if (chunkOccurrence[oldIndexNo] > 0)
        {
            chunkOccurrence[oldIndexNo] -= 1;
        }

        while (chunkOccurrence[pick] >= Wi)
        {
            pick = Rnd(); //randomly pick a chunk
        }
        chunkOccurrence[pick] += 1;

        currentLevel[oldIndex] = pick;
        currentLevelIndex[oldIndex] = pick;

        // print("new speed" + currentLevel[oldIndex].speedCost + "new name "+currentLevel[oldIndex].Name);

    }

    void Add()
    { //randomly add one wave 

        int index = Rnd();

        while (chunkOccurrence[index] >= Wi)
        {
            index = Rnd();
        }

        chunkOccurrence[index] += 1;

        oldchunk = index;
        //choose one place from level to add
        //we fixed the first one
        oldIndex = UnityEngine.Random.Range(1, currentLevel.Count);

        currentLevel.Insert(oldIndex, oldchunk);
        currentLevelIndex.Insert(oldIndex, index);

    }

    
    void Remove()
    {
        if (currentLevel.Count == 0)
        {
            // Debug.LogError("cannot remove");
            return;
        }

        //choose one num as level index from level to remove
        //we fixed the first one 
        oldIndex = UnityEngine.Random.Range(1, currentLevel.Count);

        oldchunk = currentLevel[oldIndex];
        oldIndexNo = currentLevelIndex[oldIndex];
        if (chunkOccurrence[oldIndexNo] > 0)
        {
            chunkOccurrence[oldIndexNo] -= 1;
        }
        currentLevel.RemoveAt(oldIndex);
        //print("removing" );
        currentLevelIndex.RemoveAt(oldIndex);
    }


    void Reverse()
    {

        //if is add;remove the wave
        //if is remove; add the wave back
        //if is replece; replace it back

        if (curSelection == Selection.Modify)
        {
            //revert replace method

            currentLevel[oldIndex] = oldchunk;  //put the old object back

            if (chunkOccurrence[currentLevelIndex[oldIndex]] > 0) //decrease corresponding chunk number
            {
                chunkOccurrence[currentLevelIndex[oldIndex]] -= 1;
            }

            currentLevelIndex[oldIndex] = oldIndexNo; //restore old index back
            chunkOccurrence[oldIndexNo] += 1;

        }
        else if (curSelection == Selection.Remove)
        {  //revert remove method

            currentLevel.Insert(oldIndex, oldchunk); //add the wave back 
            currentLevelIndex.Insert(oldIndex, oldIndexNo);
            chunkOccurrence[oldIndexNo] += 1;

        }
        else
        {  //reveret add method

            if (chunkOccurrence[currentLevelIndex[oldIndex]] > 0) //decrease corresponding waves number
            {
                chunkOccurrence[currentLevelIndex[oldIndex]] -= 1;
            }

            currentLevel.RemoveAt(oldIndex); //remove the new wave and corresponding wave number
            currentLevelIndex.RemoveAt(oldIndex);

        }


    }

    void PrintRJMCMCResult()
    {
        outputScript.InitializeInifile();

        printEachJointLevelRotation();
        printSumCM();

        //outputScript.CreateSection("ImportanceDistJoint");
        //outputScript.PrintEntireFloatArray(helperScript.ImportantJointDist, "ImportanceDistJoint");
        outputScript.CreateSection("ImportanceRotJoint");
        outputScript.PrintEntireFloatArray(helperScript.ImportantJointRot, "ImportanceRotJoint");


        outputScript.CreateSection("MCMC Prior targets");
        outputScript.printLevelTargets(targetAdVar, targetDuration);
        //outputScript.CreateSection("MCMC Dist targets");
        //outputScript.printFloatArray(helperScript.targetDist);
        outputScript.CreateSection("MCMC Rot targets");
        outputScript.printFloatArray(helperScript.targetRot);
        outputScript.CreateSection("MCMC CM targets");
        outputScript.printFloat(helperScript.targetCM);


        outputScript.CreateSection("LevelCost");
        outputScript.printCurrentLevelCost(CurrentAdjVar, CurrentDur, CurrentAdjVarCost, CurrentDurCost,
            CurrentAdjVarCost * adVarWeight, 
                    currentLevelDistCost, currentLevelRotCost, currentLevelCMCost,
                     currentLevelRotCost * helperScript.rotWeight,
                    currentLevelCMCost * helperScript.cmWeight);


        outputScript.CreateSection("MCMC Weights");
        outputScript.printLevelWeight(adVarWeight, DurationWeight, helperScript.rotWeight, helperScript.cmWeight);

        outputScript.CreateSection("MCMC best cost");
        outputScript.FinalLevelBestCost(bestCost);

        outputScript.CreateSection("Level Index");
        outputScript.printcurLevelInex(currentLevel);

        outputScript.CreateSection("Level Size");
        outputScript.printcurLevelSize(currentLevel.Count);
        outputScript.CreateSection("TargetDuration");
        outputScript.printFloat(targetDuration);
        outputScript.FinishOutput(levelFilename);

        Debug.Log("Finish output");

    }
    //void printEachJointLevelDistance()
    //{
    //    float[] sumdist = new float[helperScript.nDistJoint];
    //    //calculate sum of dist for each joint in a level

    //    for (int i = 0; i < currentLevel.Count - 1; i++)
    //    {
    //        int j = i + 1;
    //        int prechunk = currentLevel[i];
    //        int curentchunk = currentLevel[j];

    //        helperScript.DistDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
    //        for (int k = 0; k < helperScript.nDistJoint; k++)
    //        {
    //            sumdist[k] += helperScript.DistDataFile.Get_Float(k.ToString());

    //        }
    //    }
    //    outputScript.CreateSection("Sum of Distance for each joint");
    //    outputScript.printFloatArray(sumdist);
    //}
    void printEachJointLevelRotation()
    {
        float[] sumrot = new float[helperScript.nRotJoint];
        //calculate sum of dist for each joint in a level

        for (int i = 0; i < currentLevel.Count - 1; i++)
        {
            int j = i + 1;
            int prechunk = currentLevel[i];
            int curentchunk = currentLevel[j];

            helperScript.RotDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            for (int k = 0; k < helperScript.nRotJoint; k++)
            {
                sumrot[k] += helperScript.RotDataFile.Get_Float(k.ToString());
            }
        }
        outputScript.CreateSection("Sum of Rotation for each joint");
        outputScript.printFloatArray(sumrot);
    }
    void printSumCM()
    {
        float sumCM = 0f;
        for (int i = 0; i < currentLevel.Count - 1; i++)
        {
            int j = i + 1;
            //get previous chunk and current chunk
            int prechunk = currentLevel[i];
            int curentchunk = currentLevel[j];
            //Load Center of mass shifting between these two chunks
            helperScript.CMDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            sumCM += helperScript.CMDataFile.Get_Float("Center Shift");
        }
        outputScript.CreateSection("Sum of CM for a level");
        outputScript.printFloat(sumCM);
    }
    void clearEverything()
    {
        currentLevel.Clear();
        currentLevelIndex.Clear();
        chunkOccurrence = null;
        bestCost = 0f;
        bestValue = 0f;
        bestLength = 0;

    }


}


/*
  string s2 = "";
       for (int i = 0; i < currentLevel.Count; i++)
       {
           s2 += currentLevel[i] + " ";
       }
       Debug.Log("level before reverse " + s2);
*/
