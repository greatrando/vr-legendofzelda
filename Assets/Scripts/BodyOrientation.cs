using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class BodyOrientation : MonoBehaviour
{

    /*
        Had difficulty finding the orientation of the character's body
        easy to get orientation of camera, but doesn't work if the user moves head (looks away from body facing direction)
        further orientation problems occur when using snap turn

        Solution:
            Assume hands are in front of body, doing something (climbing etc.)
            Find center point between hands
            Forward direction is from camera in the direction of that center point
    */


    public Vector3 CameraPosition { get; private set; }
    public Vector3 UpDirection { get; private set; }
    public Vector3 ForwardFacingDirection { get; private set; }


    // [SerializeField] private InputActionReference _leftHandDevicePosition= null;
    // [SerializeField] private InputActionReference  _rightHandDevicePosition = null;
    public GameObject _LeftHand;
    public GameObject _RightHand;

    private GameObject _camera;
    private GameObject _character;
    private Vector3 _leftHandPosition;
    private Vector3 _rightHandPosition;


    private void Awake()
    {
        CharacterController characterController = GetComponent<CharacterController>();
        _character = characterController.transform.gameObject;

        _camera = GetComponent<XRRig>().cameraGameObject;

        // _leftHandDevicePosition.action.performed += OnLeftHandPositionChanged;
        // _rightHandDevicePosition .action.performed += OnRightHandPositionChanged;
    }


    // private void OnEnable()
    // {
    //     _leftHandDevicePosition.asset.Enable();
    //     _rightHandDevicePosition.asset.Enable();
    // }


    // private void OnDisable()
    // {
    //     _leftHandDevicePosition.asset.Disable();
    //     _rightHandDevicePosition.asset.Disable();
    // }


    // private void OnDestroy()
    // {
    //     _leftHandDevicePosition.action.performed -= OnLeftHandPositionChanged;
    //     _rightHandDevicePosition.action.performed -= OnRightHandPositionChanged;
    // }


    private void FixedUpdate()
    {
        this.CameraPosition = _camera.transform.position;
        this.UpDirection = _character.transform.up;

        _leftHandPosition = _LeftHand.transform.position;
        _rightHandPosition = _RightHand.transform.position;

        // get vector from camera to center of hands (world position)
        Vector3 centerPoint = (_leftHandPosition + _rightHandPosition) / 2;

        this.transform.Find("Sphere").transform.position = centerPoint;

        Vector3 forwardFacingDirection = centerPoint - this.CameraPosition;

        // change to a horizontal vector from our camera in assumed body direction
        this.ForwardFacingDirection = new Vector3(forwardFacingDirection.x, 0, forwardFacingDirection.z).normalized;
        DebugHUD.GetInstance().PresentToast("Forward: " + this.ForwardFacingDirection.ToString(), 0.2f, 2, 0.2f);

        Debug.DrawRay(_camera.transform.position, this.ForwardFacingDirection, Color.red);
    }


    // private void OnLeftHandPositionChanged(InputAction.CallbackContext context)
    // {
    //     _leftHandPosition = context.ReadValue<Vector3>();

    //     // we need our center point in world position
    //     _leftHandPosition = _character.transform.TransformPoint(_leftHandPosition);
    // }


    // private void OnRightHandPositionChanged(InputAction.CallbackContext context)
    // {
    //     _rightHandPosition = context.ReadValue<Vector3>();

    //     // we need our center point in world position
    //     _rightHandPosition = _character.transform.TransformPoint(_rightHandPosition);
    // }


}