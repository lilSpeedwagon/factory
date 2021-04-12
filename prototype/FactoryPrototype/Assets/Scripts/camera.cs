using System;
using UnityEngine;

public class camera : MonoBehaviour
{
    public float MaxCameraSize = 7.5f;
    public float MinCameraSize = 1.0f;
    public float CursorOffsetToMoveScreen = 32;
    public Vector3 CameraSpeed = new Vector3(0.4f, 0, 0);

    public static void SetLayerVisibility(string layerName, bool visible)
    {
        Camera camera = Camera.main;

        if (camera != null)
        {
            if (visible)
            {
                camera.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
            }
            else
            {
                camera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
            }
        }
    }

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
        Vector3 newPosition = gameObject.transform.position;
        
        if (mousePos.x <= CursorOffsetToMoveScreen)
        {
            newPosition -= m_currentSpeed;
        }
        if (mousePos.x >= Screen.width - CursorOffsetToMoveScreen)
        {
            newPosition += m_currentSpeed;
        }
        if (mousePos.y <= CursorOffsetToMoveScreen)
        {
            newPosition -= QuaternionUtils.qRotate90 * m_currentSpeed;
        }
        if (mousePos.y >= Screen.height - CursorOffsetToMoveScreen)
        {
            newPosition += QuaternionUtils.qRotate90 * m_currentSpeed;
        }

        if (Input.GetKey(KeyCode.Mouse2))
        {
            Vector3 mouseMovement = mousePos - m_prevMousePos;
            newPosition -= mouseMovement * m_currentSpeed.magnitude;
        }

        if (TileManagerScript.TileManager.IsValidCoords(newPosition))
        {
            gameObject.transform.position = newPosition;
        }
        m_prevMousePos = mousePos;
    }

    void Update()
    {
        if (!MenuManager.Manager.IsCameraZoomAllowed) return;

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
    private Range m_horizontalLimit, m_verticalLimit;
}
