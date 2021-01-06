﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Programmator : tileObjectScript
{
    public static float MinFreq = 0.1f;
    public static float MaxFreq = 10f;

    public int Id => m_id;

    public bool IsActive => m_state != ProgrammerState.Idle;

    public string CurrentScript
    {
        get => m_currentScript;
        set
        {
            m_currentScript = value;
            m_currentScriptHash = BeltScriptCodeManager.Instance.GetHashForScript(value);
        }
    }

    public float Frequency
    {
        get => m_frequency;
        set
        {
            m_frequency = value;
            if (IsActive)
            {
                ReconfigureTimer();
            }
        }
    }

    public string LogHistory => m_logContainer.ToString();

    public void Run()
    {
        m_debugLogger.Log("Run");

        // force init of BeltScript DLLs in Main Thread
        BeltScriptCodeManager.ForceInit();

        m_state = ProgrammerState.Runnable;
        ReconfigureTimer();
        m_execTimer.Start();
    }

    public void Stop()
    {
        m_debugLogger.Log("Stop");
        m_execTimer.Stop();
        m_state = ProgrammerState.Idle;
    }

    private void Update()
    {
        if (m_state == ProgrammerState.Running)
        {
            if (string.IsNullOrEmpty(m_currentScript))
            {
                m_debugLogger.Error("No script specified!");
                return;
            }

            bool result = ExecuteScript();
            m_state = ProgrammerState.Runnable;

            m_debugLogger.Log($"execution result {result}");

            if (!result)
            {
                Log("Execution finished with errors.");
                m_failedExecutionInRow++;

                if (m_failedExecutionInRow >= MaxFailedExecutionsInRow)
                {
                    Log("Failure execution limit exceeded.");
                    Log("Failure execution limit exceeded.");
                    m_failedExecutionInRow = 0;
                    Stop();
                }
            }
            else
            {
                m_failedExecutionInRow = 0;
            }
        }
    }
    private bool ExecuteScript()
    {
        m_debugLogger.Log($"running {m_currentScript}");

        bool result = false;
        try
        {
            result = BeltScriptCodeManager.Instance.Run(m_currentScript, Log);
        }
        catch (Exception e)
        {
            m_debugLogger.Warn(e.Message);
        }

        return result;
    }

    private void Log(string message)
    {
        m_logContainer.AppendLine(message);
        ProgrammerMenu.Menu.Log(this, message);
    }

    private void ReconfigureTimer()
    {
        //m_execTimer.Stop();
        float timerInterval = 1000.0f / m_frequency; // in milliseconds
        m_execTimer.Interval = timerInterval;

        m_debugLogger.Log($"Timer was reconfigured with new interval {timerInterval}");
    }

    void Start()
    {
        m_id = NextId++;
        m_debugLogger = new LogUtils.DebugLogger($"Programmer{m_id}");

        InitButton();

        m_execTimer.AutoReset = true;
        m_execTimer.Elapsed += (sender, e) => { m_state = ProgrammerState.Running; };
    }

    void OnApplicationQuit()
    {
        // dispose timer (not managed by unity)
        if (m_execTimer.Enabled)
        {
            m_execTimer.Stop();
        }

        m_execTimer.Close();
    }

    // menu logic
    void OnMouseDown()
    {
        ProgrammerMenu.Menu.ShowFor(this);
    }

    private void InitButton()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnMouseDown);
        }
        else
        {
            m_debugLogger.Warn("There is no UI.Button component for Programmer!");
        }
    }

    private enum ProgrammerState
    {
        Idle,
        Runnable,
        Running
    }

    private string m_currentScript;
    private string m_currentScriptHash;
    private int m_id;

    private static int NextId = 1;

    private const int MaxFailedExecutionsInRow = 4;
    private int m_failedExecutionInRow = 0;
    private ProgrammerState m_state = ProgrammerState.Idle;
    private readonly Timer m_execTimer = new Timer();
    private float m_frequency = 1.0f;

    private static int LogCapacity = 10000;
    private readonly StringBuilder m_logContainer = new StringBuilder(LogCapacity);

    private LogUtils.DebugLogger m_debugLogger;
}
