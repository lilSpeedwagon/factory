using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Programmator : tileObjectScript
{
    public Guid Id => m_id;

    public bool IsActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }

    public string CurrentScriptHash
    {
        get => m_currentScriptHash;
        set => m_currentScriptHash = value;
    }

    void Start()
    {
        m_id = Guid.NewGuid();
        
        InitButton();
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
        // TODO
    }

    private void InitButton()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ShowMenu);
        }
        else
        {
            Debug.LogWarning("There is no UI.Button component for Programmer!");
        }
    }

    private string m_currentScriptHash;
    private bool m_isActive;
    private Guid m_id;
}
