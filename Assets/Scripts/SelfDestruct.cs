using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{


    public float CountDown = 4f;


    private float _countDown = float.MaxValue;


    void Start()
    {
        _countDown = CountDown;
    }


    void FixedUpdate()
    {
        _countDown -= Time.fixedDeltaTime;

        if (_countDown <= 0)
        {
            UnityEngine.Debug.Log("self destruct");
            Destroy(this.gameObject);
        }    
    }


}
