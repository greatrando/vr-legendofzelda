using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuGesture : MonoBehaviour
{


    public GameObject Preparing;

    public HandTracking LeftHandTracking;
    public GameObject LeftPointer;
    public GameObject LeftThumb;

    public HandTracking RightHandTracking;
    public GameObject RightPointer;
    public GameObject RightThumb;


    private DateTime _waitUntil = DateTime.MaxValue;
    private Light _light;
    private bool _changing = false;


    void Start()
    {
        Preparing.transform.SetParent(LeftHandTracking.gameObject.transform);
        _light = Preparing.GetAllChildren()[0].gameObject.GetComponent<Light>();
    }
    

    public void Update()
    {
        // const float MAX_DISTANCE = 5f;
        const float MAX_DISTANCE = 0.02f;

        float distancePointer = Vector3.Distance(LeftPointer.transform.position, RightPointer.transform.position);
        float distanceThumb = Vector3.Distance(LeftThumb.transform.position, RightThumb.transform.position);

        if  (
                _changing || 
                !LeftHandTracking.IsIndexOut || !LeftHandTracking.IsThumbUp || 
                !RightHandTracking.IsIndexOut || !RightHandTracking.IsThumbUp || 
                distancePointer > MAX_DISTANCE || distanceThumb > MAX_DISTANCE
            )
        {
            _waitUntil = DateTime.MaxValue;
            _light.intensity = 0;
            Preparing.SetActive(false);
            return;
        }

        if (_waitUntil == DateTime.MaxValue)
        {
            _waitUntil = DateTime.Now.AddSeconds(2);
        }
        else if (_waitUntil < DateTime.Now)
        {
            // _waitUntil = DateTime.Now.AddSeconds(4);
            _changing = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            return;
        }

        double timeDelta = _waitUntil.Subtract(DateTime.Now).TotalMilliseconds;
        float percent = timeDelta <= 0 ? 1.0f : 2000f / (float)timeDelta;

        PlayerTitle.GetInstance().Haptics.Play(
            Haptics.HAND.Left, 
            percent > 0.5f ? Haptics.VIBRATION_FORCE.Hard : percent > 0.25 ? Haptics.VIBRATION_FORCE.Medium : Haptics.VIBRATION_FORCE.Light, 
            0.1f);
        PlayerTitle.GetInstance().Haptics.Play(
            Haptics.HAND.Right, 
            percent > 0.5f ? Haptics.VIBRATION_FORCE.Hard : percent > 0.25 ? Haptics.VIBRATION_FORCE.Medium : Haptics.VIBRATION_FORCE.Light, 
            0.1f);

        Preparing.transform.position = (LeftPointer.transform.position + RightThumb.transform.position) / 2;
        Preparing.transform.LookAt(Camera.main.transform);
        Preparing.SetActive(true);

        _light.intensity = (10 * percent);
    }


}
