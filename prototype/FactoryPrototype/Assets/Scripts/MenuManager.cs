using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * Menus:
 *  1. Builder menu
 *  2. Programmer menu
 *  3. Main menu
 *  4. Wires menu
 */

public interface IMenu
{
    void Show();
    void Hide();
}

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

    public ProgrammatorMenu ProgrammerMenu;
    // TODO public BuilderMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_isActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                hide();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(MouseUtils.PRIMARY_MOUSE_BUTTON))
            {
                Vector2 mousePos = TileUtils.MouseCellPosition();
                GameObject obj = TileManagerScript.TileManager.GetGameObject(mousePos);

                if (obj != null)
                {
                    Programmator prog = obj.GetComponent<Programmator>();
                    if (prog != null)
                    {
                        ProgrammerMenu?.ShowFor(prog);
                        m_current = ProgrammerMenu;
                    }
                }
            }
        }

        
    }

    private void hide()
    {
        m_current.Hide();
        m_current = null;
    }

    private bool m_isActive => m_current != null;
    private IMenu m_current;
    private static MenuManager g_instance;
}
