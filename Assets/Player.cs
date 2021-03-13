using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{


    public HealthSystem HealthSystem;


    void Start()
    {
        HealthSystem = this.GetComponent<HealthSystem>();
        if (HealthSystem == null)
        {
            HealthSystem = this.gameObject.AddComponent<HealthSystem>();
        }

        HealthSystem.MaxHealth = 3; // 3 hearts
        HealthSystem.OnHealthChanged += OnHealthChanged;
        HealthSystem.OnDeath += OnDeath;
        HealthSystem.Health = HealthSystem.MaxHealth;
    }


    private void OnHealthChanged()
    {
        UnityEngine.Debug.Log("Health Changed: " + HealthSystem.Health.ToString());
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


}
