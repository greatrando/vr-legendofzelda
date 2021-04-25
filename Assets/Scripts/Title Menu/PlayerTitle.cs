using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTitle : MonoBehaviour
{

    private static PlayerTitle _player = null;


    public GameObject Camera;


    private GameObject XRRig;
    public Haptics Haptics;
    private bool _isLoaded = false;


    void Start()
    {
        _player = this;

        Haptics = this.GetComponent<Haptics>();
    }


    public void OnEnable()
    {
        XRRig = GameObject.Find("XR Rig");
    }


    public void Update()
    {
        if (!_isLoaded)
        {
            if (Application.isEditor)
            {
                Vector3 newCameraPosition = Camera.transform.position;
                newCameraPosition.y = 1.5f;
                Camera.transform.position = newCameraPosition;
            }
            _isLoaded = true;
        }

        // DebugHUD.GetInstance().PresentToast("From: " + this.transform.eulerAngles + " :: " + Camera.transform.eulerAngles);
        Vector3 rigRotation = this.transform.parent.localEulerAngles;
        rigRotation.x = -Camera.transform.eulerAngles.x;
        rigRotation.y = 0; //Camera.transform.eulerAngles.y;
        this.transform.parent.localEulerAngles = rigRotation;
        // DebugHUD.GetInstance().PresentToast("To: " + this.transform.eulerAngles);

        Vector3 positionCamera = Camera.transform.position;
        Vector3 positionXR = XRRig.transform.position;
        Vector3 positionCapsule = this.transform.localPosition;
        float newPosition = -0.5f;
        float currentPositon = positionCapsule.y;
        if (currentPositon != newPosition)
        {
            // DebugHUD.GetInstance().PresentToast(positionCamera.y.ToString() + " to " + positionXR.y.ToString() + " from " + currentPositon.ToString() + " Set height to: " + newPosition.ToString(), 0.5f, 10, 0.5f);
            positionCapsule.y = newPosition;
            this.transform.localPosition = positionCapsule;
        }
       
        // this.transform.localPosition = new Vector3(-0.25f, this.transform.localPosition.y, 0);
    }


    public static PlayerTitle GetInstance()
    {
        return _player;
    }


}
