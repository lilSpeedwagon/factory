using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{
    public float MaxCameraSize = 7.5f;
    public float MinCameraSize = 1.0f;
    public float CursorOffsetToMoveScreen = 32;
    public Vector3 CameraSpeed = new Vector3(0.15f, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_zoomRange = new Range(MinCameraSize, MaxCameraSize);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        
        if (mousePos.x <= CursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= CameraSpeed;
        }
        if (mousePos.x >= Screen.width - CursorOffsetToMoveScreen)
        {
            gameObject.transform.position += CameraSpeed;
        }
        if (mousePos.y <= CursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= QuaternionUtils.qRotate90 * CameraSpeed;
        }
        if (mousePos.y >= Screen.height - CursorOffsetToMoveScreen)
        {
            gameObject.transform.position += QuaternionUtils.qRotate90 * CameraSpeed;
        }

        if (Input.GetKey(KeyCode.Mouse2))
        {
            var mouseMovement = mousePos - m_prevMousePos;
            gameObject.transform.position += mouseMovement * CameraSpeed.magnitude;
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
    }
    
    private Camera m_camera;
    private Range m_zoomRange;
    private Vector3 m_prevMousePos;
}
