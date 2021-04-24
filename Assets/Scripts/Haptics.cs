using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Haptics : MonoBehaviour
{


    public enum HAND
    {
        Left,
        Right
    }


    public enum VIBRATION_FORCE
    {
        Light,
        Medium,
        Hard,
    }
    

    private UnityEngine.XR.InputDevice _leftHand;
    private UnityEngine.XR.InputDevice _rightHand;
    private bool _supported = false;

    
    private void Start()
    {
        InitializeOVRHaptics();
    }
  
 
    void OnEnable()
    {
        StartCoroutine(InitializeOVRHaptics());
    }


    private IEnumerator InitializeOVRHaptics()
    {
        yield return new WaitForSeconds(1.0f);

        _leftHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);
        _rightHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);

        UnityEngine.XR.HapticCapabilities capabilities;
        if (_leftHand.TryGetHapticCapabilities(out capabilities))
        {
            _supported = capabilities.supportsImpulse;
        }

        yield return null;
    }


    public void Play(HAND hand, VIBRATION_FORCE force, float time)
    {
        try
        {
            UnityEngine.XR.InputDevice device = hand == HAND.Left ? _leftHand : _rightHand;
    
            switch (force)
            {
                case VIBRATION_FORCE.Light:
                    device.SendHapticImpulse(0, 0.25f, time);
                    break;
                case VIBRATION_FORCE.Medium:
                    device.SendHapticImpulse(0, 0.5f, time);
                    break;
                default: //case VIBRATION_FORCE.Hard:
                    device.SendHapticImpulse(0, 1f, time);
                    break;
            }
        }
        catch (System.Exception)
        {
            //could be non-oculus
        }
    }


}
