using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rupie : MonoBehaviour
{


    public int Value = 1;


    private bool _collected = false;


    public void OnTriggerEnter(Collider collider)
    {
        if (!_collected && collider.gameObject.IsChildOf("XR Rig"))
        {
            _collected = true;
            Player.GetInstance().PlayAudio(this.GetComponent<AudioSource>(), Value);
            Destroy(this.gameObject);
            Player.GetInstance().GetComponent<Wallet>().CurrentValue += Value;
        }
    }


}
