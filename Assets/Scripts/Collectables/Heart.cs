using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
 

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.IsChildOf("OVRPlayerController"))
        {
            Destroy(this.gameObject);
            Player.GetInstance().HealthSystem.Health += 1;
        }
    }


}
