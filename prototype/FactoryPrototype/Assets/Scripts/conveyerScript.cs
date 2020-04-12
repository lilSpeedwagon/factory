using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class conveyerScript : tileObjectScript, IMover
{
    public float Speed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        m_vDirection = GetVector();
    }

    public IMover Next => TileManagerScript.TileManager.GetGameObject(GetNextPostion())?.GetComponent<IMover>();

    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished && IsAbleToMove())
        {
            m_currentObject = null;
            motionObject.StartMotion(GetNextPostion(), Speed);
            Next.HoldMotion(motionObject);
        }
    }

    public bool IsFree()
    {
        return m_currentObject == null;
    }

    public void HoldMotion(MotionScript obj)
    {
        m_currentObject = obj;
    }

    public bool IsAbleToMove()
    {
        var mover = Next;
        return mover != null && mover.IsDirectionAllowed(m_dir) && mover.IsFree();
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return true;
    }

    // Update is called once per frame
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
    
    private Vector2 m_vDirection;
    private MotionScript m_currentObject;
}
