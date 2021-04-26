using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MotionScript : MonoBehaviour
{
    public bool IsFinished { get; private set; } = true;

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public bool IsStarted => m_startTime != TimeUndefined;

    public void StartMotion(Vector2 to, float speed)
    {
        m_startTime = Time.time;
        m_from = transform.position;
        m_to = to;
        m_trajectory = new List<Vector2> { to };
        m_distance = Vector2.Distance(to, transform.position);
        m_localDistance = m_distance;
        m_speed = speed;
        IsFinished = false;
    }


    // trajectory must not include starting point
    public void StartMotion(List<Vector2> trajectory, float speed)
    {
        if (trajectory.Count == 0) return;

        m_startTime = Time.time;
        m_trajectory = new List<Vector2>(trajectory);
        m_currentPointIndex = 0;
        m_from = transform.position;
        m_to = m_trajectory.First();

        m_localDistance = Vector2.Distance(m_from, m_to);
        m_distance = 0;
        var previousPoint = m_from;
        foreach (var point in m_trajectory)
        {
            m_distance += Vector2.Distance(point, previousPoint);
            previousPoint = point;
        }

        m_speed = speed;
        IsFinished = false;
    }

    public void Stop()
    {
        IsFinished = true;
    }

    public float Progress
    {
        get
        {
            if (!IsStarted)
                return ProgressNotStarted;

            if (IsFinished)
                return ProgressFinished;

            float distCovered = (Time.time - m_startTime) * m_speed;
            return distCovered / m_distance;
        }
    }
    
    private void Update()
    {
        if (!IsFinished)
        {
            float distCovered = (Time.time - m_startTime) * m_speed;
            transform.position = Vector2.Lerp(m_from, m_to, distCovered / m_localDistance);
            if (transform.position.Equals(m_to))
            {
                m_currentPointIndex++;
                if (m_currentPointIndex >= m_trajectory.Count)
                {
                    IsFinished = true;
                }
                else
                {
                    m_from = m_to;
                    m_to = m_trajectory[m_currentPointIndex];
                    m_localDistance = Vector2.Distance(m_to, m_from);
                    m_startTime = Time.time;
                }
            }
        }
    }

    private Vector2 m_from, m_to;
    private List<Vector2> m_trajectory;
    private int m_currentPointIndex = -1;
    private float m_startTime = TimeUndefined;
    private float m_distance;
    private float m_localDistance;
    private float m_speed;

    private const float TimeUndefined = -1.0f;
    private const float ProgressNotStarted = 0.0f;
    private const float ProgressFinished = 1.0f;
}
