using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI MoneyText;

    public void Earn(int cost)
    {
        Money += cost;
    }

    public void Spend(int cost)
    {
        if (IsPossibleToSpent(cost))
        {
            Money -= cost;
        }
        else
        {
            throw new ArgumentException("Not enough money");
        }
    }

    public bool IsPossibleToSpent(int cost)
    {
        return Money >= cost;
    }


    private void Start()
    {
        if (instance == null)
            instance = this;
        m_recipeManager = GetComponent<RecipeManager>();
        Money = StartMoney;
    }

    private void UpdateValue()
    {
        MoneyText.text = Money.ToString() + '$';
    }

    private RecipeManager m_recipeManager;
    private int m_money;
}
