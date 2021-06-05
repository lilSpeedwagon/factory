using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.Find("RecipeManager").GetComponent<RecipeManager>();
            }

            return g_instance;
        }
    }

    public List<Recipe> Recipes;

    public Recipe FindRecipe(string from, string processor)
    {
        try
        {
            return m_recipesByFrom[from]?.Find(recipe => recipe.Processor == processor);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public bool CanBeProcessed(string from, string processor, int temperature = 0)
    {
        var recipe = FindRecipe(from, processor);
        if (recipe?.Temperature != 0)
        {
            return temperature >= recipe?.Temperature;
        }
        return false;
    }

    private void Start()
    {
        m_recipesByFrom = new Dictionary<string, List<Recipe>>();
        foreach (var recipe in Recipes)
        {
            if (!m_recipesByFrom.ContainsKey(recipe.From))
            {
                m_recipesByFrom[recipe.From] = new List<Recipe>();
            }
            m_recipesByFrom[recipe.From].Add(recipe);
        }
    }

    private Dictionary<string, List<Recipe>> m_recipesByFrom;

    private static RecipeManager g_instance;
}

[System.Serializable]
public class Recipe
{
    public string From, To;
    public string Processor;
    public float ProcessingTime;
    public int Temperature;
}
