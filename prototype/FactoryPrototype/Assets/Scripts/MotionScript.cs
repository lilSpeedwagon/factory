using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionScript : MonoBehaviour
{
    public bool IsFinished { get; private set; }

    public void StartMotion(Vector2 to, float speed)
    {
        m_startTime = Time.time;
        m_from = transform.position;
        m_to = to;
        m_distantion = Vector2.Distance(to, transform.position);
        m_speed = speed;
        IsFinished = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        IsFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFinished)
        {
            float distCovered = (Time.time - m_startTime) * m_speed;
            transform.position = Vector2.Lerp(m_from, m_to, distCovered / m_distantion);
            if (transform.position.Equals(m_to))
            {
                IsFinished = true;
            }
        }
    }

    private Vector2 m_from, m_to;
    private float m_startTime;
    private float m_distantion;
    private float m_speed;
}
