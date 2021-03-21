using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rupie : MonoBehaviour
{


    public int Value = 1;


    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.IsChildOf("OVRPlayerController"))
        {
            Player.GetInstance().GetComponent<Wallet>().CurrentValue += Value;
            Destroy(this.gameObject);
        }
    }


}
