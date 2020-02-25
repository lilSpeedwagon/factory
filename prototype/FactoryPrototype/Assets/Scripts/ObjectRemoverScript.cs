using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRemoverScript : MonoBehaviour
{
    public TileManagerScript tileManager;
    public GameObject shadowPrefab;
    public GameObject builderPanel; 

    bool isActive = false;
    GameObject shadow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (shadow != null)
            {
                shadow.transform.position = TileUtils.MouseCellPosition();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }

            
            if (Input.GetMouseButton(MouseUtils.PRIMARY_MOUSE_BUTTON))
            {
                try
                {
                    tileManager.RemoveObject(shadow.transform.position);
                }
                catch(System.Exception)
                {
                    Debug.Log("Nothing to remove");
                }
            }
        }
        
    }

    void Disable()
    {
        isActive = false;
        Destroy(shadow);
        shadow = null;
        builderPanel.SetActive(true);
    }

    void CreateShadow()
    {
        if (shadowPrefab != null)
        {
            if (shadow != null)
            {
                Destroy(shadow);
            }

            shadow = Instantiate(shadowPrefab, TileUtils.MouseCellPosition(), TileUtils.qInitRotation);
        }
    }

    public void Pick()
    {
        isActive = true;
        CreateShadow();
        builderPanel.SetActive(false);
    }
}
