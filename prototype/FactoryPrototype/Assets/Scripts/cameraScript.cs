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
        var vCursorCoords = Input.mousePosition;
        var vCurrentCameraPos = gameObject.transform.position;
        
        if (vCursorCoords.x <= CursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= CameraSpeed;
        }
        if (vCursorCoords.x >= Screen.width - CursorOffsetToMoveScreen)
        {
            gameObject.transform.position += CameraSpeed;
        }
        if (vCursorCoords.y <= CursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= QuaternionUtils.qRotate90 * CameraSpeed;
        }
        if (vCursorCoords.y >= Screen.height - CursorOffsetToMoveScreen)
        {
            gameObject.transform.position += QuaternionUtils.qRotate90 * CameraSpeed;
        }

        zoomCamera(Input.mouseScrollDelta.y);
        //m_camera.orthographicSize += Input.mouseScrollDelta.y;
    }

    void zoomCamera(float delta)
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
}
