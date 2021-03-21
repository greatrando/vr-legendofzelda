using UnityEngine;


public class PlayerKeyboardController : MonoBehaviour
{
    
    private const bool ENABLED = true;

    public GameObject MainCamera;
    public GameObject GroundCollider;
    // public CharacterController controller;

    public float Speed = 12f;
    public float MouseSensitivity = 100f;
    public float Gravity = -9.8f;
    public float GroundDistance = 0.4f;
    public float JumpHeight = 3f;

    private GameObject _capsule;
    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool shouldDie = false;
    private int _dieFrames = 0;
    private GameObject _pickupHammer;
    private bool _hammerEquipped = false;


    void Start()
    {
        if (!ENABLED) return;
isGrounded = true;
        _capsule = this.transform.FindChildRecursive("Capsule").gameObject;
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
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded)
        {
            bool shouldJump = Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(1);
            if (shouldJump)
            {
                velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }

        velocity.y += (Gravity * Time.deltaTime);

        this.transform.position += (velocity * Time.deltaTime);
        // controller.Move(velocity * Time.deltaTime);
    }


    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        UnityEngine.Debug.Log("Grounded.");
    }


    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        UnityEngine.Debug.Log("NOT grounded.");
    }


    private void UpdateMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z) * Speed * Time.deltaTime;

        this.transform.position += move;
        // controller.Move(move);
    }


    private void UpdateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        MainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * mouseX);
    }


}
