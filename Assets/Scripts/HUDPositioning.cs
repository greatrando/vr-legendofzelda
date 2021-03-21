using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;


public class HUDPositioning : MonoBehaviour
{


    private bool _loaded = false;
    private bool _isOculus = false;
    private Vector3 _originalPostion;
    private float _time;


    void Start()
    {
    }


    public void Update()
    {
        if (_loaded) return;
        _loaded = true;

        _isOculus = isHardwarePresent();
        if (!_isOculus)
        {
            Transform childTransform = GameObject.Find("XR Rig").GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "testsword");
            childTransform.gameObject.SetActive(true);
        }

        AdjustHUD();
    }


    private void AdjustHUD()
    {
        if (!_isOculus) return;

        Transform canvas = this.transform.Find("Canvas");
        canvas.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        Transform hearts = canvas.Find("Hearts");
        hearts.localScale = new Vector3(0.2f, 0.2f, 1);

        RectTransform rect = hearts.GetComponent<RectTransform>();
        if (_originalPostion == null)
        {
            _originalPostion = rect.localPosition;
        }

        rect.localPosition = new Vector3(-10, -100 , 0);
    }


    public static bool isHardwarePresent()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }


}
