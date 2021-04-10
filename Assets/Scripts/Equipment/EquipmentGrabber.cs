using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equippable;
using static HandTracking;

[RequireComponent(typeof(HandTracking))]
public class EquipmentGrabber : MonoBehaviour
{


    private const float TRIGGER_THRESHOLD_GRABBING = 0.55f;
    private const float TRIGGER_THRESHOLD_RELEASE = 0.35f;


    public Equipment Equipment = null;
    public EquipmentMount Mount = null;


	private bool _ableToGrab = true;
    private HAND_STATE _state = HAND_STATE.OPEN;
    private bool isHandClosed = false;
    private List<Equippable> _touchingEquipables = new List<Equippable>();
    

    public void Start()
    {
        this.GetComponent<HandTracking>().OnHandStateChanged += OnHandStateChanged;
    }


    private void OnHandStateChanged(HandTracking sender)
    {
        HAND_STATE newState = sender.State;

        if (newState != _state)
        {
            bool oldHandClosed = isHandClosed;

            _state = newState;
            isHandClosed = sender.IsHandClosed;
            // DebugHUD.GetInstance().PresentToast("New State: " + (isHandClosed ? "Closed" : "Open") + " was " + (oldHandClosed ? "Closed" : "Open"));

            if (isHandClosed && !oldHandClosed && Mount.Equippable == null)
            {
                GrabVolumeEnabled = false;
                CheckForEquippable();
            }
            else if (!isHandClosed)
            {
                Equipment.Unequip(Mount);
                GrabVolumeEnabled = true;
            }
        }
    }


    void OnTriggerEnter(Collider otherCollider)
    {
		Equippable equippable = otherCollider.GetComponent<Equippable>() ?? otherCollider.GetComponentInParent<Equippable>();
        if (equippable == null) return;

        // DebugHUD.GetInstance().PresentToast("Touching: " + equippable.name);
        if (!_touchingEquipables.Contains(equippable))
        {
            _touchingEquipables.Add(equippable);
        }
    }


    void OnTriggerExit(Collider otherCollider)
    {
		Equippable equippable = otherCollider.GetComponent<Equippable>() ?? otherCollider.GetComponentInParent<Equippable>();
        if (equippable == null) return;

        // DebugHUD.GetInstance().PresentToast("Not Touching: " + equippable.name);
        if (_touchingEquipables.Contains(equippable))
        {
            _touchingEquipables.Remove(equippable);
        }
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

                Vector3 directionToTarget = equippable.transform.position - this.transform.position;
                float distanceSquared = directionToTarget.sqrMagnitude;
                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestGrabbableEquippable = equippable;
                }
            }

            // DebugHUD.GetInstance().PresentToast("Do acquire!!");
            if (closestGrabbableEquippable != null)
            {
                Equipment.Acquire(Mount, closestGrabbableEquippable);
            }
        }
        catch (System.Exception ex)
        {
            DebugHUD.GetInstance().PresentToast(ex.Message);
            DebugHUD.GetInstance().PresentToast(ex.StackTrace);
        }
    }


    private bool GrabVolumeEnabled
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
