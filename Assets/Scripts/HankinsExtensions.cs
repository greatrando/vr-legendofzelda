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


    public static bool IsChildOf(this GameObject thisGameObject, string name)
    {
        if (thisGameObject.name == name) return true;

        if (thisGameObject.transform.parent == null) return false;

        return thisGameObject.transform.parent.gameObject.IsChildOf(name);
    }


    public static bool IsChild(this GameObject thisGameObject, GameObject gameObject)
    {
        List<GameObject> children = thisGameObject.GetAllChildren();

        foreach (GameObject go in children)
        {
            if (go == gameObject)
            {
                return true;
            }
        }

        return false;
    }


}