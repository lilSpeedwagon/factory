using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Conveyer : MonoBehaviour, IMover
{
    public static float ConveyerHeight = 0.12f;

    public float BaseSpeed;
    public float Speed
    {
        get
        {
            if (m_speedPort != null && m_speedPort.IsPublisher)
            {
                return m_speedPort.CurrentValue.GetNumber();
            }

            return BaseSpeed;
        }
    }

    public IMover Next
    {
        get
        {
            Vector2 position = IsReversed ? m_tileComponent.GetPrevPosition() : m_tileComponent.GetNextPosition();
            return TileManagerScript.TileManager.GetGameObject(position)?.GetComponent<IMover>();
        }
    }

    public float Height => ConveyerHeight;

    public MotionScript ReleaseMotion()
    {
        const float halfFinished = 0.5f;
        if (m_currentObject?.Progress >= halfFinished)
        {
            var motion = m_currentObject;
            m_currentObject = null;
            return motion;
        }

        return null;
    }

    public void Move(MotionScript motionObject)
    {
        if (motionObject != null && motionObject.IsFinished && IsAbleToMove())
        {
            m_currentObject = null;
            Vector2 position = IsReversed ? m_tileComponent.GetPrevPosition() : m_tileComponent.GetNextPosition();
            position.y += Next.Height;
            motionObject.StartMotion(position, Speed);
            Next.HoldMotion(motionObject);
        }
    }

    public bool IsReversed
    {
        get
        {
            if (m_directionPort != null && m_directionPort.IsPublisher)
            {
                return m_directionPort.CurrentValue.GetBool();
            }

            return false;
        }
    }

    public bool IsFree()
    {
        return m_currentObject == null;
    }

    public bool IsAbleToMove()
    {
        var mover = Next;
        if (mover == null)
        {
            return false;
        }

        TileUtils.Direction dir = IsReversed ? TileUtils.GetReversedDirection(m_direction) : m_direction;
        return mover.IsDirectionAllowed(dir) && mover.IsFree();
    }

    public bool IsDirectionAllowed(TileUtils.Direction dir)
    {
        return true;
    }

    public void HoldMotion(MotionScript obj)
    {
        m_currentObject = obj;
    }

    public bool IsEnabled => m_consumerComponent.IsEnergized;

    private void Start()
    {
        m_tileComponent = GetComponent<TileObject>();
        m_direction = m_tileComponent.Direction;

        m_consumerComponent = GetComponent<EnergyConsumer>();

        m_animator = GetComponent<Animator>();
        if (m_animator != null)
        {
            m_animator.enabled = false;
            m_animationEnabled = false;
        }
        else
        {
            Debug.LogError("Animator is not found for conveyer.");
        }

        DataPublisher publisher = GetComponent<DataPublisher>();
        if (publisher == null)
        {
            Debug.LogWarning("There is no publisher attached to conveyer object!");
            return;
        }

        m_directionPort = new DataPublisher.DataPort("IsReversed");
        m_speedPort = new DataPublisher.DataPort("Speed");

        publisher.SetPort(m_directionPort);
        publisher.SetPort(m_speedPort);
    }

    private void FixedUpdate()
    {
        bool isEnabled = IsEnabled;
        AnimationEnabled = isEnabled;

        if (isEnabled)
        {
            AnimationSpeed = Speed;
            Move(m_currentObject);
        }
    }

    private void OnDestroy()
    {
        if (m_currentObject != null)
            Destroy(m_currentObject.gameObject);
    }

    private bool AnimationEnabled
    {
        get => m_animationEnabled;
        set
        {
            // do not invoke animator setters if nothing was changed
            if (m_animationEnabled == value) return;

            m_animator.enabled = value;
            m_animationEnabled = value;
        }
    }

    private float AnimationSpeed
    {
        set => m_animator.speed = value / BaseSpeed;
        get => m_animator.speed;
    }

    private TileObject m_tileComponent;
    private TileUtils.Direction m_direction;
    private MotionScript m_currentObject;

    private EnergyConsumer m_consumerComponent;

    private bool m_animationEnabled;
    private Animator m_animator;
    
    private DataPublisher.DataPort m_directionPort;
    private DataPublisher.DataPort m_speedPort;
}
