using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammatorMenu : MonoBehaviour, IMenu
{
    private static ProgrammatorMenu g_instance;
    public static ProgrammatorMenu Menu
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.FindWithTag("ProgrammerMenu").GetComponent<ProgrammatorMenu>();
            }
            return g_instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowFor(Programmator prog)
    {
        m_currentObject = prog;
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    

    private Programmator m_currentObject;
}
