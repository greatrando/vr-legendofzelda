using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class ClassExtension
{


    public static List<GameObject> GetAllChildren(this GameObject gameObject)
    {
        List<GameObject> results = new List<GameObject>();

        for (int idx = 0; idx < gameObject.transform.childCount; idx++)
        {
            results.Add(gameObject.transform.GetChild(idx).gameObject);
        }

        return results;
    }


}