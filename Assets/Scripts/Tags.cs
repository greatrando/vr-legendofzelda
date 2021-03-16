using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{


    public List<string> TagList = new List<string>();


    public bool HasTag(string tag)
    {
        return TagList.Contains(tag);
    }


}
