using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rupie : MonoBehaviour
{


    public int Value = 1;


    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.IsChildOf("XR Rig"))
        {
            Player.GetInstance().PlayAudio(this.GetComponent<AudioSource>(), Value);
            Destroy(this.gameObject);
            Player.GetInstance().GetComponent<Wallet>().CurrentValue += Value;
        }
    }


}
