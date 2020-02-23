using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{
    private static float cursorOffsetToMoveScreen = 32;
    private static Vector3 vCameraSpeed = new Vector3(0.05f, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var vCursorCoords = Input.mousePosition;
        var vCurrentCameraPos = gameObject.transform.position;
        
        if (vCursorCoords.x <= cursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= vCameraSpeed;
        }
        if (vCursorCoords.x >= Screen.width - cursorOffsetToMoveScreen)
        {
            gameObject.transform.position += vCameraSpeed;
        }
        if (vCursorCoords.y <= cursorOffsetToMoveScreen)
        {
            gameObject.transform.position -= QuaternionUtils.qRotate90 * vCameraSpeed;
        }
        if (vCursorCoords.y >= Screen.height - cursorOffsetToMoveScreen)
        {
            gameObject.transform.position += QuaternionUtils.qRotate90 * vCameraSpeed;
        }
    }
}
