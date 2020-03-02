using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialScript : MonoBehaviour, IProcessable
{
    [SerializeField]
    private MaterialType m_type;
    public MaterialType Type { set { m_type = value; } get => m_type; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
