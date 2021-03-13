using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Damageor))]
public class Octorok : MonoBehaviour
{


    public float FireSeconds = 2.0f;
    public float FireSpeed = 10f;


    private const float NOSE_LERP_TIME = 0.25f;
    private const float LEG_LERP_TIME = 0.4f;


    private Transform _nose;

    private List<Transform> _leftLegs;
    private List<Transform> _rightLegs;
    private Vector3 _originalLegRotation;
    private Vector3 _destinationLegRotation;
    private float _legTimeElapsed;

    private GameObject _rock;
    private GameObject _projectile;
    private float _nextFire;

    private Vector3 _originalNosePosition;
    private Vector3 _destinationNosePosition;
    private float _noseTimeElapsed;

    private List<GameObject> _ignoreObjects;


    void Start()
    {
        _ignoreObjects = new List<GameObject>();

        _nose = this.transform.Find("Nose");
        ChangeNoseDirection();

        _rock = this.transform.Find("Rock").gameObject;
        _nextFire = 0;

        _ignoreObjects.Add(this.gameObject);
        _ignoreObjects.Add(_nose.gameObject);
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

        _destinationLegRotation = new Vector3(43, 30f, 0);
        ChangeLegDirection();

        this.GetComponent<Damageor>().OnDamaging += OnDamaging;
    }


    void FixedUpdate()
    {
        UpdateLegs();
        UpdateNose();
        Fire();
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

        _projectile = Instantiate(_rock, Vector3.zero, transform.rotation);
        _projectile.transform.SetParent(this.transform);
        _projectile.transform.localPosition = Vector3.zero;
        _projectile.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        _projectile.GetComponent<Rigidbody>().velocity = this.transform.forward * FireSpeed;

        Damageor damageor = _projectile.GetComponent<Damageor>();
        damageor.IgnoreObjects = _ignoreObjects;
        damageor.OnDamaging += OnDamaging;

        _nextFire = 0;
    }


    private void OnDamaging(GameObject damageor, GameObject damagee)
    {
        if (damageor == this.gameObject)
        {
            return;
        }


        Destroy(damageor);
    }


}
