using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


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
    public GameObject EnergyAreaPrefab;
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
            m_logger.Warn(e.Message);
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

    private bool IsPossibleToCreate => m_isActive && m_tileManager.IsEmpty(TileUtils.NormalizedMousePosition()) && 
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

            m_shadow = Instantiate(ShadowPrefab, TileUtils.NormalizedMousePosition() + TileUtils.LevelOffset(m_currentZlevel), TileUtils.InitRotation);
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

        m_shadow = Instantiate(RemoverPrefab, TileUtils.NormalizedMousePosition(), TileUtils.InitRotation);
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
                m_logger.Error($"Cannot build. Error: {e.Message}.");
                return;
            }

            GameObject newObj = m_tileManager.InstantiateObject(PrefabToCreate.gameObject, TileUtils.NormalizedMousePosition());
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

        EnergySource source = PrefabToCreate.GetComponent<EnergySource>();
        if (source != null)
        {
            ShowEnergyArea(source);
        }
    }

    private void Disable()
    {
        m_isActive = false;
        m_isRemoverActive = false;
        PrefabToCreate = null;
        Destroy(m_shadow);
        m_shadow = null;
        HideEnergyArea();

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

    private void ShowEnergyArea(EnergySource source)
    {
        if (m_energySourceSelected)
        {
            HideEnergyArea();
        }

        if (EnergyAreaPrefab == null)
        {
            m_logger.Warn("Cannot show energy area. Tile prefab not found.");
            return;
        }

        int radius = (int) source.EnergyDistributionCellRadius;
        Vector2 mouseWorldPosition = TileUtils.NormalizedMousePosition();
        Vector2 cellStep = TileUtils.TileSizeRelative;

        for (float i = -radius * cellStep.y; i <= radius * cellStep.y; i += cellStep.y)
        {
            for (float j = - radius * cellStep.x; j <= radius * cellStep.x; j += cellStep.x)
            {
                Vector2 tilePosition = TileManagerScript.TileManager.CellToWorld(new Vector2(j, i)) + mouseWorldPosition;
                var energyAreaTile = Instantiate(EnergyAreaPrefab, tilePosition, Quaternion.identity);
                m_energyAreaTiles.Add(energyAreaTile);
            }
        }

        m_energyAreaPosition = mouseWorldPosition;
        m_energySourceSelected = true;
    }

    private void MoveEnergyArea()
    {
        Vector2 mousePosition = TileUtils.NormalizedMousePosition();
        Vector2 diff = mousePosition - m_energyAreaPosition;

        foreach (var tile in m_energyAreaTiles)
        {
            tile.GetComponent<Transform>().Translate(diff);
        }

        m_energyAreaPosition = mousePosition;
    }

    private void HideEnergyArea()
    {
        foreach (var tile in m_energyAreaTiles)
        {
            Destroy(tile);
        }
        m_energyAreaTiles.Clear();
        m_energySourceSelected = false;
    }

    private void Start()
    {
        m_logger = new LogUtils.DebugLogger("Builder");

        m_tileManager = TileManagerScript.TileManager;
        m_energyAreaTiles = new List<GameObject>(25);
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

                m_shadow.transform.position = TileUtils.NormalizedMousePosition() + TileUtils.LevelOffset(m_currentZlevel);
                m_shadow.GetComponent<SpriteRenderer>().color = bIsPossibleToCreate ? ColorUtils.colorTransparentGreen : ColorUtils.colorTransparentRed;

                if (m_energySourceSelected)
                {
                    MoveEnergyArea();
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
            else
            {
                m_shadow.transform.position = TileUtils.NormalizedMousePosition();

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

    private Vector2 m_energyAreaPosition;
    private List<GameObject> m_energyAreaTiles;
    private bool m_energySourceSelected = false;

    private bool m_isActive = false;
    private bool m_isRemoverActive = false;
    private int m_currentZlevel = 0;

    private LogUtils.DebugLogger m_logger;
}
