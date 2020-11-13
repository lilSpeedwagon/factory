using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Programmator : tileObjectScript
{
    public static float MinFreq = 0.1f;
    public static float MaxFreq = 10f;

    public int Id => m_id;

    public bool IsActive { get; set; }

    public string CurrentScript
    {
        get => m_currentScript;
        set
        {
            m_currentScript = value;
            m_currentScriptHash = BeltScriptCodeManager.Instance.GetHashForScript(value);
        }
    }

    public float Frequency { get; set; }

    public void Run()
    {
        m_debugLogger.Log("Run");

        // force init of BeltScript DLLs in Main Thread
        BeltScriptCodeManager.ForceInit();

        m_state = ProgrammerState.Runnable;
        ReconfigureTimer();
        m_execTimer.Start();
        //ExecuteScript();
    }

    public void Stop()
    {
        m_debugLogger.Log("Stop");

        m_execTimer.Stop();
        m_state = ProgrammerState.Idle;
    }

    private void ReconfigureTimer()
    {
        m_execTimer.Stop();
        double timerInterval = 1.0d / Frequency * 1000.0d; // in milliseconds
        m_execTimer.Interval = timerInterval;

        m_debugLogger.Log($"Timer was reconfigured with new interval {timerInterval}");
    }

    private Task OnTimerElapsed(ElapsedEventArgs e)
    {
        m_debugLogger.Log($"OnTimerElapsed {e.SignalTime}");

        if (m_state == ProgrammerState.Running)
        {
            m_debugLogger.Warn("timer elapsed while programmer was in 'Running' state");
            return Task.CompletedTask;
        }

        m_state = ProgrammerState.Running;
        ExecuteScript();
        m_state = ProgrammerState.Runnable;

        return Task.CompletedTask;
    }

    private void ExecuteScript()
    {
        if (string.IsNullOrEmpty(m_currentScript))
        {
            m_debugLogger.Error("No script specified!");
        }

        m_debugLogger.Log($"running {m_currentScript}");

        bool result = false;
        try
        {
            result = BeltScriptCodeManager.Instance.Run(m_currentScript, ProgrammerMenu.Menu.Log);
        }
        catch (Exception e)
        {
            m_debugLogger.Warn(e.Message);
        }

        m_debugLogger.Log($"execution result {result}");

        if (!result)
        {
            ProgrammerMenu.Menu.Log("Execution finished with errors.");
            m_failedExecutionInRow++;

            if (m_failedExecutionInRow >= MaxFailedExecutionsInRow)
            {
                m_debugLogger.Log("Failure execution limit exceeded.");
                ProgrammerMenu.Menu.Log("Failure execution limit exceeded.");
                m_failedExecutionInRow = 0;
                Stop();
            }
        }
        else
        {
            m_failedExecutionInRow = 0;
        }
    }

    void Start()
    {
        m_id = NextId++;

        m_debugLogger = new LogUtils.DebugLogger($"Programmer{m_id}");
        
        InitButton();

        Frequency = 1.0f;
        ReconfigureTimer();
        m_execTimer.AutoReset = true;
        
        m_execTimer.Elapsed += async (sender, e ) => await OnTimerElapsed(e);
    }

    void Update()
    {
        
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
    private Timer m_execTimer = new Timer();

    private LogUtils.DebugLogger m_debugLogger;
}
