using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Tektite : MonoBehaviour
{


    public Material Material;
    public float MaxHealth = 0.5f;


    private const float MIN_JUMP_SPEED = 1f;
    private const float MAX_JUMP_SPEED = 5f;
    private const float JUMP_ANGLE_TIME = 0.125f;
    private const float COAST_ANGLE_TIME = 1.0f;
    private const float RESET_ANGLE_TIME = 0.5f;
    private const float BOUNCE_ANGLE_TIME = 0.25f;


    private enum MOTION_STATE
    {
        RESET = 0,
        JUMP = 1,
        BOUNCE = 2,
    }


    private Dictionary<GameObject, Vector3> _hips = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> _knees = new Dictionary<GameObject, Vector3>();
    private Vector3 _lastPosition;
    private float _hipTargetX;
    private float _kneeTargetX;
    private bool _extend = false;
    private MOTION_STATE _state = MOTION_STATE.RESET;
    private float _stateTime;
    private float _bendTime;
    private float _motionTime;


    void Start()
    {
        SetupMaterial();
        SetupLegs();
        SetupHealth();

        //Tektite needs double gravity to move faster
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        ConstantForce gravity = gameObject.AddComponent<ConstantForce>();
        gravity.force = new Vector3(0.0f, -9.81f * 2, 0.0f);
    }


    private void SetupMaterial()
    {
        if (Material != null)
        {
            foreach (GameObject desendant in this.gameObject.GetAllChildren(true).Where(go => !go.name.StartsWith("Eye")))
            {
                if (desendant.HasComponent<MeshRenderer>())
                {
                    desendant.GetComponent<MeshRenderer>().material = Material;
                }
            }
        }
    }


    private void SetupLegs()
    {
        foreach (GameObject go in this.gameObject.GetAllChildren(true))
        {
            if (go.name == "Hip")
            {
                _hips.Add(go, go.transform.localEulerAngles);
            }
            else if (go.name == "Knee")
            {
                _knees.Add(go, go.transform.localEulerAngles);
            }
        }
    }


    private void SetupHealth()
    {
        HealthSystem healthSystem = this.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = this.gameObject.AddComponent<HealthSystem>();
        }
        healthSystem.IgnoreDamagees.AddRange(new string[] { "Tektite" });
        healthSystem.MaxHealth = MaxHealth;
        healthSystem.Health = MaxHealth;
        healthSystem.OnHealthChanged += OnHealthChanged;
        healthSystem.OnDeath += OnDeath;        
    }


    private void ChangePositionDestination()
    {
        ResetInitialState();

        if (_state != MOTION_STATE.RESET)
        {
            ApplyResetState();
        }
        else
        {
            MOTION_STATE rand = (MOTION_STATE)Random.Range(1, 3);
            switch (rand)
            {
                case MOTION_STATE.JUMP:
                    ApplyJumpState();
                    break;
                case MOTION_STATE.BOUNCE:
                    ApplyBouncState();
                    break;
            }

        }
    }


    private void ResetInitialState()
    {
        _lastPosition = this.transform.position;

        foreach (GameObject go in _hips.Keys.ToList())
        {
            _hips[go] = go.transform.localEulerAngles;
        }

        foreach (GameObject go in _knees.Keys.ToList())
        {
            _knees[go] = go.transform.localEulerAngles;
        }

        _stateTime = 0;
    }


    private void ApplyResetState()
    {
        _state = MOTION_STATE.RESET;
        _hipTargetX = 0;
        _kneeTargetX = 0;
        _bendTime = RESET_ANGLE_TIME;
        _motionTime = _bendTime + 0.25f;
    }


    private void ApplyJumpState()
    {
        _state = MOTION_STATE.JUMP;
        _hipTargetX = -130;
        _kneeTargetX = 130;
        _bendTime = JUMP_ANGLE_TIME;
        _motionTime = _bendTime + COAST_ANGLE_TIME;

        float jumpSpeed = Random.Range(MIN_JUMP_SPEED, MAX_JUMP_SPEED);
        float angle = Random.Range(0, 360);
        Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(jumpSpeed, 0, 0);

        this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 15, 0), ForceMode.Impulse);
        this.GetComponent<Rigidbody>().AddForce(rotatedVector, ForceMode.Impulse);
    }


    private void ApplyBouncState()
    {
        _state = MOTION_STATE.BOUNCE;
        _hipTargetX = -90;
        _kneeTargetX = 90;
        _bendTime = BOUNCE_ANGLE_TIME;
        _motionTime = _bendTime;
    }


    void FixedUpdate()
    {
        UpateRotation();
    }


    private void UpateRotation()
    {
        _stateTime += Time.fixedDeltaTime;

        foreach (GameObject go in _hips.Keys.ToList())
        {
            go.transform.localEulerAngles = new Vector3(Mathf.LerpAngle(_hips[go].x, _hipTargetX, _stateTime / _bendTime), 0, 0);
        }

        foreach (GameObject go in _knees.Keys.ToList())
        {
            go.transform.localEulerAngles = new Vector3(Mathf.LerpAngle(_knees[go].x, _kneeTargetX, _stateTime / _bendTime), 0, 0);
        }

        if (_stateTime >= _motionTime)
        {
            ChangePositionDestination();
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
