using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

public class BeltScriptMenu : MonoBehaviour, IMenu
{
    private BeltScriptMenu() {}
    public static BeltScriptMenu Menu
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.FindWithTag("BeltScriptMenu").GetComponent<BeltScriptMenu>();
            }
            return g_instance;
        }
    }

    public InputField CodeInputField;
    public Button CompileButton;
    public InputField ScriptNameInputField;
    public VerticalLayoutGroup ScriptListContent;
    public Button RemoveButton;
    public Scrollbar LogScrollbar;
    public Text LogText;

    public RectTransform ScriptListItemPrefab;

    public void OnCompile()
    {
        Debug.Log("BeltScriptMenu: OnCompile()");

        if (ScriptNameInputField == null || CodeInputField == null)
        {
            Debug.LogWarning("Missing programmer menu element reference");
            return;
        }

        string fileName = ScriptNameInputField.text;
        string code = CodeInputField.text;
        if (fileName.Length == 0)
        {
            Debug.LogWarning("BeltScriptMenu: empty script name");
            return;
        }

        LogBorder();
        if (BeltScriptCodeManager.Instance.Compile(fileName, code, Log))
        {
            UpdateScriptList();
        }
    }

    public void OnLoad()
    {
        if (!string.IsNullOrEmpty(m_currentSelection) && CodeInputField != null)
        {
            string code = BeltScriptCodeManager.Instance.LoadSourceCode(m_currentSelection);
            CodeInputField.text = code;
        }
    }

    public void OnDelete()
    {
        Debug.Log("BeltScriptMenu: OnDelete()");

        if (!String.IsNullOrEmpty(m_currentSelection))
        {
            BeltScriptCodeManager.Instance.DeleteScript(m_currentSelection);
            UpdateScriptList();
            m_currentSelection = null;
        }
    }

    // IMenu implementation
    public void Show()
    {
        GetComponent<Image>().enabled = true;
        SetActiveForChildren(true);

        MenuManager.Manager.SetActive(this);

        UpdateScriptList();
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        SetActiveForChildren(false);
    }

    public bool IsCameraZoomAllowed()
    {
        return false;
    }

    private void SetActiveForChildren(bool isActive)
    {
        foreach (Transform t in GetComponent<Transform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(isActive);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Hide();
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

    private void UpdateScriptList()
    {
        if (ScriptListContent == null)
        {
            Debug.LogWarning("BeltScriptMenu: Script list content is not specified.");
            return;
        }

        ClearScriptList();
        
        foreach (var script in BeltScriptCodeManager.Instance.ScriptList)
        {
            AddScriptListItem(script);
        }
    }

    private void AddScriptListItem(string scriptName)
    {
        Vector3 position = ScriptListContent.GetComponent<RectTransform>().position;

        var newElement = Instantiate(ScriptListItemPrefab, position, Quaternion.identity, ScriptListContent.transform);

        newElement.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // for some reasons it scales to big sizes
        newElement.GetComponent<Text>().text = scriptName;

        Toggle itemToggle = newElement.GetComponent<Toggle>();

        itemToggle.group = ScriptListContent.GetComponent<ToggleGroup>();
        itemToggle.onValueChanged.AddListener(val =>
        {
            if (val)
            {
                Debug.Log($"BeltScriptMenu: Selected {scriptName}");
                m_currentSelection = scriptName;
            }
            else
            {
                m_currentSelection = null;
            }
        });

        m_scriptListItems.Add(newElement);
    }

    private void ClearScriptList()
    {
        ScriptListContent.GetComponent<RectTransform>().DetachChildren();

        foreach (var item in m_scriptListItems)
        {
            Destroy(item.gameObject);
        }

        m_scriptListItems.Clear();
    }

    private List<RectTransform> m_scriptListItems = new List<RectTransform>();
    private string m_currentSelection;
    private static BeltScriptMenu g_instance;
}
