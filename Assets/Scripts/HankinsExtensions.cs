using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class ClassExtension
{


    public static bool HasComponent<T>(this GameObject gameObject)
    {
        var item = gameObject.GetComponent<T>();
        bool isNull = (item == null) || (item.Equals(null));
        return !isNull;
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
                results.AddRange(go.GetAllChildren(includeDescendants));
            }
        }

        return results;
    }


    public static bool IsChildOfTag(this GameObject thisGameObject, List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (thisGameObject.IsChildOfTag(tag)) return true;
        }

        return false;
    }


    public static bool IsChildOfTag(this GameObject thisGameObject, string tag)
    {
        if (thisGameObject.HasComponent<Tags>() && thisGameObject.GetComponent<Tags>().HasTag(tag)) return true;

        if (thisGameObject.transform.parent == null) return false;

        return thisGameObject.transform.parent.gameObject.IsChildOfTag(tag);
    }


    public static bool IsChildOf(this GameObject thisGameObject, List<string> names)
    {
        foreach (string name in names)
        {
            if (thisGameObject.IsChildOf(name)) return true;
        }

        return false;
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


    public static GameObject FindParent(this GameObject thisGameObject, string name)
    {
        if (thisGameObject.name == name) return thisGameObject;

        if (thisGameObject.transform.parent == null) return null;

        return thisGameObject.transform.parent.gameObject.FindParent(name);
    }


    public static Rect BoardBounds(this Bounds bounds)
    {
        float halfX = bounds.size.x / 2.0f;
        float halfZ = bounds.size.z / 2.0f;

        Rect result = new Rect()
        {
            // xMin = bounds.center.x - halfX,
            // xMax = bounds.center.x + halfX,
            // yMin = bounds.center.z - halfZ,
            // yMax = bounds.center.z + halfZ
            xMin = -halfX,
            xMax = halfX,
            yMin = -halfZ,
            yMax = halfZ            
        };

        return result;
    }


}