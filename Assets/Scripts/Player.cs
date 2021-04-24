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
    public AudioSource DieAudio = null;


    private GameObject XRRig;
    private List<RawImage> _hearts;
    private Wallet _wallet;
    private Text _rupieValue;
    private Haptics _haptics;
    private bool _isLoaded = false;
    private bool _isDead = false;


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
        HealthSystem.OnTakingDamage += OnTakingDamage;
        HealthSystem.OnDeath += OnDeath;
        HealthSystem.Health = HealthSystem.MaxHealth;

        _wallet = this.GetComponent<Wallet>();
        _wallet.OnChanged += OnWalletChanged;
        OnWalletChanged();

        _haptics = this.GetComponent<Haptics>();
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
            OnWalletChanged();
            _isLoaded = true;
        }

        if (_isDead)
        {
            _isDead = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
            // #if UNITY_EDITOR
            //     UnityEditor.EditorApplication.isPlaying = false;
            // #else
            //     Application.Quit();
            // #endif
            return;
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


    public void PlayAudio(AudioSource audioSource)
    {
        PlayAudio(audioSource, 1);
    }


    public void PlayAudio(AudioSource audioSource, int count)
    {
        if (audioSource == null) return;

        AudioClip clip = audioSource.clip;
        try { StartCoroutine(PlayAudioLoops(clip, count)); } catch (System.Exception) { }
    }


    private IEnumerator PlayAudioLoops(AudioClip clip, int count)
    {
        yield return null;

        float repeatDelayTime = clip.length;
        if (count > 1)
        {
            repeatDelayTime = 0.05f;
        }

        for (int index = 0; index < count; index++)
        {
            AudioSource.PlayClipAtPoint(clip, this.gameObject.transform.position, 1.0f);

            yield return new WaitForSeconds(repeatDelayTime);
        }

        yield return null;
    }


    public Wallet Wallet
    {
        get
        {
            return _wallet;
        }
    }


    private void OnHealthChanged()
    {
        UnityEngine.Debug.Log("Health Changed: " + HealthSystem.Health.ToString());
        _hearts[0].texture = HealthSystem.Health > 0.5 ? HeartFullTexture : HealthSystem.Health < 0.5 ? HeartNoneTexture : HeartHalfTexture;
        _hearts[1].texture = HealthSystem.Health > 1.5 ? HeartFullTexture : HealthSystem.Health < 1.5 ? HeartNoneTexture : HeartHalfTexture;
        _hearts[2].texture = HealthSystem.Health > 2.5 ? HeartFullTexture : HealthSystem.Health < 2.5 ? HeartNoneTexture : HeartHalfTexture;
    }


    private void OnTakingDamage()
    {
        _haptics.Play(Haptics.HAND.Left, Haptics.VIBRATION_FORCE.Hard, 0.5f);
        _haptics.Play(Haptics.HAND.Right, Haptics.VIBRATION_FORCE.Hard, 0.5f);
    }


    private void OnDeath()
    {
        UnityEngine.Debug.Log("Quit");


        try { StartCoroutine(StartDeath()); } catch (System.Exception) { }
    }


    private IEnumerator StartDeath()
    {
        if (DieAudio != null)
        {
            DieAudio.Play();
        }
        yield return null;

        float waitTime = 0;
        while (waitTime < 3)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        _isDead = true;

        yield return null;
    }


    private void OnWalletChanged()
    {
        // DebugHUD.GetInstance().PresentToast("set value: " + _wallet.CurrentValue.ToString());
        _rupieValue.text = _wallet.CurrentValue.ToString();
    }


    public static Player GetInstance()
    {
        return _player;
    }

}
