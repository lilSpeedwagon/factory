using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public List<Recipe> Recipes;

    public bool CanBeProcessed(ProcessorType proc, MaterialType mat)
    {
        foreach (var r in Recipes)        
            if (r.Processor == proc && r.From == mat)
                return true;
        return false;
    }

    public GameObject FindPrefab(ProcessorType proc, MaterialType mat)
    {
        foreach (var r in Recipes)
            if (r.Processor == proc && r.From == mat)
                return r.Prefab;
        return null;
    }
}

[System.Serializable]
public class Recipe
{
    public MaterialType From, To;
    public ProcessorType Processor;
    public GameObject Prefab;
}

public enum MaterialType
{
    Undefined,
    Gear,
    Gear2
}

public enum ProcessorType
{
    Undefined,
    Processor,
}