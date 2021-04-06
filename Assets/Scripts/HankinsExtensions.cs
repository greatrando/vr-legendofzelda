using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class ClassExtension
{


    public static bool HasComponent<T>(this GameObject gameObject)
    {
        return (gameObject.GetComponent<T>() != null);
    }


    public static List<GameObject> GetAllChildren(this GameObject gameObject)
    {
        return GetAllChildren(gameObject, false);
    }


    public static List<GameObject> GetAllChildren(this GameObject gameObject, bool includeDescendants)
    {
        List<GameObject> results = new List<GameObject>();

        for (int idx = 0; idx < gameObject.transform.childCount; idx++)
        {
            GameObject go = gameObject.transform.GetChild(idx).gameObject;
            if (go.HasComponent<Tags>())
            {
                if (go.GetComponent<Tags>().HasTag("room")) continue;
            }

            results.Add(go);

            if (includeDescendants)
            {
                results.AddRange(GetAllChildren(go, includeDescendants));
            }
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