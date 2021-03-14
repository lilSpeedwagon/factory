using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInfoHolder : MonoBehaviour
{
    public static MaterialInfoHolder Instance
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.Find("MaterialInfoHolder").GetComponent<MaterialInfoHolder>();
            }

            return g_instance;
        }
    }

    public List<Material> Materials;

    public Material GetMaterialPrefab(string name)
    {
        try
        {
            return m_materials[name];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public bool Exist(string name)
    {
        return m_materials.ContainsKey(name);
    }

    // MonoBehavior
    private void Start()
    {
        m_materials = new Dictionary<string, Material>(Materials.Count);
        foreach (var material in Materials)
        {
            m_materials[material.Name] = material;
        }
    }

    private Dictionary<string, Material> m_materials;

    private static MaterialInfoHolder g_instance;
}
