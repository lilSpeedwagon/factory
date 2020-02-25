using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectBuilder : MonoBehaviour
{
    public List<GameObject> prefabs;
    public GameObject prefabToCreate = null;
    public GameObject builderPanel;
    public TileManagerScript tileManager;

    GameObject grid;
    GridLayout gridLayout;
    GameObject shadow;
        
    bool isActive = false;

    bool IsPossibleToCreate
    {
        get
        {
            if (!isActive)
                return false;

             return tileManager.IsEmpty(TileUtils.MouseCellPosition());

            /*var collisions = new List<Collider2D>();
            var collider = shadow.GetComponent<PolygonCollider2D>();
            collider.OverlapCollider(filter, collisions);

            foreach (var collision in collisions)
            {
                if (!collision.gameObject.Equals(collider.gameObject) && collision.gameObject.tag == "object")
                {
                    return false;
                }
            }

            return true;*/
        }
    }

    void RotateShadow(bool bClockwise = true)
    {
        if (shadow == null)
            return;

        shadow.GetComponent<tileObjectScript>().Rotate(bClockwise);
    }

    void CreateShadow()
    {
        if (prefabToCreate != null)
        {
            if (shadow != null)
            {
                Destroy(shadow);
            }

            shadow = Instantiate(prefabToCreate, TileUtils.MouseCellPosition(), TileUtils.qInitRotation);
            shadow.GetComponent<SpriteRenderer>().color = ColorUtils.colorTransparentGreen;
            shadow.GetComponent<SpriteRenderer>().sortingLayerName = "shadow";
            shadow.GetComponent<PolygonCollider2D>().isTrigger = true;
            shadow.GetComponent<tileObjectScript>().isShadow = true;
        }
    }

    void CreateObject()
    {
        if (isActive && prefabToCreate != null)
        {
            GameObject newObj = tileManager.InstantiateObject(prefabToCreate, TileUtils.MouseCellPosition());
            newObj.GetComponent<tileObjectScript>().direction = shadow.GetComponent<tileObjectScript>().direction;
        }
    }

    void ChangePrefab(GameObject p)
    {
        isActive = true;
        prefabToCreate = p;
        CreateShadow();
    }

    void Disable()
    {
        isActive = false;
        prefabToCreate = null;
        Destroy(shadow);
        shadow = null;
        builderPanel.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.FindWithTag("grid");
        if (grid != null)
        {
            gridLayout = grid.GetComponent<GridLayout>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            bool bIsPossibleToCreate = IsPossibleToCreate;

            if (shadow != null)
            {
                shadow.transform.position = TileUtils.MouseCellPosition();
                shadow.GetComponent<SpriteRenderer>().color = bIsPossibleToCreate ? ColorUtils.colorTransparentGreen : ColorUtils.colorTransparentRed;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateShadow(true);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateShadow(false);
            }                   

            if (Input.GetMouseButton(MouseUtils.PRIMARY_MOUSE_BUTTON) && bIsPossibleToCreate)
            {
                CreateObject();
            }
        }        
    }

    public void Pick(int prefabId)
    {
        var prefab = prefabs[prefabId];

        if (prefab != null)
        {
            ChangePrefab(prefab);
            builderPanel.SetActive(false);
        }
    }
}
