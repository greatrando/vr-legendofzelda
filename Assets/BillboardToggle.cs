using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BillboardToggle : MonoBehaviour
{


    private GameObject _text;


    // Start is called before the first frame update
    void Start()
    {
        _text = this.transform.Find("Text").gameObject;
        _text.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

     
    void OnTriggerEnter(Collider col) 
    {
        _text.SetActive(true);
    }


    void OnTriggerExit(Collider col) 
    {
        _text.SetActive(false);
    }


}
