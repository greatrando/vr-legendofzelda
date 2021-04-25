using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Heart : MonoBehaviour
{
 

    private bool _collected = false;


    public void OnTriggerEnter(Collider collider)
    {
        if (!_collected && collider.gameObject.IsChildOf("XR Rig"))
        {
            _collected = true;
            Player.GetInstance().PlayAudio(this.GetComponent<AudioSource>());
            Destroy(this.gameObject);
            Player.GetInstance().HealthSystem.Health += 1;
        }
    }


}
