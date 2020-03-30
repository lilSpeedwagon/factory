using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance { get; private set; }

    public List<Recipe> Recipes;

    public float ProcessingTime(IProcessable obj, ProcessorType proc)
    {
        foreach (var r in Recipes)
        {
            if (obj.Type == r.From && proc == r.Processor)
                return r.ProcessingTime;
        }

        throw new InvalidExpressionException();
    }

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

    private void Start()
    {
        if (instance == null)
            instance = this;
    }
}

[System.Serializable]
public class Recipe
{
    public MaterialType From, To;
    public ProcessorType Processor;
    public GameObject Prefab;
    public float ProcessingTime;
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