using System.Collections.Generic;
using UnityEngine;


public class CollisionHelper : MonoBehaviour
{


    public delegate void CollisionEnterEvent(CollisionHelper sender, GameObject gameObject);
    public delegate void CollisionExitEvent(CollisionHelper sender, GameObject gameObject);


    public List<GameObject> IgnoreObjects { get; set; }
    public List<GameObject> CurrentCollisions { get; private set; }


    public CollisionEnterEvent OnEnter = null;
    public CollisionExitEvent OnExit = null;


    public CollisionHelper()
    {
        IgnoreObjects = new List<GameObject>();
        CurrentCollisions = new List<GameObject>();
    }


    void Start()
    {
    }

     
    void OnCollisionEnter(Collision col) 
    {
        if (IgnoreObjects.Contains(col.gameObject)) return;

        CurrentCollisions.Add(col.gameObject);
        OnEnter?.Invoke(this, col.gameObject);
    }


    void OnCollisionExit(Collision col) 
    {
        if (IgnoreObjects.Contains(col.gameObject)) return;

        CurrentCollisions.Remove(col.gameObject);
        OnExit?.Invoke(this, col.gameObject);
    }

     
    void OnTriggerEnter(Collider col) 
    {
        if (IgnoreObjects.Contains(col.gameObject)) return;

        CurrentCollisions.Add(col.gameObject);
        OnEnter?.Invoke(this, col.gameObject);
    }


    void OnTriggerExit(Collider col) 
    {
        if (IgnoreObjects.Contains(col.gameObject)) return;

        CurrentCollisions.Remove(col.gameObject);
        OnExit?.Invoke(this, col.gameObject);
    }


}
