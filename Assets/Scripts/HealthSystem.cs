using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{


    public float MaxHealth { get; set; }


    public delegate void DeathEvent();
    public delegate void HealthChangedEvent();


    public DeathEvent OnDeath = null;
    public HealthChangedEvent OnHealthChanged = null;


    private float _health = 0;


    public void Start()
    {
        Health = MaxHealth;

        Damageee takeDamage = this.GetComponent<Damageee>();

        if (takeDamage == null)
        {
            takeDamage = this.gameObject.AddComponent<Damageee>();
        }

        takeDamage.OnApplyDamage += TakeDamage;
    }


    public void TakeDamage(GameObject gameObject)
    {
        Damageor damageor = gameObject.GetComponent<Damageor>();
        if (damageor != null)
        {
            Health -= damageor.DamageAmount;
        }
    }


    public float Health 
    { 
        get
        {
            return _health;
        }
        set
        {
            value = (float)System.Math.Round(value, 1);
            value = Mathf.Min(value, MaxHealth);

            _health = value;

            this.OnHealthChanged?.Invoke();

            if (_health <= 0)
            {
                this.OnDeath?.Invoke();
            }
        }
    }


}
