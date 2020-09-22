using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class objectBuilder : MonoBehaviour
{
    private static objectBuilder g_instance;
    public static objectBuilder Builder
    {
        get
        {
            if (g_instance == null)
                g_instance = GameObject.FindWithTag("Player").GetComponent<objectBuilder>();
            return g_instance;
        }
    }


    public List<BuildableObjectScript> Objects;
    public GameObject BuilderPanel;

    public GameObject ShadowPrefab;
    public GameObject PrefabToCreate;
    public GameObject RemoverPrefab;

    delegate void OnBuildSignal(BuildableObjectScript obj);
    delegate void OnRemoveSignal(int sellCost);

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

            m_shadow = Instantiate(ShadowPrefab, TileUtils.MouseCellPosition() + TileUtils.LevelOffset(m_currentZlevel), TileUtils.qInitRotation);
            m_shadow.GetComponent<SpriteRenderer>().sprite = PrefabToCreate.GetComponent<SpriteRenderer>().sprite;
            m_shadow.GetComponent<SpriteRenderer>().color = ColorUtils.colorTransparentGreen;
            m_shadow.GetComponent<SpriteRenderer>().sortingLayerName = "shadow";
            m_shadow.GetComponent<tileObjectScript>().isShadow = true;
            m_shadow.GetComponent<tileObjectScript>().OnlyXFlip = PrefabToCreate.GetComponent<tileObjectScript>().OnlyXFlip;
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

    int GetSellCost(GameObject removable)
    {
        return removable.GetComponent<BuildableObjectScript>()?.SellCost ?? 0;
    }

    void RemoveObject()
    {
        try
        {
            var pos = m_shadow.transform.position;
            var removableObject = m_tileManager.GetGameObject(pos);
            int cost = GetSellCost(removableObject);
            m_tileManager.RemoveObject(pos);
            m_onRemoveSignal(cost);
        }
        catch (Exception)
        {
            Debug.Log("Nothing to remove");
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


    // Start is called before the first frame update
    void Start()
    {
        m_tileManager = TileManagerScript.TileManager;

        m_onBuildSignal = delegate(BuildableObjectScript obj) { ResoucesScript.instance.OnBuild(obj); };
        m_onRemoveSignal = delegate (int sellCost) { ResoucesScript.instance.OnSell(sellCost); };
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
                    RemoveObject();
                }
            }
        }
    }

    public void Pick(GameObject prefab)
    {
        ChangePrefab(prefab);
        Hide();
        try
        {
            m_currentZlevel = prefab.GetComponent<tileObjectScript>().ZPosition;
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public void PickRemover()
    {
        Show();
        m_isActive = true;
        m_isRemoverActive = true;
        CreateRemoverShadow();
    }

    public void Hide()
    {
        BuilderPanel.SetActive(false);
    }

    public void Show()
    {
        BuilderPanel.SetActive(false);
    }

    private TileManagerScript m_tileManager;
    private GameObject m_shadow;

    private OnBuildSignal m_onBuildSignal;
    private OnRemoveSignal m_onRemoveSignal;

    private bool m_isActive = false;
    private bool m_isRemoverActive = false;
    private int m_currentZlevel = 0;
}
