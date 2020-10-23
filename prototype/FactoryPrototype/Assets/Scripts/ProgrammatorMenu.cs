using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgrammatorMenu : MonoBehaviour, IMenu
{
    private ProgrammatorMenu() {}
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

    public InputField CodeField;
    public Button CompileButton;
    

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
                return;
            }
        }
    }

    public void Compile()
    {
        string code = CodeField.GetComponent<InputField>().text;

    }

    public void ShowFor(Programmator prog)
    {
        m_currentObject = prog;
        Show();
        Debug.Log(prog.Id);
    }

    public void Show()
    {
        GetComponent<Image>().enabled = true;
        setActiveForChildren(true);

        m_isActive = true;

        MenuManager.Manager.SetActive(this);
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        setActiveForChildren(false);
        
        m_isActive = false;
    }

    private void setActiveForChildren(bool isActive)
    {
        foreach (Transform t in GetComponent<Transform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(isActive);
        }
    }


    private bool m_isActive = false;
    private Programmator m_currentObject;
}
