using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class objectBuilder : MonoBehaviour, IMenu
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
    public BuilderMenu BuilderPanel;

    public GameObject ShadowPrefab;
    public BuildableObjectScript PrefabToCreate;
    public GameObject RemoverPrefab;

    // IMenu implementation
    public void Hide()
    {
    }

    public void Show()
    {
        MenuManager.Manager.SetActive(this);
        HidePanel();
    }

    public bool IsCameraZoomAllowed()
    {
        return false;
    }

    // builder
    public void Pick(BuildableObjectScript prefab)
    {
        ChangePrefab(prefab);
        try
        {
            m_currentZlevel = prefab.GetComponent<tileObjectScript>().ZPosition;
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e);
        }
        Show();
    }

    public void PickRemover()
    {
        m_isActive = true;
        m_isRemoverActive = true;
        CreateRemoverShadow();
        Show();
    }

    private bool IsPossibleToCreate => m_isActive && m_tileManager.IsEmpty(TileUtils.MouseCellPosition()) && 
                                       ResoucesScript.instance.IsPossibleToSpent(PrefabToCreate.Cost);

    private void RotateShadow(bool bClockwise = true)
    {
        if (m_shadow == null)
            return;

        m_shadow.GetComponent<tileObjectScript>().Rotate(bClockwise);
    }

    private void CreateShadow()
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

    private void CreateRemoverShadow()
    {
        if (m_shadow != null)
        {
            Destroy(m_shadow);
        }

        m_shadow = Instantiate(RemoverPrefab, TileUtils.MouseCellPosition(), TileUtils.qInitRotation);
    }

    private void CreateObject()
    {
        if (m_isActive && PrefabToCreate != null)
        {
            try
            {
                ResoucesScript.instance.Spend(PrefabToCreate.Cost);
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning($"Cannot build. Error: {e.Message}.");
                return;
            }

            GameObject newObj = m_tileManager.InstantiateObject(PrefabToCreate.gameObject, TileUtils.MouseCellPosition());
            newObj.GetComponent<tileObjectScript>().direction = m_shadow.GetComponent<tileObjectScript>().direction;
            newObj.transform.position += (Vector3) TileUtils.LevelOffset(m_currentZlevel);

            TryAddEnergyObject(newObj);
        }
    }

    private int GetSellCost(GameObject removable)
    {
        return removable.GetComponent<BuildableObjectScript>()?.SellCost ?? 0;
    }

    private void RemoveObject()
    {
        try
        {
            var pos = m_shadow.transform.position;
            var removableObject = m_tileManager.GetGameObject(pos);
            TryRemoveEnergyObject(removableObject);
            int cost = GetSellCost(removableObject);
            ResoucesScript.instance.Earn(cost);
            m_tileManager.RemoveObject(pos);
        }
        catch (NullReferenceException) { }
    }

    private void ChangePrefab(BuildableObjectScript p)
    {
        m_isActive = true;
        m_isRemoverActive = false;
        PrefabToCreate = p;
        CreateShadow();
    }

    private void Disable()
    {
        m_isActive = false;
        m_isRemoverActive = false;
        PrefabToCreate = null;
        Destroy(m_shadow);
        m_shadow = null;

        ShowPanel();
    }

    private void TryAddEnergyObject(GameObject obj)
    {
        EnergyConsumer consumer = obj.GetComponent<EnergyConsumer>();
        EnergySource source = obj.GetComponent<EnergySource>();
        if (consumer != null)
        {
            EnergyManager.Instance.AddConsumer(consumer);
        }
        else if (source != null)
        {
            EnergyManager.Instance.AddSource(source);
        }
    }

    private void TryRemoveEnergyObject(GameObject obj)
    {
        EnergyConsumer consumer = obj.GetComponent<EnergyConsumer>();
        EnergySource source = obj.GetComponent<EnergySource>();
        if (consumer != null)
        {
            EnergyManager.Instance.RemoveConsumer(consumer);
        }
        else if (source != null)
        {
            EnergyManager.Instance.RemoveSource(source);
        }
    }

    private void Start()
    {
        m_tileManager = TileManagerScript.TileManager;
    }

    private void Update()
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

    private void HidePanel()
    {
        BuilderPanel.Hide();
    }

    private void ShowPanel()
    {
        MenuManager.Manager.SetActive(BuilderPanel);
        BuilderPanel.Show();
    }

    private TileManagerScript m_tileManager;
    private GameObject m_shadow;

    private bool m_isActive = false;
    private bool m_isRemoverActive = false;
    private int m_currentZlevel = 0;
}
