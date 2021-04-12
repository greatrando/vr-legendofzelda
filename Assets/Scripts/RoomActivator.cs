using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomActivator : MonoBehaviour
{


    private const string PLAYER_GAMEOBJECT_NAME = "XR Rig";


    private GameObject _room;
    private bool _inRoom = false;


    // Start is called before the first frame update
    void Start()
    {
        _room = this.gameObject.transform.GetChild(0).gameObject;
        _room.SetActive(false);    
    }


    public void OnTriggerEnter(Collider collider)
    {
        if (_inRoom || !collider.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME)) return;
        _inRoom = true;

        _room.SetActive(true);    
    }


    public void OnTriggerExit(Collider collider)
    {
        if (!_inRoom || !collider.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME)) return;
        _inRoom = false;
        
        _room.SetActive(false);    
    }



}
