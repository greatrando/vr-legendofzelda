using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExitButton : MonoBehaviour
{


    private const string PLAYER_GAMEOBJECT_NAME = "XR Rig";


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    Pressed();
                }
            }
        }        
    }


    void OnCollisionEnter(Collision col) 
    {
        if (!col.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME)) return;

        Pressed();
    }


    private void Pressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


}
