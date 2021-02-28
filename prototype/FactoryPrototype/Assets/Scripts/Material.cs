using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;


public class Material : MonoBehaviour
{
    [SerializeField]
    private int m_cost = 0;
    public int Cost => m_cost;

    public int SellCost;

    public string Name;

    public Sprite Image;
}
