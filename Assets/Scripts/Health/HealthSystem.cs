using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{


    public GameObject ParentObject = null;
    public float MaxHealth;
    public AudioSource DamageAudioSource = null;
    public AudioSource DieAudioSource = null;
    public List<string> IgnoreDamagees = new List<string>();


    public delegate void DeathEvent();
    public delegate void HealthChangedEvent();
    public delegate void TakingDamage();
    public delegate void Healing();


    public DeathEvent OnDeath = null;
    public HealthChangedEvent OnHealthChanged = null;
    public TakingDamage OnTakingDamage = null;
    public Healing OnHealing = null;


    private float _health = 0;


    public void Start()
    {
        if (ParentObject == null)
        {
            ParentObject = this.gameObject;
        }

        Health = MaxHealth;

        AttachDamagee(this.gameObject);
        AttachDamagee(ParentObject);
    }


    private void AttachDamagee(GameObject gameObject)
    {
        Damageee takeDamage = gameObject.GetComponent<Damageee>();

        if (takeDamage == null)
        {
            takeDamage = gameObject.AddComponent<Damageee>();
        }

        takeDamage.OnApplyDamage += TakeDamage;
    }


    public void TakeDamage(GameObject gameObject)
    {
        if (!IgnoreDamagees.Contains(gameObject.name))
        {
            Damageor damageor = gameObject.GetComponent<Damageor>();
            if (damageor != null)
            {
                if (DamageAudioSource != null)
                {
                    DamageAudioSource.Play();
                }
                OnTakingDamage?.Invoke();
                Health -= damageor.DamageAmount;
                KnockBack(damageor);
            }
        }
    }


    private void KnockBack(Damageor damageor)
    {
        Vector3 _knockBack = (this.transform.position - damageor.transform.position).normalized;
        _knockBack.y = 0;
        if (ParentObject.GetComponent<CharacterController>() != null)
        {
            _knockBack *= 100f * 2;
            ParentObject.GetComponent<CharacterController>().SimpleMove(_knockBack);
        }
        else
        {
            _knockBack *= 1;
            ParentObject.transform.position += _knockBack;
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

            bool healing = (value > _health);

            _health = value;

            if (healing)
            {
                OnHealing?.Invoke();
            }

            this.OnHealthChanged?.Invoke();

            if (_health <= 0)
            {
                Player.GetInstance().PlayAudio(DieAudioSource);
                this.OnDeath?.Invoke();
            }
        }
    }


}
