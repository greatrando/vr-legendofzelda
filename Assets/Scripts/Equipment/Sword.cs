using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{


    public void Start()
    {
        Damageor damageor = this.GetComponent<Damageor>();
        damageor.OnDamaging += DamagingEvent;
    }


    private void DamagingEvent(GameObject damageor, GameObject damageee)
    {
        if (!damageee.IsChildOf("XR Rig"))
        {
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
