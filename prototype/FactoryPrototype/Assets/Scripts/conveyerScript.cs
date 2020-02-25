using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyerScript : tileObjectScript
{
    public float Speed = 0.2f;



    // Start is called before the first frame update
    void Start()
    {
        m_vDirection = GetVector();
    }

    private void UpdateLine()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isShadow)
        {
            var collisions = new List<Collider2D>();
            var collider = GetComponent<Collider2D>();
            collider.OverlapCollider(m_filter, collisions);

            foreach (var collision in collisions)
            {
                if (collision.gameObject.tag.Equals("detail"))
                {
                    try
                    {
                        var motion = collision.GetComponent<MotionScript>();
                        if (motion.IsFinished)
                        {
                            Vector2 target = m_vDirection * TileUtils.tileSize + (Vector2)transform.position;
                            motion.StartMotion(target, Speed);
                        }
                    }
                    catch (System.NullReferenceException e)
                    {
                        Debug.LogError(e.Message);
                    }
                    //collision.transform.Translate(detailSpeed * Time.fixedDeltaTime);
                }
            }
        }

        
    }

    private conveyerScript m_prev, m_next;
    private ContactFilter2D m_filter = new ContactFilter2D();
    private Vector2 m_vDirection;
}
