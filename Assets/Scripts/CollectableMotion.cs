using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollectableMotion : MonoBehaviour
{


    public float LifeTimeSeconds;
    public float RotationSpeed = 150;

    public float BottomHeight = 1;
    public float height = 1;
    public float speed = 1;


    private float _currentLifeTime = 0;


    void FixedUpdate()
    {
        Rotate();
        Bob();
        Expire();
    }


    private void Rotate()
    {
        transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * RotationSpeed);
    }


    private void Bob()
    {
        Vector3 pos = transform.position;
        float newY = Mathf.Sin(Time.time * speed);
        transform.position = new Vector3(0, BottomHeight, 0) + (new Vector3(pos.x, newY * height, pos.z));
    }


    private void Expire()
    {
        _currentLifeTime += Time.fixedDeltaTime;
        if (_currentLifeTime >= LifeTimeSeconds)
        {
            Destroy(this.gameObject);
        }
    }


}
