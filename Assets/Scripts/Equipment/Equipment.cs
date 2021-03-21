using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Equipment : MonoBehaviour
{


    public int MaxBagCount = 0;
    public List<EquipmentMount> StowedMountables = new List<EquipmentMount>();
    public List<EquipmentMount> EquippedMountables = new List<EquipmentMount>();


    private List<Equippable> _equipables = new List<Equippable>();
    private List<EquipmentMount> _mounts = new List<EquipmentMount>();
    private List<Equippable> _bag = new List<Equippable>();

    

    public void Start()
    {
        foreach (EquipmentMount mount in EquippedMountables)
        {
            _mounts.Add(mount);
        }
        foreach (EquipmentMount mount in StowedMountables)
        {
            _mounts.Add(mount);
        }
    }


    public void Acquire(Equippable equippable)
    {
        _equipables.Add(equippable);
        Equip(equippable);
    }


    public void Acquire(GameObject acquirer, Equippable equippable)
    {
        if (!_equipables.Contains(equippable))
        {
            _equipables.Add(equippable);
        }

        // try to equip to acquirer
        foreach (EquipmentMount mount in _mounts.ToList())
        {
            if (mount.gameObject == acquirer)
            {
                DebugHUD.GetInstance().PresentToast("add to acquirer");
                Equip(equippable, mount);
                return;
            }
        }

        // try to find open mount
        foreach (EquipmentMount mount in _mounts.ToList())
        {
            if (mount.Equippable == null)
            {
                Equip(equippable, mount);
                return;
            }
        }

        // stick it in the bag
        if (_bag.Count >= MaxBagCount)
        {
            PerformDrop(equippable);
            return;
        }
        
        MoveToBag(equippable);     
    }


    public void Drop(GameObject gameObject)
    {
        Equippable equippable = gameObject.GetComponent<Equippable>();
        if (equippable == null) return;

        PerformDrop(equippable);
    }


    private void Equip(Equippable equippable)
    {
        if (equippable == null) return;

        foreach (EquipmentMount mount in _mounts.ToList())
        {
            if (mount.Equippable == null)
            {
                Equip(equippable, mount);
                return;
            }
        }

        MoveToBag(equippable);
    }


    private void Equip(Equippable equippable, EquipmentMount mount)
    {
        if (equippable == null) return;

        Unequip(mount);

        if (equippable.EquippedBy != null)
        {
            equippable.EquippedBy.Equippable = null;
        }

        mount.Equippable = equippable;
        MoveToEquippedMount(equippable, mount);
    }


    public void Unequip(EquipmentMount mount)
    {
        if (mount == null || mount.Equippable == null) return;

        Equippable equippable = mount.Equippable;
        mount.Equippable = null;

        if (equippable.Droppable)
        {
            PerformDrop(equippable);
            return;
        }

        // try to find an empty mount
        foreach (EquipmentMount em in StowedMountables)
        {
            if (em.Equippable == null)
            {
                DebugHUD.GetInstance().PresentToast("move to empty mount");
                MoveToStowedMount(equippable, em);
                return;
            }
        }

        // no more room in bag
        if (_bag.Count >= MaxBagCount)
        {
            DebugHUD.GetInstance().PresentToast("can't bag, drop");
            PerformDrop(equippable);
            return;
        }

        DebugHUD.GetInstance().PresentToast("add to bag");
        MoveToBag(equippable);
    }


    private void MoveToStowedMount(Equippable equippable, EquipmentMount mount)
    {
        mount.Equippable = equippable;
        equippable.transform.SetParent(mount.transform);
        equippable.transform.localEulerAngles = equippable.StowedLocation + mount.MountLocation;
        equippable.transform.localPosition = equippable.StowedRotation + mount.MountRotation;
        equippable.transform.localScale = equippable.StowedScale + mount.MountScale;
        equippable.gameObject.SetActive(equippable.StowedVisible);
        if (equippable.gameObject.HasComponent<Rigidbody>())
        {
            equippable.GetComponent<Rigidbody>().isKinematic = true;
        }
        equippable.UnEquipped();
    }


    private void MoveToBag(Equippable equippable)
    {
        _bag.Add(equippable);
        equippable.transform.SetParent(this.transform);
        equippable.transform.localEulerAngles = equippable.StowedLocation;
        equippable.transform.localPosition = equippable.StowedRotation;
        equippable.transform.localScale = equippable.StowedScale;
        equippable.gameObject.SetActive(false);
        equippable.UnEquipped();
    }


    private void MoveToEquippedMount(Equippable equippable, EquipmentMount mount)
    {
        mount.Equippable = equippable;
        equippable.transform.SetParent(mount.transform);        
        equippable.transform.localPosition = equippable.EquippedLocation + mount.MountLocation;
        equippable.transform.localEulerAngles = equippable.EquippedRotation + mount.MountRotation;
        equippable.transform.localScale = equippable.EquippedScale + mount.MountScale;
        equippable.gameObject.SetActive(equippable.EquippedVisible);
        if (equippable.gameObject.HasComponent<Rigidbody>())
        {
            equippable.GetComponent<Rigidbody>().isKinematic = true;
        }
        equippable.Equipped(mount);
    }


    private void PerformDrop(Equippable equippable)
    {
        // remove from any mounts
        foreach (EquipmentMount mount in _mounts.ToList())
        {
            if (mount.Equippable == equippable)
            {
                mount.Equippable = null;
                break;
            }
        }

        // remove from bag
        if (_bag.Contains(equippable))
        {
            _bag.Remove(equippable);
        }

        // finish drop
        _equipables.Remove(equippable);
        equippable.transform.SetParent(null);
        if (equippable.gameObject.HasComponent<Rigidbody>())
        {
            equippable.GetComponent<Rigidbody>().isKinematic = false;
        }
        equippable.Dropped();
    }


    public List<Equippable> Equipables
    {
        get
        {
            return _equipables;
        }
    }


}
