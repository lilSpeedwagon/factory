using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Programmator : tileObjectScript
{
    public ProgrammatorMenu Menu;
    public Guid Id => m_id;

    public bool IsActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }



    void Start()
    {
        m_id = Guid.NewGuid();

        Menu = ProgrammatorMenu.Menu;
        initButton();
    }

    void Update()
    {
        
    }

    void OnMouseDown()
    {
        ShowMenu();
    }

    private void ShowMenu()
    {
        Debug.Log("show programmer menu");
        Menu?.ShowFor(this);
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
    private Guid m_id;
}
