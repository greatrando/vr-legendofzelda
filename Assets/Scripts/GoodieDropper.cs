using System.Collections.Generic;
using UnityEngine;


[SerializeField()]
public class GoodieDropper : MonoBehaviour
{


    public List<GameObject> Goodies = new List<GameObject>();
    public List<int> PercentChances = new List<int>();


    public void Drop()
    {
        int max = PercentChances.Count > Goodies.Count ? PercentChances.Count : Goodies.Count;

        for (int idx = 0; idx < max; idx++)
        {
            if (Random.Range(0, 100) > PercentChances[idx])
            {
                UnityEngine.Debug.Log("Spawn: " + Goodies[idx].gameObject.name);
                GameObject clone = Instantiate(Goodies[idx], this.transform.position, this.transform.rotation);
                UnityEngine.Debug.Log("Parented under: " + this.transform.parent.name);
                clone.transform.SetParent(this.transform.parent);
                // clone.transform.localScale = Goodies[idx].transform.localScale;
                clone.SetActive(true);
                return;
            }
        }
    }


}
