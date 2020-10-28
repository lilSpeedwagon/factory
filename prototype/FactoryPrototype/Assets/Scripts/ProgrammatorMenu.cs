using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgrammatorMenu : MonoBehaviour, IMenu
{
    private ProgrammatorMenu() {}
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

    public InputField CodeInputField;
    public Button CompileButton;
    public Text TopLabel;
    public InputField ScriptNameInputField;
    public Text ScriptListText;
    public Button SelectButton;
    public Button RemoveButton;
    public Scrollbar LogScrollbar;
    public Text LogText;


    public void OnCompile()
    {
        Debug.Log("ProgrammerMenu: OnCompile()");

        if (ScriptNameInputField == null || CodeInputField == null)
        {
            Debug.LogWarning("Missing programmer menu element reference");
            return;
        }

        string fileName = ScriptNameInputField.text;
        string code = CodeInputField.text;
        if (fileName.Length == 0)
        {
            Debug.LogWarning("ProgrammerMenu: empty script name");
            return;
        }

        LogBorder();
        if (BeltScriptCodeManager.Instance.Compile(fileName, code, Log))
        {
            UpdateFileList();
        }
    }

    public void OnSelect()
    {
        Debug.Log("ProgrammerMenu: OnSelect()");

    }

    public void OnDelete()
    {
        Debug.Log("ProgrammerMenu: OnDelete()");

    }

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

    // IMenu implementation
    public void Show()
    {
        GetComponent<Image>().enabled = true;
        SetActiveForChildren(true);

        m_isActive = true;

        MenuManager.Manager.SetActive(this);
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        SetActiveForChildren(false);
        
        m_isActive = false;
    }

    public void ShowFor(Programmator prog)
    {
        m_currentObject = prog;
        Show();
        Debug.Log(prog.Id);
    }

    private void SetActiveForChildren(bool isActive)
    {
        foreach (Transform t in GetComponent<Transform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(isActive);
        }
    }

    // Menu handlers
    private void ClearLog()
    {
        if (LogText != null)
        {
            LogText.text = "";
        }
    }

    private void Log(string message)
    {
        if (LogText != null)
        {
            LogText.text += message + '\n';

            // move scroll handler to the bottom
            LogScrollbar.value = 1.0f;
        }
    }

    private void LogBorder()
    {
        Log("--------------------------------------------");
    }

    private void UpdateFileList()
    {
        Debug.Log("Files:");
        foreach (var script in BeltScriptCodeManager.Instance.ScriptList)
        {
            Debug.Log(script);
        }
    }
    
    private bool m_isActive = false;
    private Programmator m_currentObject;
    private static ProgrammatorMenu g_instance;
}
