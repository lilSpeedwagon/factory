using System;
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

    public Material GetMaterialPrefab(string materialName)
    {
        try
        {
            return m_materials[materialName];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public Material GetTrashPrefab()
    {
        return GetMaterialPrefab(TrashMaterialName);
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
    private const string TrashMaterialName = "Charcoal";

    private static MaterialInfoHolder g_instance;
}
