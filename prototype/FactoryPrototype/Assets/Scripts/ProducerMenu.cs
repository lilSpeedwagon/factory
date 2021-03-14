using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ProducerMenu : MonoBehaviour, IMenu
{
    // TODO singleton
    private ProducerMenu() { }
    public static ProducerMenu Instance
    {
        get
        {
            if (g_instance == null)
            {
                //g_instance = GameObject.Find("ProducerMenu").GetComponent<ProducerMenu>();
                g_instance = Resources.FindObjectsOfTypeAll<ProducerMenu>()[0]; // workaround for inactive objects
            }
            return g_instance;
        }
    }

    public RectTransform MaterialList;
    public RectTransform ListItemPrefab;

    // IMenu
    public void Show()
    {
        MenuManager.Manager.SetActive(this);
        GameObjectUtils.SetActiveForChildren(gameObject, true);
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ShowMaterialList(position, GetMaterialsList());
    }

    public void Hide()
    {
        HideMaterialList();
    }

    public bool IsCameraZoomAllowed()
    {
        return true;
    }
    // IMenu end

    public void ShowFor(IOutput producer)
    {
        Debug.Log("ProducerMenu: ShowFor()");
        m_selectedOutput = producer;
        Show();
    }

    private void OnMaterialSelected(string material)
    {
        m_selectedOutput.MaterialToEmit = material;
        MenuManager.Manager.HideActive();
    }

    private Dictionary<string, int> GetMaterialsList()
    {
        return StorageManager.Instance.GetMaterials().Where(pair => pair.Value > 0).ToDictionary(p => p.Key, p => p.Value);
    }

    private void ShowMaterialList(Vector2 pos, Dictionary<string, int> materials)
    {
        GetComponent<RectTransform>().SetPositionAndRotation(pos, Quaternion.identity);

        CreateItemButton(pos, "", 0);
        foreach (var m in materials)
        {
            CreateItemButton(pos, m.Key, m.Value);
        }
    }

    private void CreateItemButton(Vector2 pos, string material = "", int quantity = 0)
    {
        var item = Instantiate(ListItemPrefab, pos, Quaternion.identity, MaterialList);
        item.localScale = new Vector3(1.0f, 1.0f, 1.0f); // prevent wrong scaling

        bool isNothing = string.IsNullOrEmpty(material);
        
        var text = item.GetComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.text = !isNothing ? $"{material} ({quantity})" : "Nothing";

        var toggle = item.GetComponent<Toggle>();
        toggle.isOn = m_selectedOutput.MaterialToEmit == material;
        toggle.onValueChanged.AddListener(val =>
        {
            OnMaterialSelected(material);
        });
    }
    

    private void HideMaterialList()
    {
        // destroy list items
        foreach (RectTransform t in MaterialList.GetComponentInChildren<RectTransform>())
        {
            if (t != MaterialList)
                Destroy(t.gameObject);
        }

        GameObjectUtils.SetActiveForChildren(gameObject, false);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Hide();
    }
    
    private IOutput m_selectedOutput;

    private static ProducerMenu g_instance;
}
