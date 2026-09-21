using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MCMC : MonoBehaviour {
    enum Selection { Add, Remove, Modify };
    Selection curSelection = Selection.Add;
    public int levelSize = 10;
    int PrefabSize = 10; //size of prefab;  counting the origin pose
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
        public int rejectCounter, rejectTotal;

        [Header("Weight:  Adajacent, Duration")]
        public float adVarWeight = 0.3f;
        public float DurationWeight = 0.3f;

        [Header("Fixed part")]
        public bool fixedPartEnable = false;
        public int[] fixedSequence = { 1, 6, 1, 6, 1 };
        [Header("Other Section/Output")]
        public string levelFilename = "easy";
        //public float CurrentCal = 0f;
        public float currentLevelDistCost = 0f;
        public float currentLevelRotCost = 0f;
        public float currentLevelCMCost = 0f;
        public float CurrentDur = 0f;
        public float CurrentAdjVar = 0f;
        public float CurrentDurCost = 0f;
        public float CurrentAdjVarCost = 0f;
        bool GameInitial = false;

        private DateTime start;
        DateTime end;
        public AnimationCurve curve;

       
        public int levelcounter = 0;
        // Use this for initialization
        void Start()
        {
            helperScript = RJMCMCHelper.Instance;
            outputScript = Output.Instance;
            if (helperScript == null) { Debug.LogError("Cannot find optimization helper Script!!"); }
            GameInitial = false;
            //one array store the object; one array store Rnd Pick #;
            currentLevel = new List<int>();
            currentLevelIndex = new List<int>();
            chunkOccurrence = new int[PrefabSize];


          

          //  Iteration = TargetIteration;
            temperature = MaxTemperature;
           curSelection = Selection.Modify;


            //Add  a chunk; 
            //We fix the first chunk to be type 0
            //!!
            int index = 0;
            chunkOccurrence[index] += 1;
            oldchunk = index;
            oldIndex = 0;
            currentLevel.Add(oldchunk);
            currentLevelIndex.Add(oldIndex);
            

            for (int i = 1; i < levelSize; i++)
            {
                int newchunk = Rnd();
                currentLevel.Add(newchunk); // add firxed sequece chunk to current level
                currentLevelIndex.Add(newchunk);
                chunkOccurrence[newchunk] += 1;
               
            }
            //print(curSelection + "  " + currentLevel.Count + levelFilename);


            bestCost = Mathf.Abs(CalculateCost());
            // print(Mathf.Abs(CalculateCost()));
            bestValue = UseFormula(bestCost);

            //float possibleDuration = W * 10f; //Max possible duration 
            //if (targetDuration > possibleDuration) { Debug.Log("target duration value is too large!"); targetDuration = possibleDuration; } //if  target duration is greater than possible duration
            curve = new AnimationCurve();

            begin = true;
           start = DateTime.Now;
        }


        //optimazation
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
            //if (Iteration <= 0 && !GameInitial)
            if ((rejectCounter >= rejectTotal) && !GameInitial)
            {
                end = DateTime.Now;
                begin = false;
                GameInitial = true;
                CalculateCost();

                PrintRJMCMCResult();
                clearEverything();
                levelcounter++;

                this.enabled = false;


            }
            else
            {
                //Iteration--;
                Iteration++;
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
                        rejectCounter = 0;
                        // Debug.Log("Accept "+currentLevel.Count );
                        //  print("accept " + curSelection);

                        //

                        bestCost = curCost;
                        bestValue = newValue;
                      //bestLength = currentLevel.Count;
                        //   Debug.Log("Accept" + bestCost );
                    }
                    else
                    {
                        //Debug.Log("Reject");
                        rejectCounter++;
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
                        rejectCounter = 0;
                        bestCost = curCost;
                        bestValue = newValue;
                        //bestLength = currentLevel.Count;
                        //   Debug.Log("Accept" + bestCost );
                    }
                    else
                    {
                        // Debug.Log("Reject");
                        rejectCounter++;
                        Reverse();

                        // print("reverse " + s);
                    }
                }



                //make temp drops faster
                //temperature = ((float)Iteration / TargetIteration) * MaxTemperature + 0.001f;
                if ((targetAdVar - Iteration) % 1000 == 0)
                {
                //curve.AddKey((1f - 1f * Iteration / TargetIteration), bestCost);
                curve.AddKey((1.0f * Iteration / TargetIteration), bestCost);
                }
                temperature = ((0.67f * TargetIteration - (float)Iteration) / (TargetIteration * 0.67f)) * MaxTemperature + 0.001f;
              
                temperature = Mathf.Max(temperature, 0.001f);

            }
        }


        private float CalculateCost()
        {
            float cost = 0f;
            CurrentDurCost = CalculateChunkDurationCost();
            cost += CurrentDurCost * DurationWeight;
            CurrentAdjVarCost = CalculateAdVariationCost();
            cost += CurrentAdjVarCost * adVarWeight;

         
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
            float sigma = targetDuration * 0.5f;

            levelduration = currentLevel.Count * 3f;
            CurrentDur = levelduration; //update in inspector

            float cost = (levelduration - targetDuration) * (levelduration - targetDuration);
            cost = -1f * cost / (2f * sigma * sigma);
            cost = 1f - Mathf.Exp(cost);
            //  print("totaltime" + c + "  durationC " + answer +"  best  cost   "+bestCost + "iteration" + Iteration);
            return cost;
        }
        //random function 1~prefabsize
        int Rnd()
        {
            return UnityEngine.Random.Range(1, PrefabSize);
        }

        void SelectMethod()
        {
            float select = 0.5f;
            ////print("select " + select);
            //if (currentLevel.Count <= 1)
            //{  //cannot remove the level if it is too short
            //    curSelection = Selection.Add;

            //    Add();
            //    return;

            //}
            //add a wave; 
            if (select <= 0.4f)
            {
                curSelection = Selection.Add;
                //Add();

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
               // Remove();
                //   Debug.Log(" Action: REMOVE " + Iteration);
                //remove a wave; 

            }

        }

    float UpdateAcceptance()
    {
        float a1 =0f;
        if (curSelection == Selection.Modify) //replace
        {
            a1 = Mathf.Min(newValue / bestValue, 1.0f);
            //Debug.Log(" acceptance rate modify ");
            //      Debug.Log("replace" + "acceptance rate " + a1);

        }
        return a1;
    }
        //else if (curSelection == Selection.Remove) //remove
        //{
        //    //  Debug.Log(" acceptance rate remove ");
        //    //Debug.Log("remove" + "bestlength"+ bestLength + "cur length" + currentLevel.Count);
        //    float v1 = pa / pr;
        //    float v2 = newValue / bestValue;
        //    float v3 = (bestLength * 1.0f) / (W - currentLevel.Count);
        //    float a2 = v1 * v2 * v3;
        //    //    Debug.Log("v1  " + v1 + "  v2  " + v2 + "  v3  " + v3);
        //    a2 = Mathf.Min(1, a2);
        //    return a2;

        //}
        //else //if (method.Equals("add"))
        //{
        //    float v1 = pr / pa;
        //    float v2 = newValue / bestValue;
        //    float v3 = ((W - bestLength) * 1.0f / currentLevel.Count);
        //    //  Debug.Log("add" + "bestlength" + bestLength + "cur length" + currentLevel.Count);
        //    float a3 = v1 * v2 * v3;
        //    a3 = Mathf.Min(a3, 1);

        //    // Debug.Log(" acceptance rate add ");
        //    //   Debug.Log("add"+ "bestlength" + bestLength + "cur length" + currentLevel.Count);
        //    return a3;
        //}

    
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
                        print(currentLevel[i + j] + " " + fixedSequence[j]);
                        counter++;
                    }
                    else
                    {
                        counter = 0;
                    }
                }
                if (counter == fixedSequence.Length)
                {
                    print("Find it");
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

        //void Add()
        //{ //randomly add one wave 

        //    int index = Rnd();
        //    while (chunkOccurrence[index] >= Wi)
        //    {
        //        index = Rnd();
        //    }

        //    chunkOccurrence[index] += 1;

        //    oldchunk = index;
        //    //choose one place from level to add
        //    //we fixed the first one
        //    oldIndex = UnityEngine.Random.Range(1, currentLevel.Count);

        //    currentLevel.Insert(oldIndex, oldchunk);
        //    currentLevelIndex.Insert(oldIndex, index);

        //}

        //void Remove()
        //{
        //    if (currentLevel.Count == 0)
        //    {
        //        // Debug.LogError("cannot remove");
        //        return;
        //    }

        //    //choose one num as level index from level to remove
        //    //we fixed the first one 
        //    oldIndex = UnityEngine.Random.Range(1, currentLevel.Count);

        //    oldchunk = currentLevel[oldIndex];
        //    oldIndexNo = currentLevelIndex[oldIndex];
        //    if (chunkOccurrence[oldIndexNo] > 0)
        //    {
        //        chunkOccurrence[oldIndexNo] -= 1;
        //    }
        //    currentLevel.RemoveAt(oldIndex);
        //    //print("removing" );
        //    currentLevelIndex.RemoveAt(oldIndex);
        //}


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
            //else if (curSelection == Selection.Remove)
            //{  //revert remove method

            //    currentLevel.Insert(oldIndex, oldchunk); //add the wave back 
            //    currentLevelIndex.Insert(oldIndex, oldIndexNo);
            //    chunkOccurrence[oldIndexNo] += 1;

            //}
            //else
            //{  //reveret add method

            //    if (chunkOccurrence[currentLevelIndex[oldIndex]] > 0) //decrease corresponding waves number
            //    {
            //        chunkOccurrence[currentLevelIndex[oldIndex]] -= 1;
            //    }

            //    currentLevel.RemoveAt(oldIndex); //remove the new wave and corresponding wave number
            //    currentLevelIndex.RemoveAt(oldIndex);

            //}


        }

        void PrintRJMCMCResult()
        {
            outputScript.InitializeInifile();
          
             double d = (end - start).TotalMinutes;
               double s = (end - start).TotalSeconds;

            string minute = d.ToString("g");
            string second = s.ToString("g");

        outputScript.CreateSection("Time");
        outputScript.printString("startTime",start.ToString("G"));
        outputScript.printString("endTime", end.ToString("G"));
        outputScript.printString("minute", minute);
        outputScript.printString("second", second);
        outputScript.CreateSection("Total Iteration");
        outputScript.printFloat(Iteration);

        printEachJointLevelRotation();
            printSumCM();

            
            outputScript.CreateSection("ImportanceRotJoint");
            outputScript.PrintEntireFloatArray(helperScript.ImportantJointRot, "ImportanceRotJoint");


            outputScript.CreateSection("MCMC Prior targets");
            outputScript.printLevelTargets(targetAdVar, targetDuration);
        
            outputScript.CreateSection("MCMC Rot targets");
            outputScript.printFloatArray(helperScript.targetRot);
            outputScript.CreateSection("MCMC CM targets");
            outputScript.printFloat(helperScript.targetCM);


            outputScript.CreateSection("LevelCost");
            outputScript.printCurrentLevelCost(CurrentAdjVar, CurrentDur, CurrentAdjVarCost, CurrentDurCost,
                CurrentAdjVarCost * adVarWeight, CurrentDurCost * DurationWeight,
                        currentLevelRotCost, currentLevelCMCost,
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
          
            outputScript.FinishOutput(levelFilename);
          
          
            Debug.Log("Finish output");

        }
      


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
          

        }


    }


   

