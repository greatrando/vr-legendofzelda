using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wallet : MonoBehaviour
{


    public delegate void ChangedEvent();


    public ChangedEvent OnChanged = null;


    public int InitialValue = 0;


    private int _currentValue = 0;


    public void Start()
    {
        _currentValue = InitialValue;
    }


    public int CurrentValue
    {
        get
        {
            return _currentValue;
        }
        set
        {
            _currentValue = value;
            OnChanged?.Invoke();
        }
    }

}
