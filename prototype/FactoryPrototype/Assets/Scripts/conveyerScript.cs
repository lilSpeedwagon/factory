using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    void FixedUpdate()
    {
        /*var collisions = new List<Collider2D>();
        var collider = GetComponent<Collider2D>();
        collider.OverlapCollider(m_filter, collisions);

        foreach (var collision in collisions.Where(collision => collision.gameObject.tag.Equals("detail")))
        { 
            try
            {
                var obj = collision.GetComponent<MotionScript>();
                if (obj != null && m_currentObject == null)
                    m_currentObject = obj;
            }
            catch (System.NullReferenceException e)
            {
                Debug.LogError(e.Message);
            }
        }*/

        if (m_currentObject != null)
            Move(m_currentObject);
    }
    
    private Vector2 m_vDirection;
    private MotionScript m_currentObject;
}
