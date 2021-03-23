using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro; // Add the TextMesh Pro namespace to access the various functions.
using System.Linq;


public class HandTracking : MonoBehaviour
{


    public enum HAND_STATE
    {
        OPEN,
        CLOSED,
        PINCH,
        POINT,
        GUN,
        THUMBS_UP
    }


    public delegate void HandStateChangedEvent(HandTracking sender);


    public HandStateChangedEvent OnHandStateChanged = null;


    public XRController controller = null;
    public Animator m_animator = null;


    public const string ANIM_LAYER_NAME_POINT = "Point Layer";
    public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";
    public const string ANIM_PARAM_NAME_FLEX = "Flex";
    public const string ANIM_PARAM_NAME_POSE = "Pose";


    private int m_animLayerIndexThumb = -1;
    private int m_animLayerIndexPoint = -1;
    private int m_animParamIndexFlex = -1;
    private int m_animParamIndexPose = -1;
    private Collider[] m_colliders = null;


    public float anim_frames = 4f;
    private float grip_state = 0f;
    private float trigger_state = 0f;
    private float triggerCap_state = 0f;
    private float thumbCap_state = 0f;

    private HAND_STATE _handState = HAND_STATE.OPEN;
    private bool _handClosed = false;
    private bool _thumbUp = false;
    private bool _indexOut = false;


    private void Awake()
    {
    }


    void Start()
    {
        m_colliders = this.GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
        for (int i = 0; i < m_colliders.Length; ++i)
        {
            Collider collider = m_colliders[i];
            collider.enabled = true;
        }
        m_animLayerIndexPoint = m_animator.GetLayerIndex(ANIM_LAYER_NAME_POINT);
        m_animLayerIndexThumb = m_animator.GetLayerIndex(ANIM_LAYER_NAME_THUMB);
        m_animParamIndexFlex = Animator.StringToHash(ANIM_PARAM_NAME_FLEX);
        m_animParamIndexPose = Animator.StringToHash(ANIM_PARAM_NAME_POSE);
    }


    void Update()
    {
        UpdateHand();
        UpdateTrigger();
        UpdateIndex();
        UpdateThumb();
        UpdateState();
    }


    private void UpdateHand()
    {
        if (!controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripTarget)) return;

        float grip_state_delta = gripTarget - grip_state;
        if (grip_state_delta > 0f)
        {
            grip_state = Mathf.Clamp(grip_state + 1/anim_frames, 0f, gripTarget);
        }
        else if (grip_state_delta < 0f)
        {
            grip_state = Mathf.Clamp(grip_state - 1/anim_frames, gripTarget, 1f);
        }
        else
        {
            grip_state = gripTarget;
        }

        m_animator.SetFloat(m_animParamIndexFlex, grip_state);
    }


    private void UpdateTrigger()
    {
        if (!controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerTarget)) return;

        float trigger_state_delta = triggerTarget - trigger_state;
        if (trigger_state_delta > 0f)
        {
            trigger_state = Mathf.Clamp(trigger_state + 1/anim_frames, 0f, triggerTarget);
        }
        else if (trigger_state_delta < 0f)
        {
            trigger_state = Mathf.Clamp(trigger_state - 1/anim_frames, triggerTarget, 1f);
        }
        else
        {
            trigger_state = triggerTarget;
        }

        m_animator.SetFloat("Pinch", trigger_state);
    }


    private void UpdateIndex()
    {
        if (!controller.inputDevice.TryGetFeatureValue(CommonUsages.indexTouch, out float triggerCapTarget)) return;

        float triggerCap_state_delta = triggerCapTarget - triggerCap_state;
        if (triggerCap_state_delta > 0f)
        {
            triggerCap_state = Mathf.Clamp(triggerCap_state + 1/anim_frames, 0f, triggerCapTarget);
        }
        else if (triggerCap_state_delta < 0f)
        {
            triggerCap_state = Mathf.Clamp(triggerCap_state - 1/anim_frames, triggerCapTarget, 1f);
        }
        else
        {
            triggerCap_state = triggerCapTarget;
        }
        m_animator.SetLayerWeight(m_animLayerIndexPoint, 1f-triggerCap_state);
    }


    private void UpdateThumb()
    {
        if (!controller.inputDevice.TryGetFeatureValue(CommonUsages.thumbTouch, out float thumbCapTarget)) return;

        float thumbCap_state_delta = thumbCapTarget - thumbCap_state;
        if (thumbCap_state_delta > 0f)
        {
            thumbCap_state = Mathf.Clamp(thumbCap_state + 1/anim_frames, 0f, thumbCapTarget);
        }
        else if (thumbCap_state_delta < 0f)
        {
            thumbCap_state = Mathf.Clamp(thumbCap_state - 1/anim_frames, thumbCapTarget, 1f);
        }
        else
        {
            thumbCap_state = thumbCapTarget;
        }
        m_animator.SetLayerWeight(m_animLayerIndexThumb, 1f-thumbCap_state);
    }


    private void UpdateState()
    {
        _handClosed = (grip_state > 0f);
        _thumbUp = (thumbCap_state <= 0);
        _indexOut = (triggerCap_state <= 0);

        HAND_STATE newState;
        if (grip_state > 0f)
        {
            if (_thumbUp)
            {
                newState = _indexOut ? HAND_STATE.GUN : HAND_STATE.THUMBS_UP;
            }
            else
            {
                newState = _indexOut ? HAND_STATE.POINT : HAND_STATE.CLOSED;
            }
        }
        else if (!_thumbUp && !_indexOut)
        {
            newState = HAND_STATE.PINCH;
        }
        else
        {
            newState = HAND_STATE.OPEN;
        }

        if (_handState != newState)
        {
            _handState = newState;
            OnHandStateChanged?.Invoke(this);
        }
    }


    public HAND_STATE State
    {
        get
        {
            return _handState;
        }
    }


    public bool IsHandClosed
    {
        get
        {
            return _handClosed;
        }
    }
  

    public bool IsThumbUp
    {
        get
        {
            return _thumbUp;
        }
    }


    public bool IsIndexOut
    {
        get
        {
            return _indexOut;
        }
    }
  

}