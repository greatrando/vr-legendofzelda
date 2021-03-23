using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerKeyboardController : MonoBehaviour
{
    
    private const bool ENABLED = true;

    public GameObject MainCamera;
    public GameObject GroundCollider;
    public CharacterController controller;

    public float Speed = 12f;
    public float MouseSensitivity = 100f;
    public float Gravity = -9.8f;
    public float GroundDistance = 0.4f;
    public float JumpHeight = 3f;

    private InputDevice _rightController;
    private GameObject _capsule;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float _groundPointY;
    private bool shouldDie = false;
    private int _dieFrames = 0;
    private GameObject _pickupHammer;
    private bool _hammerEquipped = false;


    void Start()
    {
        if (!ENABLED) return;
        _capsule = this.transform.FindChildRecursive("Capsule").gameObject;

        if (!Application.isEditor)
        {
            var inputDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);
            _rightController = inputDevices[2];
        }
    }


    void Update()
    {
        if (!ENABLED) return;

        UpdateCamera();
        UpdateMovement();
        UpdateGravity();
    }
  

    private void UpdateGravity()
    {
        Vector3 velocity = new Vector3(0, Gravity, 0);

        controller.Move(velocity * Time.deltaTime);
    }


    private void UpdateMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z) * Speed * Time.deltaTime;

        move = this.transform.TransformDirection(move);

        //so in VR, two rotations, one for camera looking around, another for the controller rotating the camera
        //we need to account for both
        float camY = MainCamera.transform.rotation.eulerAngles.y;
        float rigY = this.transform.rotation.eulerAngles.y;
        float newY = rigY - (camY - rigY); //find difference between rig and camera rotation

        move = Quaternion.Euler(0, -newY, 0) * move;
        controller.Move(move);
    }


    private void UpdateCamera()
    {
        if (Application.isEditor)
        {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            MainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            float rotateInfluence = 60f;

            Vector3 euler = transform.rotation.eulerAngles;

            Vector2 axis2D;
            _rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out axis2D);

            euler.y += axis2D.x * Time.deltaTime *rotateInfluence;
            transform.rotation = Quaternion.Euler(euler);
        }
        
        // else
        // {
        //     Vector3 cameraAngles = MainCamera.transform.localEulerAngles;
        //     Vector3 newAngles = new Vector3(0, cameraAngles.y, 0);
        //     cameraAngles.y = 0;
        //     this.transform.localEulerAngles = newAngles;
        //     MainCamera.transform.localEulerAngles = cameraAngles;
        // }
    }


}
