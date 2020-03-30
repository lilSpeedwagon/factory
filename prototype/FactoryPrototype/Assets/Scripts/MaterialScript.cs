using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialScript : MonoBehaviour, IProcessable
{
    [SerializeField]
    private MaterialType m_type;
    public MaterialType Type { set => m_type = value; get => m_type; }

    [SerializeField]
    private int m_cost;
    public int Cost => m_cost;
}
