using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

     
    void OnCollisionEnter(Collision col) 
    {
        if (col.gameObject.GetComponent<Damageor>() != null && !col.gameObject.IsChildOf("XR Rig"))
        {
            this.GetComponent<AudioSource>().Play();

            if (this.gameObject.IsChildOf("LeftHand Controller"))
            {
                Player.GetInstance().GetComponent<Haptics>().Play(Haptics.HAND.Left, Haptics.VIBRATION_FORCE.Medium, 0.2f);
            }
            else
            {
                Player.GetInstance().GetComponent<Haptics>().Play(Haptics.HAND.Right, Haptics.VIBRATION_FORCE.Medium, 0.2f);
            }
        }
    }


}
