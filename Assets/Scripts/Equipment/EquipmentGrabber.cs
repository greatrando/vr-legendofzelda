using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equippable;


[RequireComponent(typeof(EquipmentMount))]
public class EquipmentGrabber : MonoBehaviour
{


    private const float TRIGGER_THRESHOLD_GRABBING = 0.55f;
    private const float TRIGGER_THRESHOLD_RELEASE = 0.35f;


    public Equipment Equipment = null;


    // OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
    [SerializeField]
    protected OVRInput.Controller m_controller;


	private bool _ableToGrab = true;
    private float _lastTrigger = 0;
    private List<Equippable> _touchingEquipables = new List<Equippable>();
    

    public void Start()
    {
		// We're going to setup the player collision to ignore the hand collision.
		SetPlayerIgnoreCollision(gameObject, true);
    }


    public void Update()
    {
        CheckForGrabOrRelease();
    }


    void OnTriggerEnter(Collider otherCollider)
    {
		Equippable equippable = otherCollider.GetComponent<Equippable>() ?? otherCollider.GetComponentInParent<Equippable>();
        if (equippable == null) return;

        if (!_touchingEquipables.Contains(equippable))
        {
            _touchingEquipables.Add(equippable);
        }
    }


    void OnTriggerExit(Collider otherCollider)
    {
		Equippable equippable = otherCollider.GetComponent<Equippable>() ?? otherCollider.GetComponentInParent<Equippable>();
        if (equippable == null) return;

        if (_touchingEquipables.Contains(equippable))
        {
            _touchingEquipables.Remove(equippable);
        }
    }


	protected void SetPlayerIgnoreCollision(GameObject grabbable, bool ignore)
	{
		// if (m_player != null)
		// {
		// 	Collider[] playerColliders = m_player.GetComponentsInChildren<Collider>();
		// 	foreach (Collider pc in playerColliders)
		// 	{
		// 		Collider[] colliders = grabbable.GetComponentsInChildren<Collider>();
		// 		foreach (Collider c in colliders)
		// 		{
        //             if(!c.isTrigger && !pc.isTrigger)
		// 			    Physics.IgnoreCollision(c, pc, ignore);
		// 		}
		// 	}
		// }
	}


    protected void CheckForGrabOrRelease()
    {
        float currentTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

        if ((currentTrigger >= TRIGGER_THRESHOLD_GRABBING) && (_lastTrigger < TRIGGER_THRESHOLD_GRABBING))
        {
            GrabVolumeEnable = false;
            CheckForEquippable();
        }
        else if ((currentTrigger <= TRIGGER_THRESHOLD_RELEASE) && (_lastTrigger > TRIGGER_THRESHOLD_RELEASE))
        {
            Equipment.Unequip(this.GetComponent<EquipmentMount>());
            GrabVolumeEnable = true;
        }
        
        _lastTrigger = currentTrigger;
    }


    private void CheckForEquippable()
    {
        try
        {
            float closestDistanceSquared = float.MaxValue;
            Equippable closestGrabbableEquippable = null;

            // Iterate grab candidates and find the closest grabbable candidate
            foreach (Equippable equippable in _touchingEquipables)
            {
                bool canGrab = (equippable.State == EQUIPPABLE_STATE.DROPPED || equippable.EquippedBy != this);
                if (!canGrab)
                {
                    continue;
                }

                if (equippable.EquippedBy != null)
                {
                    DebugHUD.GetInstance().PresentToast("already equipped by: " + equippable.EquippedBy.name);
                }

                Vector3 directionToTarget = equippable.transform.position - this.transform.position;
                float distanceSquared = directionToTarget.sqrMagnitude;
                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestGrabbableEquippable = equippable;
                }
            }

            if (closestGrabbableEquippable != null)
            {
                Equipment.Acquire(this.gameObject, closestGrabbableEquippable);
            }
            else
            {
                DebugHUD.GetInstance().PresentToast("nothing to equip");            
            }
        }
        catch (System.Exception ex)
        {
            DebugHUD.GetInstance().PresentToast(ex.Message);
            DebugHUD.GetInstance().PresentToast(ex.StackTrace);
        }
    }


    private bool GrabVolumeEnable
    {
        get
        {
            return _ableToGrab;
        }
        set
        {
            if (_ableToGrab == value)
            {
                return;
            }

            _ableToGrab = value;
        }

    }


}
