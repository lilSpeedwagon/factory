using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResoucesScript : MonoBehaviour
{
    public static ResoucesScript instance;

    public int Money
    {
        get => m_money;
        set
        {
            m_money = value; 
            UpdateValue();
        }
    }

    public int StartMoney = 0;
    public UnityEngine.UI.Text MoneyText;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        m_recipeManager = GetComponent<RecipeManager>();
        Money = StartMoney;
    }

    public void Earn(int cost)
    {
        Money += cost;
    }

    public void OnEarn(int cost)
    {
        Earn(cost);
    }

    public void Spend(int cost)
    {
        Money -= cost;
    }

    public void OnSpend(int cost)
    {
        Spend(cost);
    }

    public void OnBuild(BuildableObjectScript obj)
    {
        Spend(obj.Cost);
    }

    public void OnSell(int sellCost)
    {
        Earn(sellCost);
    }

    public bool CanBeBuilt(BuildableObjectScript obj)
    {
        return Money >= obj.Cost;
    }

    private void UpdateValue()
    {
        MoneyText.text = Money.ToString();
    }

    private RecipeManager m_recipeManager;
    private int m_money;
}
