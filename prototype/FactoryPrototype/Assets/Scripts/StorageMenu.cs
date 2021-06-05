using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StorageMenu : MonoBehaviour, IMenu
{
    public static StorageMenu Instance
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.Find("StorageMenu").GetComponent<StorageMenu>();
            }

            return g_instance;
        }
    }

    public GridLayoutGroup MaterialsGrid;
    public RectTransform MaterialItemPrefab;

    // IMenu
    public void Show()
    {
        if (!m_isInitialized)
        {
            InitGrid();
        }

        MenuManager.Manager.SetActive(this);
        GameObjectUtils.SetActiveForChildren(gameObject, true);
    }

    public void Hide()
    {
        GameObjectUtils.SetActiveForChildren(gameObject, false);
    }

    public bool IsCameraZoomAllowed()
    {
        return false;
    }

    public string Name => MenuName;
    // IMenu end

    public void UpdateMaterialsCount()
    {
        foreach (var kv in m_materialCountLabels)
        {
            int count = StorageManager.Instance.GetMaterialCount(kv.Key);
            kv.Value.text = count.ToString();
        }
    }

    private void InitMaterialItem(Material material)
    {
        RectTransform item = GameObject.Instantiate(MaterialItemPrefab, MaterialsGrid.GetComponent<RectTransform>());
        
        Image img = item.GetChild(0).GetComponent<Image>();
        img.sprite = material.Image;

        var layoutGroup = img.GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>();

        TextMeshProUGUI countLabel = layoutGroup.GetChild(0).GetComponent<TextMeshProUGUI>();
        countLabel.text = "0";
        m_materialCountLabels[material.Name] = countLabel;

        TextMeshProUGUI nameLabel = layoutGroup.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameLabel.text = material.Name;

        Button buyButton = item.GetChild(1).GetComponent<Button>();
        // this item can not be bought
        if (material.Name == "charcoal")
        {
            Destroy(buyButton);
        }
        else
        {
            buyButton.onClick.AddListener(delegate
            {
                StorageManager.Instance.BuyMaterial(material.Name, 1);
            });
        }

        TextMeshProUGUI buyButtonLabel = buyButton.GetComponent<RectTransform>().GetChild(0).GetComponent<TextMeshProUGUI>();
        buyButtonLabel.text = material.Cost.ToString() + '$';

        Button sellButton = item.GetChild(2).GetComponent<Button>();
        sellButton.onClick.AddListener(delegate
        {
            StorageManager.Instance.SellMaterial(material.Name, 1);
        });

        TextMeshProUGUI sellButtonLabel = sellButton.GetComponent<RectTransform>().GetChild(0).GetComponent<TextMeshProUGUI>();
        sellButtonLabel.text = material.SellCost.ToString() + '$';
    }

    private void InitGrid()
    {
        List<string> materials = StorageManager.Instance.GetMaterialsList();
        
        foreach (var name in materials)
        {
            Material material = MaterialInfoHolder.Instance.GetMaterialPrefab(name);
            InitMaterialItem(material);
        }

        m_isInitialized = true;
    }

    private void Start()
    {
        Hide();
        m_materialCountLabels = new Dictionary<string, TextMeshProUGUI>();
        m_isInitialized = false;
    }

    private bool m_isInitialized;
    private Dictionary<string, TextMeshProUGUI> m_materialCountLabels;

    private const string MenuName = "Storage Menu";
    private static StorageMenu g_instance;
}
