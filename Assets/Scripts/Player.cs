using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{


    private static Player _player = null;


    public GameObject Camera;
    public HealthSystem HealthSystem;
    public GameObject HeartsContainer;
    public Texture HeartFullTexture;
    public Texture HeartHalfTexture;
    public Texture HeartNoneTexture;


    private GameObject XRRig;
    private List<RawImage> _hearts;
    private Wallet _wallet;
    private Text _rupieValue;
    private bool _isLoaded = false;


    void Start()
    {
        _player = this;

        HealthSystem = this.GetComponent<HealthSystem>();
        if (HealthSystem == null)
        {
            HealthSystem = this.gameObject.AddComponent<HealthSystem>();
        }

        _hearts = new List<RawImage>();
        for (int idx = 0; idx < HeartsContainer.transform.childCount; idx++)
        {
            if (HeartsContainer.GetAllChildren()[idx].HasComponent<Text>())
            {
                _rupieValue = HeartsContainer.GetAllChildren()[idx].GetComponent<Text>();
            }
            else
            {
                _hearts.Add(HeartsContainer.GetAllChildren()[idx].GetComponent<RawImage>());
            }
        }

        HealthSystem.MaxHealth = 3; // 3 hearts
        HealthSystem.OnHealthChanged += OnHealthChanged;
        HealthSystem.OnDeath += OnDeath;
        HealthSystem.Health = HealthSystem.MaxHealth;

        _wallet = this.GetComponent<Wallet>();
        _wallet.OnChanged += OnWalletChanged;
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


    private void OnHealthChanged()
    {
        UnityEngine.Debug.Log("Health Changed: " + HealthSystem.Health.ToString());
        _hearts[0].texture = HealthSystem.Health > 0.5 ? HeartFullTexture : HealthSystem.Health < 0.5 ? HeartNoneTexture : HeartHalfTexture;
        _hearts[1].texture = HealthSystem.Health > 1.5 ? HeartFullTexture : HealthSystem.Health < 1.5 ? HeartNoneTexture : HeartHalfTexture;
        _hearts[2].texture = HealthSystem.Health > 2.5 ? HeartFullTexture : HealthSystem.Health < 2.5 ? HeartNoneTexture : HeartHalfTexture;
    }


    private void OnDeath()
    {
        UnityEngine.Debug.Log("Quit");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    private void OnWalletChanged()
    {
        _rupieValue.text = _wallet.CurrentValue.ToString();
    }


    public static Player GetInstance()
    {
        return _player;
    }

}
