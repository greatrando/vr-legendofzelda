using System.Collections.Generic;
using UnityEngine;


[SerializeField()]
public class GoodieDropper : MonoBehaviour
{


    public List<GameObject> Goodies = new List<GameObject>();
    public List<int> PercentChances = new List<int>();
    public int TotalDroppings = 1;


    private int _amountDropped = 0;


    public void Drop()
    {
        if (_amountDropped >= TotalDroppings) return;
        _amountDropped++;


        int max = PercentChances.Count > Goodies.Count ? PercentChances.Count : Goodies.Count;
        int rand = Random.Range(0, 100);
        int totalMaxPercent = 0;

        for (int idx = 0; idx < max; idx++)
        {
            totalMaxPercent += PercentChances[idx];
            
            if (rand < totalMaxPercent)
            {
                if (Goodies[idx] == null)
                {   //not dropping anything
                    return;
                }

                // UnityEngine.Debug.Log("Spawn: " + Goodies[idx].gameObject.name);
                GameObject clone = Instantiate(Goodies[idx], this.transform.position, this.transform.rotation);
                // UnityEngine.Debug.Log("Parented under: " + this.transform.parent.name);
                clone.transform.SetParent(this.transform.parent);
                // clone.transform.localScale = Goodies[idx].transform.localScale;
                clone.SetActive(true);
                return;
            }
        }
    }


}
