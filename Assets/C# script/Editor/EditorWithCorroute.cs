using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Allow us to run the corroute in Editor,
 * This is the simply version, do not accept the nested corroute struction, 
 * require a specially designed instruction to do so.
 * 
 * 
 */
public class EditorWithCorroute : Editor {

    // corroute list 
    static Dictionary<string,IEnumerator> corrouteDict = new Dictionary<string, IEnumerator>();
    // waiting for deleteing, which is done, by the given ref name.
    static List<string> dels = new List<string>();

    static bool isAwaked = false;

    private void Awake()
    {
       
            ActivateCorroutFunction();
       
    }

    public void ActivateCorroutFunction()
    {
        if (!isAwaked)
        {
            isAwaked = true;

            // update function
            EditorApplication.update -= UpdateFunc;
            EditorApplication.update += UpdateFunc;
            Debug.Log("<b>Editor corroute function online.</b>");
        }
    }
   

    public void StartEditorCorroute(IEnumerator route, string refName)
    {
        corrouteDict.Remove(refName);
        corrouteDict.Add(refName,route);
    }

    public void StopEditorCorroute(string refName)
    {
        corrouteDict.Remove(refName);
    }

    public void StopAllEditorCorroute()
    {
        corrouteDict.Clear();
    }
    
    // editor update func
    static void UpdateFunc()
    {
        // run all the corroutes;
        foreach (KeyValuePair<string, IEnumerator> pair 
            in corrouteDict)
        {
            if (!pair.Value.MoveNext())
            {
                dels.Add(pair.Key);
            }
        }

        // del the finished corroutes
        for (int i = 0; i < dels.Count; i++)
        {

            corrouteDict.Remove(dels[i]);
        }
        dels.Clear();

    }
}
