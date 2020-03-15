using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResoucesScript : MonoBehaviour
{
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
        m_recipeManager = GetComponent<RecipeManager>();
        Money = StartMoney;
    }

    public void Spend(int cost)
    {
        Money -= cost;
    }

    private void UpdateValue()
    {
        MoneyText.text = Money.ToString();
    }

    private RecipeManager m_recipeManager;
    private int m_money;
}
