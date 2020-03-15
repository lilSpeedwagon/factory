using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class BuildableObject
{
    public int Cost;
    public GameObject Prefab;
    public Sprite Image;
}

public class objectBuilder : MonoBehaviour
{
    public List<BuildableObject> Objects;

    public GameObject PrefabToCreate = null;
    public GameObject BuilderPanel;
    public GameObject ButtonPrefab;
    public int ButtonsMargin = 15;

    bool IsPossibleToCreate => m_isActive && m_tileManager.IsEmpty(TileUtils.MouseCellPosition());

    void RotateShadow(bool bClockwise = true)
    {
        if (m_shadow == null)
            return;

        m_shadow.GetComponent<tileObjectScript>().Rotate(bClockwise);
    }

    void CreateShadow()
    {
        if (PrefabToCreate != null)
        {
            if (m_shadow != null)
            {
                Destroy(m_shadow);
            }

            m_shadow = Instantiate(PrefabToCreate, TileUtils.MouseCellPosition() + TileUtils.LevelOffset(m_currentZlevel), TileUtils.qInitRotation);
            m_shadow.GetComponent<SpriteRenderer>().color = ColorUtils.colorTransparentGreen;
            m_shadow.GetComponent<SpriteRenderer>().sortingLayerName = "shadow";
            m_shadow.GetComponent<PolygonCollider2D>().isTrigger = true;
            m_shadow.GetComponent<tileObjectScript>().isShadow = true;
        }
    }

    void CreateObject()
    {
        if (m_isActive && PrefabToCreate != null)
        {
            GameObject newObj = m_tileManager.InstantiateObject(PrefabToCreate, TileUtils.MouseCellPosition());
            newObj.GetComponent<tileObjectScript>().direction = m_shadow.GetComponent<tileObjectScript>().direction;
            newObj.transform.position += (Vector3) TileUtils.LevelOffset(m_currentZlevel);
        }
    }

    void ChangePrefab(GameObject p)
    {
        m_isActive = true;
        PrefabToCreate = p;
        CreateShadow();
    }

    void Disable()
    {
        m_isActive = false;
        PrefabToCreate = null;
        Destroy(m_shadow);
        m_shadow = null;
        BuilderPanel.SetActive(true);
    }

    void InitBuilderPanel()
    {
        Vector3 position = BuilderPanel.transform.position + new Vector3(ButtonsMargin, ButtonsMargin);
        GameObject viewPort = BuilderPanel.transform.FindChild("Viewport").gameObject;
        GameObject content = viewPort.transform.FindChild("Content").gameObject;

        // clear panel
        for (var i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        foreach (var obj in Objects)
        {
            // todo init panel buttons according to specified objects
            Sprite img = (obj.Image != null) ? obj.Image : obj.Prefab.GetComponent<SpriteRenderer>().sprite;
            GameObject button = Instantiate(ButtonPrefab);
            button.transform.parent = content.transform;
            button.transform.position = position;
            button.GetComponent<Image>().sprite = img;
            button.GetComponent<Button>().onClick.AddListener(call: delegate() { Pick(obj.Prefab); });
            position += new Vector3(0, ButtonsMargin);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBuilderPanel();

        m_tileManager = TileManagerScript.TileManager;
        m_grid = GameObject.FindWithTag("grid");
        if (m_grid != null)
        {
            m_gridLayout = m_grid.GetComponent<GridLayout>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isActive)
        {
            bool bIsPossibleToCreate = IsPossibleToCreate;

            if (m_shadow != null)
            {
                m_shadow.transform.position = TileUtils.MouseCellPosition() + TileUtils.LevelOffset(m_currentZlevel);
                m_shadow.GetComponent<SpriteRenderer>().color = bIsPossibleToCreate ? ColorUtils.colorTransparentGreen : ColorUtils.colorTransparentRed;
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

    public void Pick(GameObject prefab)
    {
        ChangePrefab(prefab);
        BuilderPanel.SetActive(false);
        try
        {
            m_currentZlevel = prefab.GetComponent<tileObjectScript>().ZPosition;
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private TileManagerScript m_tileManager;
    private GameObject m_grid;
    private GridLayout m_gridLayout;
    private GameObject m_shadow;

    private bool m_isActive = false;
    private int m_currentZlevel = 0;
}
