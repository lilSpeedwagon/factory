using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Programmator : tileObjectScript
{
    public ProgrammatorMenu Menu;

    public bool IsActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }



    void Start()
    {
        initButton();
    }

    void Update()
    {
        
    }

    private void ShowMenu()
    {
        Debug.Log("show menu");
        if (Menu != null)
        {
            Menu.ShowFor(this);
        }
        else
        {
            Debug.LogWarning("There is no ProgrammatorMenu attached to Programmator!");
        }
    }

    private void initButton()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ShowMenu);
        }
        else
        {
            Debug.LogWarning("There is no UI.Button component for Programmator!");
        }
    }


    private bool m_isActive;
}
