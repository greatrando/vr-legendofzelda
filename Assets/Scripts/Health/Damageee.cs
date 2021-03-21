using UnityEngine;


public class Damageee : MonoBehaviour
{


    public delegate void ApplyDamageEvent(GameObject sender);


    public ApplyDamageEvent OnApplyDamage = null;


}
