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

    public GameObject Next => TileManagerScript.TileManager.GetGameObject(GetNextPostion());

    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished)
        {
            motionObject.StartMotion(GetNextPostion(), Speed);
        }
    }

    public bool IsAbleToMove()
    {
        var next = Next;
        if (next != null)
        {
            var mover = next.GetComponent<IMover>();
            if (mover != null && mover.IsDirectionAllowed(m_dir))
                return true;
        }
        return false;
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isShadow)
        {
            var collisions = new List<Collider2D>();
            var collider = GetComponent<Collider2D>();
            collider.OverlapCollider(m_filter, collisions);

            foreach (var collision in collisions.Where(collision => collision.gameObject.tag.Equals("detail")))
            {
                try
                {
                    var obj = collision.GetComponent<MotionScript>();
                    if (IsAbleToMove())
                        Move(obj);
                }
                catch (System.NullReferenceException e)
                {
                    Debug.LogError(e.Message);
                }
                //collision.transform.Translate(detailSpeed * Time.fixedDeltaTime);
            }
        }

        
    }
    
    private Vector2 m_vDirection;
}
