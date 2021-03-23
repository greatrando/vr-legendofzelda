using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equippable;
using static HandTracking;

[RequireComponent(typeof(EquipmentMount))]
[RequireComponent(typeof(HandTracking))]
public class EquipmentGrabber : MonoBehaviour
{


    private const float TRIGGER_THRESHOLD_GRABBING = 0.55f;
    private const float TRIGGER_THRESHOLD_RELEASE = 0.35f;


    public Equipment Equipment = null;


	private bool _ableToGrab = true;
    private EquipmentMount mount;
    private HAND_STATE _state = HAND_STATE.OPEN;
    private bool isHandClosed = false;
    private List<Equippable> _touchingEquipables = new List<Equippable>();
    

    public void Start()
    {
        this.GetComponent<HandTracking>().OnHandStateChanged += OnHandStateChanged;
        mount = this.GetComponent<EquipmentMount>();
    }


    private void OnHandStateChanged(HandTracking sender)
    {
        HAND_STATE newState = sender.State;

        if (newState != _state)
        {
            bool oldHandClosed = isHandClosed;

            _state = newState;
            isHandClosed = sender.IsHandClosed;

            if (isHandClosed && !oldHandClosed && mount.Equippable == null)
            {
                GrabVolumeEnabled = false;
                CheckForEquippable();
            }
            else if (!isHandClosed)
            {
                Equipment.Unequip(this.GetComponent<EquipmentMount>());
                GrabVolumeEnabled = true;
            }
        }
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

                // if (equippable.EquippedBy != null)
                // {
                //     DebugHUD.GetInstance().PresentToast("already equipped by: " + equippable.EquippedBy.name);
                // }

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
