using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class conveyerScript : tileObjectScript, IMover
{
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
            Vector2 position = IsReversed ? GetPrevPostion() : GetNextPostion();
            return TileManagerScript.TileManager.GetGameObject(position)?.GetComponent<IMover>();
        }
    }

    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished && IsAbleToMove())
        {
            m_currentObject = null;
            Vector2 position = IsReversed ? GetPrevPostion() : GetNextPostion();
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

    private void Start()
    {
        m_direction = GetComponent<tileObjectScript>()?.direction ?? new TileUtils.Direction();

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
        if (m_currentObject != null)
            Move(m_currentObject);
    }

    private void OnDestroy()
    {
        if (m_currentObject != null)
            Destroy(m_currentObject.gameObject);
    }

    private TileUtils.Direction m_direction;
    private MotionScript m_currentObject;
    
    private DataPublisher.DataPort m_directionPort;
    private DataPublisher.DataPort m_speedPort;
}
