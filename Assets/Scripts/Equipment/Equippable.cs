using UnityEngine;


public class Equippable : MonoBehaviour
{


    public enum EQUIPPABLE_STATE
    {
        DROPPED,
        UNEQUIPPED,
        EQUIPPED
    }


    public delegate void EquippedEvent(Equippable sender);
    public delegate void UnEquippedEvent(Equippable sender);
    public delegate void DroppedEvent(Equippable sender);


    public EquippedEvent OnEquipped = null;
    public UnEquippedEvent OnUnEquipped = null;
    public DroppedEvent OnDropped = null;


    public string EquipableName = "";
    public bool IsBaggable = true;
    public bool Droppable = false;

    public bool StowedVisible;
    public Vector3 StowedLocation = Vector3.zero;
    public Vector3 StowedRotation = Vector3.zero;
    public Vector3 StowedScale = Vector3.one;

    public bool EquippedVisible;
    public Vector3 EquippedLocation = Vector3.zero;
    public Vector3 EquippedRotation = Vector3.zero;
    public Vector3 EquippedScale = Vector3.one;


    private EQUIPPABLE_STATE _state = EQUIPPABLE_STATE.DROPPED;
    private EquipmentMount _equippedBy = null;


    public void Equipped(EquipmentMount mount)
    {
        UnityEngine.Debug.Log("do equipped");
        _equippedBy = mount;
        OnEquipped?.Invoke(this);
    }


    public void UnEquipped()
    {
        UnityEngine.Debug.Log("do unequipped");
        OnUnEquipped?.Invoke(this);
    }


    public void Dropped()
    {
        UnityEngine.Debug.Log("do drop... gravity etc.");
        OnDropped?.Invoke(this);
    }


    public EQUIPPABLE_STATE State
    {
        get
        {
            return _state;
        }
    }


    public EquipmentMount EquippedBy
    {
        get
        {
            return _equippedBy;
        }
    }

}
