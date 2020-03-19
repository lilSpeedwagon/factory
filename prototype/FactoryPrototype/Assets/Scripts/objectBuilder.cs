using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class objectBuilder : MonoBehaviour
{
    public List<BuildableObjectScript> Objects;

    public GameObject PrefabToCreate = null;
    public GameObject BuilderPanel;
    public GameObject ButtonPrefab;
    public GameObject RemoverPrefab;
    public int ButtonsMargin = 10;

    delegate void OnBuildSignal(BuildableObjectScript obj);

    bool IsPossibleToCreate => m_isActive && m_tileManager.IsEmpty(TileUtils.MouseCellPosition()) && 
                               ResoucesScript.instance.CanBeBuilt(PrefabToCreate.GetComponent<BuildableObjectScript>());

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

    void CreateRemoverShadow()
    {
        if (m_shadow != null)
        {
            Destroy(m_shadow);
        }

        m_shadow = Instantiate(RemoverPrefab, TileUtils.MouseCellPosition(), TileUtils.qInitRotation);
    }

    void CreateObject()
    {
        if (m_isActive && PrefabToCreate != null)
        {
            GameObject newObj = m_tileManager.InstantiateObject(PrefabToCreate, TileUtils.MouseCellPosition());
            newObj.GetComponent<tileObjectScript>().direction = m_shadow.GetComponent<tileObjectScript>().direction;
            newObj.transform.position += (Vector3) TileUtils.LevelOffset(m_currentZlevel);

            m_onBuildSignal(newObj.GetComponent<BuildableObjectScript>());
        }
    }

    void ChangePrefab(GameObject p)
    {
        m_isActive = true;
        m_isRemoverActive = false;
        PrefabToCreate = p;
        CreateShadow();
    }

    void Disable()
    {
        m_isActive = false;
        m_isRemoverActive = false;
        PrefabToCreate = null;
        Destroy(m_shadow);
        m_shadow = null;
        BuilderPanel.SetActive(true);
    }

    void InitButton(BuildableObjectScript obj, ref Vector2 position)
    {
        Sprite img = (obj.Image != null) ? obj.Image : obj.Prefab.GetComponent<SpriteRenderer>().sprite;
        GameObject button = Instantiate(ButtonPrefab, m_builderPanelContent.transform);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.localPosition = position;
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        button.GetComponent<RawImage>().texture = img.texture;
        button.GetComponent<Button>().onClick.AddListener(call: delegate () { Pick(obj.Prefab); });
        position -= new Vector2(0, ButtonsMargin + button.GetComponent<RectTransform>().rect.height);
    }

    void InitBuilderPanel()
    {
        Vector2 localPos = new Vector3(BuilderPanel.GetComponent<RectTransform>().rect.width / 2, 0.0f);
        GameObject viewPort = BuilderPanel.transform.Find("Viewport").gameObject;
        m_builderPanelContent = viewPort.transform.Find("Content").gameObject;

        // clear panel
        for (var i = 0; i < m_builderPanelContent.transform.childCount; i++)
        {
            Destroy(m_builderPanelContent.transform.GetChild(i).gameObject);
        }

        float buttonHeight = ButtonPrefab.GetComponent<RectTransform>().rect.height;
        float height = (ButtonsMargin + buttonHeight) * (Objects.Count + 1) + ButtonsMargin;
        m_builderPanelContent.GetComponent<RectTransform>().sizeDelta = new Vector2(m_builderPanelContent.GetComponent<RectTransform>().sizeDelta.x, height);

        GameObject button = Instantiate(ButtonPrefab, m_builderPanelContent.transform);
        localPos -= new Vector2(0, ButtonsMargin + button.GetComponent<RectTransform>().rect.height / 2);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.localPosition = localPos;
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        button.GetComponent<RawImage>().texture = RemoverPrefab.GetComponent<SpriteRenderer>().sprite.texture;
        button.GetComponent<Button>().onClick.AddListener(call: PickRemover);

        localPos -= new Vector2(0, ButtonsMargin + buttonHeight);
        foreach (var obj in Objects)
        {
            InitButton(obj, ref localPos);
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

        m_onBuildSignal = delegate(BuildableObjectScript obj) { ResoucesScript.instance.OnBuild(obj); };
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isActive && m_shadow != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }

            if (!m_isRemoverActive)
            {
                bool bIsPossibleToCreate = IsPossibleToCreate;

                m_shadow.transform.position = TileUtils.MouseCellPosition() + TileUtils.LevelOffset(m_currentZlevel);
                m_shadow.GetComponent<SpriteRenderer>().color = bIsPossibleToCreate ? ColorUtils.colorTransparentGreen : ColorUtils.colorTransparentRed;

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
            else
            {
                m_shadow.transform.position = TileUtils.MouseCellPosition();

                if (Input.GetMouseButton(MouseUtils.PRIMARY_MOUSE_BUTTON))
                {
                    try
                    {
                        m_tileManager.RemoveObject(m_shadow.transform.position);
                    }
                    catch (System.Exception)
                    {
                        Debug.Log("Nothing to remove");
                    }
                }
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

    public void PickRemover()
    {
        BuilderPanel.SetActive(false);
        m_isActive = true;
        m_isRemoverActive = true;
        CreateRemoverShadow();
    }

    private TileManagerScript m_tileManager;
    private GameObject m_grid;
    private GridLayout m_gridLayout;
    private GameObject m_shadow;
    private GameObject m_builderPanelContent;

    private OnBuildSignal m_onBuildSignal;

    private bool m_isActive = false;
    private bool m_isRemoverActive = false;
    private int m_currentZlevel = 0;
}
