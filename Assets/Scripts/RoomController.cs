using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RoomController : MonoBehaviour
{


    private const string PLAYER_GAMEOBJECT_NAME = "OVRPlayerController";


    public GameObject Activating;


    private List<GameObject> _activatable = new List<GameObject>();
    private List<GameObject> _activated = new List<GameObject>();
    private bool _inRoom = false;


    public void Start()
    {
        foreach (GameObject go in this.gameObject.GetAllChildren().Where(go => go.GetComponent<Tags>() != null && go.GetComponent<Tags>().HasTag("activatable")))
        {
            go.SetActive(false);
            _activatable.Add(go);
        }
    }


    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name != PLAYER_GAMEOBJECT_NAME)
        {
            return;
        }

        _inRoom = true;

        StartCoroutine("SpawnActivatables");
    }


    IEnumerator SpawnActivatables()
    {
        List<GameObject> activators = new List<GameObject>();

        foreach (GameObject go in _activatable)
        {
            GameObject clone;
            
            if (Activating != null)
            {
                clone = Instantiate(Activating, go.transform.position, go.transform.rotation);
                clone.transform.SetParent(this.transform);
                clone.transform.localScale = go.transform.localScale;
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

        yield return new WaitForSeconds(5);


        foreach (GameObject go in activators)
        {
            Destroy(go);
        }
        activators.Clear();
    }


    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name != PLAYER_GAMEOBJECT_NAME)
        {
            return;
        }

        _inRoom = false;

        foreach (GameObject go in _activated)
        {
            Destroy(go);
        }
        _activated.Clear();
    }


}
