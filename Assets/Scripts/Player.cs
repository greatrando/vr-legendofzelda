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


    private List<RawImage> _hearts;
    private Wallet _wallet;
    private Text _rupieValue;


    void Start()
    {
        _player = this;
        // if (Application.isEditor)
        // {
        //     Vector3 pos = this.transform.position;
        //     pos.y = 20f;
        //     this.transform.position = pos;
        // }

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


    public void Update()
    {
        // Vector3 angles = Camera.transform.eulerAngles;
        // angles.x = 0;
        // this.transform.eulerAngles = angles;
        // Quaternion rotation = this.transform.rotation;

        // Vector3 position = Camera.transform.position + (rotation * new Vector3(0, -.43f, -0.73f));
        // this.transform.position = position;
        // DebugHUD.GetInstance().PresentToast(Camera.transform.eulerAngles.ToString());
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
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
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
