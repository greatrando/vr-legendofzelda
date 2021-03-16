using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempFindTransform : MonoBehaviour
{


    private DebugHUD _debug;

    private Transform _transform;
    private Vector3 _zeroPosition;

    private float _destinationX;
    private float _destinationY;
    private float _destinationZ;
    private Vector3 _originalPosition;
    private Vector3 _destinationPosition;
    private Vector3 _originalRotation;
    private Vector3 _destinationRotation;
    private float _timeElapsed;


    // Start is called before the first frame update
    void Start()
    {
        _debug = DebugHUD.FindDebugHud();

        _debug.AddTextElement(new DebugHUD.HUDTextElement(){ 
            id = "X", 
            position = new Vector3(-60, 0, 0), 
            width = 1200, 
            color = Color.yellow, 
            defaultText = "X: "
        });

        _debug.AddTextElement(new DebugHUD.HUDTextElement(){ 
            id = "Y", 
            position = new Vector3(-60, -30, 0), 
            width = 1200, 
            color = Color.yellow, 
            defaultText = "Y: "
        });

        _debug.AddTextElement(new DebugHUD.HUDTextElement(){ 
            id = "Z", 
            position = new Vector3(-60, -60, 0), 
            width = 1200, 
            color = Color.yellow, 
            defaultText = "Z: "
        });

        _debug.AddTextElement(new DebugHUD.HUDTextElement(){ 
            id = "ROTATION", 
            position = new Vector3(-60, -90, 0), 
            width = 1200, 
            color = Color.yellow, 
            defaultText = "A: "
        });   

        _debug.AddTextElement(new DebugHUD.HUDTextElement(){ 
            id = "T", 
            position = new Vector3(-60, -120, 0), 
            width = 1200, 
            color = Color.yellow, 
            defaultText = "T: "
        });   


        _transform = GameObject.Find("SwordGrabTransform").transform;
        _zeroPosition = _transform.position;

        ChangeDirection();
    }


    private const float LERP_TIME = 10.0f;
    private enum CHANGE_TYPE
    {
        NONE,
        X_FORWARD,
        X_REVERSE,
        Y_FORWARD,
        Y_REVERSE,
        Z_FORWARD,
        Z_REVERSE,
        ROTATE_FORWARD,
        ROTATE_REVERSE
    }
    private CHANGE_TYPE _change = CHANGE_TYPE.NONE;


    private void ChangeDirection()
    {
        const float MAX_POS = 0.3f;

        _transform.position = _zeroPosition;
        _transform.eulerAngles = Vector3.zero;

        _destinationX = 0f;
        _destinationY = 0f;
        _destinationZ = 0f;
        _destinationRotation = Vector3.zero;

        switch (_change)
        {
            case CHANGE_TYPE.NONE:
            case CHANGE_TYPE.ROTATE_REVERSE:
                _destinationX = MAX_POS;
                _change = CHANGE_TYPE.X_FORWARD;
                break;
            case CHANGE_TYPE.X_FORWARD:
                _destinationX = -MAX_POS;
                _change = CHANGE_TYPE.X_REVERSE;
                break;
            case CHANGE_TYPE.X_REVERSE:
                _destinationY = MAX_POS;
                _change = CHANGE_TYPE.Y_FORWARD;
                break;
            case CHANGE_TYPE.Y_FORWARD:
                _destinationY = -MAX_POS;
                _change = CHANGE_TYPE.Y_REVERSE;
                break;
            case CHANGE_TYPE.Y_REVERSE:
                _destinationZ = MAX_POS;
                _change = CHANGE_TYPE.Z_FORWARD;
                break;
            case CHANGE_TYPE.Z_FORWARD:
                _destinationZ = -MAX_POS;
                _change = CHANGE_TYPE.Z_REVERSE;
                break;
            case CHANGE_TYPE.Z_REVERSE:
                _destinationRotation = new Vector3(90f, 0, 0);
                _change = CHANGE_TYPE.ROTATE_FORWARD;
                break;
            case CHANGE_TYPE.ROTATE_FORWARD:
                _destinationRotation = new Vector3(-90f, 0, 0);
                _change = CHANGE_TYPE.ROTATE_REVERSE;
                break;
        }

        _timeElapsed = 0;

        _destinationPosition = _zeroPosition + new Vector3(_destinationX, _destinationY, _destinationZ);
        _originalPosition = _transform.position;

        _originalRotation = transform.eulerAngles + _destinationRotation;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        _timeElapsed += Time.fixedDeltaTime;

        _transform.position = Vector3.Lerp(_originalPosition, _destinationPosition, _timeElapsed / LERP_TIME);
        _transform.eulerAngles = Vector3.Lerp(_originalRotation, _destinationRotation,  _timeElapsed / LERP_TIME);

        DebugHUD.FindDebugHud().UpdateTextElement("X", "X: " + _zeroPosition.x + " :: " + _transform.position.x.ToString());
        DebugHUD.FindDebugHud().UpdateTextElement("Y", "Y: " + _zeroPosition.y + " :: " + _transform.position.y.ToString());
        DebugHUD.FindDebugHud().UpdateTextElement("Z", "Z: " + _zeroPosition.z + " :: " + _transform.position.z.ToString());
        DebugHUD.FindDebugHud().UpdateTextElement("ROTATION", "R: " + _transform.rotation.x.ToString());
        DebugHUD.FindDebugHud().UpdateTextElement("T", "T: " + _timeElapsed.ToString());

        if (_timeElapsed >= LERP_TIME)
        {
            ChangeDirection();
        }
    }


}
