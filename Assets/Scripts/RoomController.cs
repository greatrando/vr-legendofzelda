using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RoomController : MonoBehaviour
{


    private const string PLAYER_GAMEOBJECT_NAME = "XR Rig";


    public GameObject Spawning;


    private List<GameObject> _spawnable = new List<GameObject>();
    private List<GameObject> _activatable = new List<GameObject>();
    private List<GameObject> _activated = new List<GameObject>();
    private bool _inRoom = false;


    public void Start()
    {
        foreach (GameObject go in this.gameObject.GetAllChildren(true).Where(go => go.GetComponent<Tags>() != null && go.GetComponent<Tags>().HasTag("spawnable")))
        {
            // if (go.activeInHierarchy)
            {
                go.SetActive(false);
            }
            _spawnable.Add(go);
        }
        foreach (GameObject go in this.gameObject.GetAllChildren(true).Where(go => go.GetComponent<Tags>() != null && go.GetComponent<Tags>().HasTag("activatable")))
        {
            // if (go.activeInHierarchy)
            {
                go.SetActive(false);
            }
            _activatable.Add(go);
        }
    }


    public void OnTriggerEnter(Collider collider)
    {
        if (_inRoom || !collider.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME))
        {
            return;
        }

        if (collider.gameObject.name.StartsWith("GrabVolume")) return; //not sure why this is firing horribly when in VR

        // DebugHUD.FindDebugHud().PresentToast("in room " + collider.gameObject.name);

        _inRoom = true;

        StartCoroutine("SpawnAndActivate");
    }


    IEnumerator SpawnAndActivate()
    {
        List<GameObject> activators = new List<GameObject>();

        foreach (GameObject go in _activatable)
        {
            if (!go.activeInHierarchy)
            {
                go.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject go in _spawnable)
        {
            GameObject clone;
            
            if (Spawning != null)
            {
                clone = Instantiate(Spawning, go.transform.position, go.transform.rotation);
                clone.transform.SetParent(this.transform);
                // clone.transform.localScale = go.transform.localScale;
                clone.SetActive(true);
                activators.Add(clone);
            }

            clone = Instantiate(go, go.transform.position, go.transform.rotation);
            clone.name = go.name;
            clone.transform.SetParent(this.transform);
            clone.transform.localScale = go.transform.localScale;
            clone.SetActive(true);
            _activated.Add(clone);
        }

        yield return new WaitForSeconds(1);


        foreach (GameObject go in activators)
        {
            Destroy(go);
        }
        activators.Clear();
    }


    public void OnTriggerExit(Collider collider)
    {
        if (!_inRoom || !collider.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME)) return;

        if (collider.gameObject.name.StartsWith("GrabVolume")) return; //not sure why this is firing horribly when in VR

        if (!collider.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME))
        {
            CheckForChildrenReflect(collider.gameObject);
            return;
        }

        // DebugHUD.FindDebugHud().PresentToast("out of room " + collider.gameObject.name);

        _inRoom = false;

        foreach (GameObject go in _activatable)
        {
            if (go.activeInHierarchy)
            {
                go.SetActive(false);
            }
        }

        foreach (GameObject go in _activated)
        {
            Destroy(go);
        }
        _activated.Clear();
    }


    private void CheckForChildrenReflect(GameObject gameObject)
    {
        Tags tags = gameObject.GetComponent<Tags>();
        if (tags == null) return;

        if (!this.gameObject.IsChild(gameObject)) return;

        //flip it
        Vector3 currentRotation = gameObject.transform.eulerAngles;
        currentRotation.y += 180;
        gameObject.transform.eulerAngles = currentRotation; 
        
        //and reverse it
        gameObject.GetComponent<Rigidbody>().velocity *= -1;
    }


}
