using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.Find("StorageManager").GetComponent<StorageManager>();
            }

            return g_instance;
        }
    }

    public bool BuyMaterial(string material, int count)
    {
        Material m = MaterialInfoHolder.Instance.GetMaterialPrefab(material);

        try
        {
            ResoucesScript.instance.Spend(m.Cost * count);
            StoreMaterial(material, count);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public bool SellMaterial(string material, int count)
    {
        Material m = MaterialInfoHolder.Instance.GetMaterialPrefab(material);
        if (RemoveMaterial(material, count))
        {
            ResoucesScript.instance.Earn(m.SellCost * count);
            return true;
        }

        return false;
    }

    public void StoreMaterial(string material, int count)
    {
        m_storage[material] += count;
        OnCountChanged();
    }

    public bool RemoveMaterial(string material, int count)
    {
        if (m_storage[material] >= count)
        {
            m_storage[material] -= count;
            OnCountChanged();
            return true;
        }

        return false;
    }

    public int GetMaterialCount(string material)
    {
        return m_storage[material];
    }

    public List<string> GetMaterialsList()
    {
        return m_storage.Keys.ToList();
    }

    public Dictionary<string, int> GetMaterials()
    {
        return m_storage;
    }

    private void OnCountChanged()
    {
        StorageMenu.Instance.UpdateMaterialsCount();
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_storage = MaterialInfoHolder.Instance.Materials.ToDictionary(m => m.Name, m => 0);
    }

    private Dictionary<string, int> m_storage;

    private static StorageManager g_instance;
}
