﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgrammerMenu : MonoBehaviour, IMenu
{
    private ProgrammerMenu() { }
    public static ProgrammerMenu Menu
    {
        get
        {
            if (g_instance == null)
            {
                //g_instance = GameObject.Find("ProgrammerMenu").GetComponent<ProgrammerMenu>();
                g_instance = Resources.FindObjectsOfTypeAll<ProgrammerMenu>()[0]; // workaround for inactive objects
            }
            return g_instance;
        }
    }

    public TextMeshProUGUI ProgrammerIdLabel;
    public TextMeshProUGUI CurrentScriptLabel;
    public Slider ExecFreqSlider;
    public TextMeshProUGUI ExecFreqLabel;
    public Toggle RunToggle;
    public TextMeshProUGUI RunToggleText;
    public VerticalLayoutGroup PortList;
    public VerticalLayoutGroup ScriptListContent;
    public TextMeshProUGUI LogText;
    public Scrollbar LogScrollbar;

    public RectTransform ScriptListItemPrefab;
    public RectTransform PortListItemPrefab;

    public void ShowFor(Programmator prog)
    {
        m_currentProgrammer = prog;

        SetProgrammerLabel("Programmable Unit " + prog.Id);
        SetCurrentScriptLabel(prog.CurrentScript);
        SetFreqSliderValue(prog.Frequency);
        SetFreqLabelValue(prog.Frequency.ToString());
        SetToggleState(prog.IsActive);

        // make the toggle interactable if we have script
        bool programmerHasScript = !string.IsNullOrEmpty(prog.CurrentScript);
        SetToggleActive(programmerHasScript);

        UpdateScriptList();
        UpdatePortListData();

        Show();

        RestoreLog(prog);
    }

    // IMenu implementation
    public void Show()
    {
        GetComponent<Image>().enabled = true;
        GameObjectUtils.SetActiveForChildren(gameObject, true);

        MenuManager.Manager.SetActive(this);

        m_isVisible = true;
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        GameObjectUtils.SetActiveForChildren(gameObject, false);
        m_isVisible = false;
        ClearLog();
    }

    public bool IsCameraZoomAllowed()
    {
        return false;
    }

    public string Name => MenuName;
    // IMenu end

    // toggle callback
    public void OnRunToggle(bool value)
    {
        if (m_currentProgrammer == null) return;

        if (value)
        {
            m_currentProgrammer.Run();
        }
        else
        {
            m_currentProgrammer.Stop();
        }
        SetToggleStateText(value);
    }

    public void OnExecutionStopped()
    {
        SetToggleState(false);
    }

    public void Log(Programmator prog, string message)
    {
        // write log only if menu is visible and only for selected programmer
        if (m_isVisible && LogText != null && prog == m_currentProgrammer)
        {
            LogText.text += '\n' + message;
            
            // move scroll handler to the bottom
            // take a value lower than 0 because the content
            // rectangle is not expanded yet, so after
            // its expansion current position will no be in the
            // very end of the log
            LogScrollbar.value = -1.0f;
        }
    }

    public void RestoreLog(Programmator prog)
    {
        if (m_isVisible && LogText != null)
        {
            LogText.text = prog.LogHistory;
            LogScrollbar.value = 0.0f;
        }
    }

    public void ClearLog()
    {
        if (LogText != null)
        {
            LogText.text = "";
        }
    }

    private void SetProgrammerLabel(string value)
    {
        if (ProgrammerIdLabel != null)
        {
            ProgrammerIdLabel.text = value;
        }
    }

    private void SetCurrentScriptLabel(string value)
    {
        if (CurrentScriptLabel != null)
        {
            CurrentScriptLabel.text = value;
        }
    }

    private void SetFreqSliderValue(float value)
    {
        if (ExecFreqSlider != null)
        {
            ExecFreqSlider.value = value;
        }
    }

    private void SetFreqLabelValue(string value)
    {
        if (ExecFreqLabel != null)
        {
            ExecFreqLabel.text = value;
        }
    }

    private void SetToggleState(bool isRunning)
    {
        if (RunToggle != null)
        {
            RunToggle.SetIsOnWithoutNotify(isRunning);
            SetToggleStateText(isRunning);
        }
    }

    private void SetToggleStateText(bool isRunning)
    {
        if (RunToggleText != null)
        {
            RunToggleText.text = isRunning ? RunButtonTextRunning : RunButtonTextStopped;
        }
    }

    private void SetToggleActive(bool isActive)
    {
        if (RunToggle != null)
        {
            RunToggle.interactable = isActive;
        }
    }

    private void SetCurrentScript(string scriptName)
    {
        if (m_currentProgrammer != null)
        {
            m_currentProgrammer.CurrentScript = scriptName;
        }

        SetCurrentScriptLabel(scriptName);
    }
    
    private void Start()
    {
        m_logger = new LogUtils.DebugLogger("Programmer Menu");

        Hide();
        InitPortList();

        if (ExecFreqSlider != null)
        {
            ExecFreqSlider.onValueChanged.AddListener(UpdateSliderValue);
            ExecFreqSlider.maxValue = Programmator.MaxFreq;
            ExecFreqSlider.minValue = Programmator.MinFreq;
        }
    }

    private void UpdateSliderValue(float frequency)
    {
        if (ExecFreqSlider != null)
        {
            SetFreqLabelValue(frequency.ToString());

            if (m_currentProgrammer != null)
            {
                m_currentProgrammer.Frequency = frequency;
            }
        }
    }

    private void UpdateScriptList()
    {
        if (ScriptListContent == null)
        {
            Debug.LogWarning("ProgrammerMenu: Script list content is not specified.");
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
        newElement.GetComponent<TextMeshProUGUI>().text = scriptName;

        Toggle itemToggle = newElement.GetComponent<Toggle>();

        itemToggle.group = ScriptListContent.GetComponent<ToggleGroup>();
        itemToggle.onValueChanged.AddListener(val =>
        {
            if (val)
            {
                SetCurrentScript(scriptName);
                SetToggleActive(true);
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

    private void InitPortList()
    {
        if (PortList == null || !PortListItemPrefab)
        {
            m_logger.Warn("Cannot init port list. Missing required data.");
            return;
        }

        m_portLabelsList = new List<TextMeshProUGUI>(Programmator.DataPortsCount);

        var transform = PortList.GetComponent<RectTransform>();
        Vector3 position = transform.position;
        for (int i = 0; i < Programmator.DataPortsCount; i++)
        {
            var item = Instantiate(PortListItemPrefab, position, Quaternion.identity, transform);
            item.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // for some reasons it scales to big sizes
            m_portLabelsList.Add(item.GetComponent<TextMeshProUGUI>());
        }
    }

    private void UpdatePortListData()
    {
        if (m_currentProgrammer != null)
        {
            var publisher = m_currentProgrammer.GetComponent<DataPublisher>();
            var portList = publisher.PortList;
            for (int i = 0; i < Programmator.DataPortsCount; i++)
            {
                var port = portList[i];
                const string outLabel = "out";
                const string inLabel = "in";
                const string notConnectedLabel = "N/A";
                string typeLabel = port.IsConnected ? (port.IsSource ? outLabel : inLabel) : notConnectedLabel;

                var value = port.CurrentValue;
                string valueLabel;
                switch (value.Type)
                {
                    case DataValue.DataType.String:
                        valueLabel = value.GetString();
                        break;
                    case DataValue.DataType.Number:
                        valueLabel = value.GetNumber().ToString();
                        break;
                    case DataValue.DataType.Bool:
                        valueLabel = value.GetBool().ToString();
                        break;
                    default:
                        valueLabel = "undefined";
                        break;
                }

                string text = $"Port{i} {typeLabel} value: {valueLabel}";
                m_portLabelsList[i].text = text;
            }
        }
    }

    private bool m_isVisible;
    private List<RectTransform> m_scriptListItems = new List<RectTransform>();
    private Programmator m_currentProgrammer;
    private List<TextMeshProUGUI> m_portLabelsList;

    private LogUtils.DebugLogger m_logger;

    private const string MenuName = "Programmer Menu";
    private const string RunButtonTextStopped = "Run";
    private const string RunButtonTextRunning = "Stop";
    private static ProgrammerMenu g_instance;
}
