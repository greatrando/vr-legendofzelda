using System.Collections.Generic;
using UnityEngine;


public class CollisionHelper : MonoBehaviour
{


    public delegate void CollisionEnterEvent(CollisionHelper sender, GameObject gameObject, Vector3 point);
    public delegate void CollisionExitEvent(CollisionHelper sender, GameObject gameObject);


    public List<GameObject> IgnoreObjects { get; set; }
    public List<string> IgnoreNames { get; set; }
    public List<string> IgnoreTags { get; set; }
    public List<GameObject> CurrentCollisions { get; private set; }


    public CollisionEnterEvent OnEnter = null;
    public CollisionExitEvent OnExit = null;


    public CollisionHelper()
    {
        IgnoreObjects = new List<GameObject>();
        IgnoreNames = new List<string>();
        IgnoreTags = new List<string>();
        CurrentCollisions = new List<GameObject>();
    }


    void Start()
    {
    }

     
    void OnCollisionEnter(Collision col) 
    {
        if (
                col.gameObject == null || IgnoreObjects.Contains(col.gameObject) || 
                col.gameObject.IsChildOf(IgnoreNames) || col.gameObject.IsChildOfTag(IgnoreTags)
            ) 
        {
            return;
        }

        CurrentCollisions.Add(col.gameObject);
        OnEnter?.Invoke(this, col.gameObject, col.contacts[0].point);
    }


    void OnCollisionExit(Collision col) 
    {
        if (col.gameObject == null || IgnoreObjects.Contains(col.gameObject)) return;

        CurrentCollisions.Remove(col.gameObject);
        OnExit?.Invoke(this, col.gameObject);
    }

     
    void OnTriggerEnter(Collider col) 
    {
        if (col.gameObject == null || IgnoreObjects.Contains(col.gameObject)) return;

        Vector3 localVector = col.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        var localClosestPoint = transform.InverseTransformPoint(localVector);
        CurrentCollisions.Add(col.gameObject);
        OnEnter?.Invoke(this, col.gameObject, localClosestPoint);
    }


    void OnTriggerExit(Collider col) 
    {
        if (col.gameObject == null || IgnoreObjects.Contains(col.gameObject)) return;

        CurrentCollisions.Remove(col.gameObject);
        OnExit?.Invoke(this, col.gameObject);
    }


}
