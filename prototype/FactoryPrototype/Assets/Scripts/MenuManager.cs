using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

/*
 * Menus:
 *  1. Builder menu (visible by default)
 *  2. Builder mode
 *  3. Programmer menu
 *  4. Main menu
 *  5. Wires menu
 *  6. Storage menu
 */

public interface IMenu
{
    void Show();
    void Hide();
    bool IsCameraZoomAllowed();
}

// Menu Manager is responsible for holding ref to current active IMenu object
// and hiding it in the case of call SetActive for another IMenu

public class MenuManager : MonoBehaviour
{
    public static MenuManager Manager
    {
        get
        {
            if (g_instance == null)
                g_instance = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
            return g_instance;
        }
    }

    public BeltScriptMenu BeltScriptMenu;
    public BuilderMenu BuilderMenu;
    public ProgrammerMenu ProgrammerMenu;
    public DataPipeMenu DataPipeMenu;

    public void SetActive(IMenu activeMenu)
    {
        if (m_current != activeMenu)
        {
            m_current?.Hide();
            m_current = activeMenu;
        }
    }

    public bool IsActive => m_current != null;
    public bool IsNonDefaultActive => IsActive && m_current != m_default;

    public bool IsCameraZoomAllowed => IsActive && m_current.IsCameraZoomAllowed();
    
    void Start()
    {
        m_default = BuilderMenu;
        m_current = m_default;
    }
    
    void Update()
    {
        // do not hide default menu by Esc
        if (IsActive && m_current != m_default)
        {
            // listen for Esc button and hide current active menu by pressing
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideActive();
            }
        }
    }
    
    private void HideActive()
    {
        SetActive(m_default);
        m_default.Show();
    }

    private IMenu m_default;
    private IMenu m_current;
    private static MenuManager g_instance;
}
