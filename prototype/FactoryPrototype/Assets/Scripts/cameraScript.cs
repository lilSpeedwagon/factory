using System;
using UnityEngine;

public class cameraScript : MonoBehaviour
{
    public float MaxCameraSize = 7.5f;
    public float MinCameraSize = 1.0f;
    public float CursorOffsetToMoveScreen = 32;
    public Vector3 CameraSpeed = new Vector3(0.4f, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_zoomRange = new Range(MinCameraSize, MaxCameraSize);
        CalculateCurrentSpeed();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        
        if (mousePos.x <= CursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= m_currentSpeed;
        }
        if (mousePos.x >= Screen.width - CursorOffsetToMoveScreen)
        {
            gameObject.transform.position += m_currentSpeed;
        }
        if (mousePos.y <= CursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= QuaternionUtils.qRotate90 * m_currentSpeed;
        }
        if (mousePos.y >= Screen.height - CursorOffsetToMoveScreen)
        {
            gameObject.transform.position += QuaternionUtils.qRotate90 * m_currentSpeed;
        }

        if (Input.GetKey(KeyCode.Mouse2))
        {
            var mouseMovement = mousePos - m_prevMousePos;
            gameObject.transform.position -= mouseMovement * m_currentSpeed.magnitude;
        }
        m_prevMousePos = mousePos;
    }

    void Update()
    {
        var scroll = Input.mouseScrollDelta.y;
        if (Math.Abs(scroll) > 0.1f)
            ZoomCamera(scroll);
    }

    void ZoomCamera(float delta)
    {
        m_camera.orthographicSize -= delta;

        if (m_zoomRange.More(m_camera.orthographicSize))
        {
            m_camera.orthographicSize = MaxCameraSize;
        }

        if (m_zoomRange.Less(m_camera.orthographicSize))
        {
            m_camera.orthographicSize = MinCameraSize;
        }

        CalculateCurrentSpeed();
    }

    void CalculateCurrentSpeed()
    {
        m_currentSpeed = CameraSpeed * m_camera.orthographicSize / MinCameraSize;
    }
    
    private Camera m_camera;
    private Vector3 m_currentSpeed;
    private Range m_zoomRange;
    private Vector3 m_prevMousePos;
}
