using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BillboardToggle : MonoBehaviour
{


    private GameObject _text;
    private bool _isInitialized = false;


    void Start()
    {
    }


    void Update()
    {
        if (!_isInitialized)
        {
            _text = this.transform.Find("Text").gameObject;
            _text.SetActive(false);
            _text.GetComponent<MeshRenderer>().enabled = false;
            _isInitialized = true;            
        }        
    }

     
    void OnTriggerEnter(Collider col) 
    {
        if (_text == null) return;

        _text.SetActive(true);
        _text.GetComponent<MeshRenderer>().enabled = true;
    }


    void OnTriggerExit(Collider col) 
    {
        if (_text == null) return;
        
        _text.SetActive(false);
        _text.GetComponent<MeshRenderer>().enabled = false;
    }


}
