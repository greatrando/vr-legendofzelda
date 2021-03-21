using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Damageor : MonoBehaviour
{


    public float DamageAmount = 0.5f;


    public delegate void DamagingEvent(GameObject damageor, GameObject damageee);


    public DamagingEvent OnDamaging = null;


    private CollisionHelper _collisionHelper;
    public List<GameObject> _ignoreObjects = new List<GameObject>();


    void Start()
    {
        _collisionHelper = this.GetComponent<CollisionHelper>();
        if (_collisionHelper == null)
        {
            _collisionHelper = this.gameObject.AddComponent<CollisionHelper>();
        }

        _collisionHelper.IgnoreObjects = _ignoreObjects;
        _collisionHelper.OnEnter += OnDamageCollision;
    }


    private void OnDamageCollision(CollisionHelper sender, GameObject gameObject)
    {
        // if (sender.name == "Rock" && gameObject.name != "Rock" && !gameObject.name.StartsWith("Rock") && !gameObject.name.StartsWith("Cube"))
        // {
        //     DebugHUD.FindDebugHud().PresentToast("Damage from " + sender.gameObject.name + " to " + gameObject.name);
        // }
        // if (this.gameObject.name == "Handle")
        // {
            // UnityEngine.Debug.Log("Collide with: " + gameObject.name);
        // }


        Damageee takeDamage = gameObject.GetComponent<Damageee>();
        if (takeDamage != null)
        {
            takeDamage.OnApplyDamage?.Invoke(sender.gameObject);
        }

        if (sender.gameObject == null) return;
        
        OnDamaging?.Invoke(sender.gameObject, gameObject);
    }


    public List<GameObject> IgnoreObjects
    {
        get
        {
            if (_collisionHelper == null)
            {
                return IgnoreObjects;
            }

            return _collisionHelper.IgnoreObjects;
        }
        set
        {
            if (_collisionHelper == null)
            {
                _ignoreObjects = value;
            }
            else
            {
                _collisionHelper.IgnoreObjects = value;
            }
        }
    }


}
