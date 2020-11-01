using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * Menus:
 *  1. Builder menu (visible by default)
 *  2. Programmer menu
 *  3. Main menu
 *  4. Wires menu
 */

public interface IMenu
{
    void Show();
    void Hide();
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
