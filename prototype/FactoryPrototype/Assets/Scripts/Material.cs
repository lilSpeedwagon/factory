using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Material : MonoBehaviour
{
    [SerializeField]
    private int m_cost = 0;
    public int Cost => m_cost;

    public int SellCost;

    public string Name;
}
