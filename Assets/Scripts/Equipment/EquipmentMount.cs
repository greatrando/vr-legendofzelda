using UnityEngine;


public class EquipmentMount : MonoBehaviour
{


    public bool IsStow;
    public Vector3 MountLocation = Vector3.zero;
    public Vector3 MountRotation = Vector3.zero;
    public Vector3 MountScale = Vector3.zero;


    private Equippable _equippable = null;


    public Equippable Equippable
    {
        get
        {
            return _equippable;
        }
        set
        {
            _equippable = value;
        }
    }


}
