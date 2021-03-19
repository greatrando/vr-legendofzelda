using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Damageor))]
public class Octorok : MonoBehaviour
{


    public Material Material;
    public float WalkSpeed = 1;
    public float FireSeconds = 2.0f;
    public float FireSpeed = 10f;
    public float MaxHealth = 0.5f;


    private const float WALK_LERP_TIME = 2f;
    private const float NOSE_LERP_TIME = 0.25f;
    private const float LEG_LERP_TIME = 0.4f;


    private Transform _nose;

    private Vector3 _originalPosition;
    private Vector3 _destinationPosition;
    private float _destinationTimeElapsed;

    private List<Transform> _leftLegs;
    private List<Transform> _rightLegs;
    private Vector3 _originalLegRotation;
    private Vector3 _destinationLegRotation;
    private float _legTimeElapsed;

    private GameObject _rock;
    // private GameObject _projectile;
    private float _nextFire;

    private Vector3 _originalNosePosition;
    private Vector3 _destinationNosePosition;
    private float _noseTimeElapsed;

    private List<GameObject> _ignoreObjects;


    void Start()
    {
        ChangePositionDestination();

        _ignoreObjects = new List<GameObject>();

        _nose = this.transform.Find("Nose");
        ChangeNoseDirection();

        _rock = this.transform.Find("Rock").gameObject;
        _nextFire = 0;

        _ignoreObjects.Add(this.gameObject);
        _ignoreObjects.Add(_nose.gameObject);
        _ignoreObjects.Add(_nose.GetChild(0).gameObject);
        _ignoreObjects.Add(_rock.gameObject);
        _ignoreObjects.Add(this.transform.Find("Right Eye").gameObject);
        _ignoreObjects.Add(this.transform.Find("Left Eye").gameObject);

        _leftLegs = new List<Transform>();
        foreach (GameObject gameObject in this.transform.Find("Left Legs").gameObject.GetAllChildren().Where(gameObject => gameObject.name.StartsWith("Left Leg")))
        {
            _ignoreObjects.Add(gameObject);
            _leftLegs.Add(gameObject.transform);
        }

        _rightLegs = new List<Transform>();
        foreach (GameObject gameObject in this.transform.Find("Right Legs").gameObject.GetAllChildren().Where(gameObject => gameObject.name.StartsWith("Right Leg")))
        {
            _ignoreObjects.Add(gameObject);
            _rightLegs.Add(gameObject.transform);
        }

        if (Material != null)
        {
            this.GetComponent<MeshRenderer>().material = Material;
            _nose.GetComponent<MeshRenderer>().material = Material;
            _nose.GetChild(0).GetComponent<MeshRenderer>().material = Material;
            foreach (Transform leg in _leftLegs)
            {
                leg.GetComponent<MeshRenderer>().material = Material;
            }
            foreach (Transform leg in _rightLegs)
            {
                leg.GetComponent<MeshRenderer>().material = Material;
            }
        }

        _destinationLegRotation = new Vector3(43, 30f, 0);
        ChangeLegDirection();

        this.GetComponent<Damageor>().OnDamaging += OnDamaging;

        HealthSystem healthSystem = this.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = this.gameObject.AddComponent<HealthSystem>();
        }
        healthSystem.IgnoreDamagees.AddRange(new string[] { "Rock", "Octorok", "Nose" });
        healthSystem.MaxHealth = MaxHealth;
        healthSystem.Health = MaxHealth;
        healthSystem.OnHealthChanged += OnHealthChanged;
        healthSystem.OnDeath += OnDeath;

        // UnityEngine.Debug.Log("health set to " + MaxHealth);
    }


    void FixedUpdate()
    {
        UpdatePosition();
        UpdateLegs();
        UpdateNose();
        FixRotation();
        Fire();
    }


    private void FixRotation()
    {
        const float DEGREE_LOCK = 90;

        Vector3 vv = Vector3.zero;
        vv.y = Mathf.Round(this.transform.eulerAngles.y / DEGREE_LOCK) * DEGREE_LOCK;
        this.transform.eulerAngles = vv;        
    }
    

    private void UpdatePosition()
    {
        _destinationTimeElapsed += Time.fixedDeltaTime;

        if (_destinationTimeElapsed >= WALK_LERP_TIME)
        {
            ChangePositionDestination();
        }
    }


    private void ChangePositionDestination()
    {
        int rand = Random.Range(1, 8);
        switch (rand)
        {
            case 1:
                this.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case 2:
                this.transform.localEulerAngles = new Vector3(0, 90, 0);
                break;
            case 3:
                this.transform.localEulerAngles = new Vector3(0, 180, 0);
                break;
            case 4:
                this.transform.localEulerAngles = new Vector3(0, 270, 0);
                break;
        }

        _originalPosition = this.transform.localPosition;
        _destinationPosition = this.transform.localPosition + (this.transform.forward * WalkSpeed);

        _destinationTimeElapsed = 0;

        this.GetComponent<Rigidbody>().velocity = this.transform.forward * WalkSpeed;
    }


    private void ForceChangePositionDestination()
    {
        Vector3 newAngle;

        do
        {
            int rand = Random.Range(1, 4);
            switch (rand)
            {
                case 1:
                    newAngle = new Vector3(0, 0, 0);
                    break;
                case 2:
                    newAngle = new Vector3(0, 90, 0);
                    break;
                case 3:
                    newAngle = new Vector3(0, 180, 0);
                    break;
                default:
                    newAngle = new Vector3(0, 270, 0);
                    break;
            }
        } while (this.transform.localEulerAngles == newAngle);

        this.transform.localEulerAngles = newAngle;

        _originalPosition = this.transform.localPosition;
        _destinationPosition = this.transform.localPosition + (this.transform.forward * WalkSpeed);

        _destinationTimeElapsed = 0;

        this.GetComponent<Rigidbody>().velocity = this.transform.forward * WalkSpeed;
    }


    private void UpdateNose()
    {
        _noseTimeElapsed += Time.fixedDeltaTime;

        _nose.localPosition = Vector3.Lerp(_originalNosePosition, _destinationNosePosition, _noseTimeElapsed / NOSE_LERP_TIME);

        if (_noseTimeElapsed >= NOSE_LERP_TIME)
        {
            ChangeNoseDirection();
        }
    }


    private void ChangeNoseDirection()
    {
        _originalNosePosition = _nose.localPosition;
        if (_originalNosePosition.z == 0.25f)
        {
            _destinationNosePosition = new Vector3(0, 0, 0.5f);
        }
        else
        {
            _destinationNosePosition = new Vector3(0, 0, 0.25f);
        }

        _noseTimeElapsed = 0;
    }


    private void UpdateLegs()
    {
        _legTimeElapsed += Time.fixedDeltaTime;

        foreach (Transform transform in _leftLegs)
        {
            transform.localEulerAngles = Vector3.Lerp(_originalLegRotation, _destinationLegRotation, _legTimeElapsed / LEG_LERP_TIME);
        }

        foreach (Transform transform in _rightLegs)
        {
            transform.localEulerAngles = Vector3.Lerp(_originalLegRotation, _destinationLegRotation, _legTimeElapsed / LEG_LERP_TIME);
        }

        if (_legTimeElapsed >= LEG_LERP_TIME)
        {
            ChangeLegDirection();
        }
    }


    private void ChangeLegDirection()
    {
        _originalLegRotation = _destinationLegRotation;
        if (_originalLegRotation.y == 30f)
        {
            _destinationLegRotation = new Vector3(43, -30f, 0);
        }
        else
        {
            _destinationLegRotation = new Vector3(43, 30f, 0);
        }

        _legTimeElapsed = 0;
    }


    private void Fire()
    {
        _nextFire += Time.fixedDeltaTime;
        if (_nextFire < FireSeconds)
        {
            return;
        }

        GameObject _projectile;
        _projectile = Instantiate(_rock, Vector3.zero, transform.rotation);
        _projectile.name = _rock.name;
        // _projectile.transform.SetParent(this.transform); //don't do this, it will rotate with the octorok
        // _projectile.transform.localPosition = Vector3.zero;
        _projectile.transform.position = this.transform.position;
        _projectile.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        _projectile.GetComponent<Rigidbody>().velocity = this.transform.forward * FireSpeed;
        _projectile.GetComponent<MeshRenderer>().enabled = true;
        _projectile.GetComponent<SelfDestruct>().enabled = true;

        Damageor damageor = _projectile.GetComponent<Damageor>();
        damageor.IgnoreObjects = _ignoreObjects;
        damageor.OnDamaging += OnDamaging;

        _nextFire = 0;
    }


    private void OnDamaging(GameObject damageor, GameObject damagee)
    {
        if (this.gameObject == null || damageor == this.gameObject)
        {
            return;
        }

        Destroy(damageor);
    }

     
    void OnCollisionEnter(Collision col) 
    {
        Tags tags = col.gameObject.GetComponent<Tags>();
        if (tags != null && tags.HasTag("environment"))
        {
            ForceChangePositionDestination();
            return;
        }
    }


    private void OnHealthChanged()
    {
        // UnityEngine.Debug.Log("Health at: " + this.GetComponent<HealthSystem>().Health.ToString());
    }


    private void OnDeath()
    {
        this.GetComponent<GoodieDropper>().Drop();
        UnityEngine.Debug.Log("killed.");
        Destroy(this.gameObject);
    }


}
