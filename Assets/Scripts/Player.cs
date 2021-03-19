using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{


    private static Player _player = null;


    public HealthSystem HealthSystem;
    public GameObject HeartsContainer;
    public Texture HeartFullTexture;
    public Texture HeartHalfTexture;
    public Texture HeartNoneTexture;


    private List<RawImage> _hearts;
    private Wallet _wallet;


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
            _hearts.Add(HeartsContainer.GetAllChildren()[idx].GetComponent<RawImage>());
        }

        HealthSystem.MaxHealth = 3; // 3 hearts
        HealthSystem.OnHealthChanged += OnHealthChanged;
        HealthSystem.OnDeath += OnDeath;
        HealthSystem.Health = HealthSystem.MaxHealth;

        _wallet = this.GetComponent<Wallet>();
        _wallet.OnChanged += OnWalletChanged;
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
        UnityEngine.Debug.Log("Wallet changed to: " + _wallet.CurrentValue);
    }


    public static Player GetInstance()
    {
        return _player;
    }

}
