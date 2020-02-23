using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyerScript : tileObjectScript
{
    public float fConvSpeed = 0.2f;

    ContactFilter2D m_filter = new ContactFilter2D();
    Vector2 m_vDirection;

    // Start is called before the first frame update
    void Start()
    {
        m_vDirection = GetVector();   
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
                    var detailSpeed = fConvSpeed * m_vDirection;
                    collision.transform.Translate(detailSpeed);
                }
            }
        }

        
    }
}
