using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;


// responsible for managing BeltScript DLLs
public class BeltScriptAdapter : MonoBehaviour
{
    // singleton
    private BeltScriptAdapter() { }
    public static BeltScriptAdapter Instance
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.Find("BeltScriptAdapter").GetComponent<BeltScriptAdapter>();
            }

            return g_instance;
        }
    }

    public bool Compile(string codeFileName, string code, Logger.LogDelegate log)
    {
        return m_compiler.Compile(codeFileName, code, log);
    }

    public bool Run(string codeFileName, Logger.LogDelegate log)
    {
        return m_runtime.Run(codeFileName, log);
    }

    public bool RunIo(string codeFileName, float[] inputs, ref float[] outputs, Logger.LogDelegate log)
    {
        return m_runtime.RunIo(codeFileName, log, inputs.Length, inputs, outputs.Length, ref outputs);
    }

    public bool Reset(string codeFileName)
    {
        return m_runtime.Reset(codeFileName);
    }

    private void Start()
    {
        m_runtime = new RuntimeDll();
        m_runtime.Load();

        m_compiler = new CompilerDll();
        m_compiler.Load();
    }

    private void OnApplicationQuit()
    {
        // always dispatch DLLs on quit
        m_compiler.Unload();
        m_runtime.Unload();
    }

    private static BeltScriptAdapter g_instance;

    private RuntimeDll m_runtime;
    private CompilerDll m_compiler;
}


public class Logger
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void LogDelegate(string message);
}

internal abstract class DllLoader
{
    public abstract void Load();
    public abstract void Unload();

    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr LoadLibrary(string lib);
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern void FreeLibrary(IntPtr module);
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern IntPtr GetProcAddress(IntPtr module, string proc);
}

internal class RuntimeDll : DllLoader
{
    public bool IsLoaded => m_handle != IntPtr.Zero;

    public bool Run(string codeFileName, Logger.LogDelegate log)
    {
        if (!IsLoaded)
        {
            Debug.LogWarning("Runtime.dll isn't loaded.");
            return false;
        }

        return m_runDelegate(codeFileName, Marshal.GetFunctionPointerForDelegate(log));
    }

    public bool RunIo(string codeFileName, Logger.LogDelegate log, int inputsCount, float[] inputs, int outputsCount, ref float[] outputs)
    {
        if (!IsLoaded)
        {
            return false;
        }
        
        return m_runIoDelegate(codeFileName, Marshal.GetFunctionPointerForDelegate(log), inputsCount, inputs, outputsCount, ref outputs);
    }

    public bool Reset(string codeFileName)
    {
        return m_resetDelegate(codeFileName);
    }

    public override void Load()
    {
        Debug.Log("Loading Runtime.dll");

        if (IsLoaded)
        {
            Debug.LogWarning("Runtime.dll is already loaded");
            return;
        }

        m_handle = LoadLibrary("Runtime.dll");
        if (m_handle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            IntPtr methodRunHandle = GetProcAddress(m_handle, "Run");
            if (methodRunHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IntPtr methodRunIoHandle = GetProcAddress(m_handle, "RunIO");
            if (methodRunIoHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IntPtr methodResetHandle = GetProcAddress(m_handle, "RemoveFromCash");
            if (methodResetHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            m_runDelegate = (RunDelegate)Marshal.GetDelegateForFunctionPointer(methodRunHandle, typeof(RunDelegate));
            m_runIoDelegate = (RunIoDelegate)Marshal.GetDelegateForFunctionPointer(methodRunIoHandle, typeof(RunIoDelegate));
            m_resetDelegate = (ResetDelegate) Marshal.GetDelegateForFunctionPointer(methodResetHandle, typeof(ResetDelegate));
        }
        catch (Exception)
        {
            Debug.LogError("An error has occurred during loading Runtime.dll.");
            Unload();
            throw;
        }

        Debug.Log("Runtime.dll has been loaded");
    }

    public override void Unload()
    {
        Debug.Log("Unloading Runtime.dll");

        if (m_handle != IntPtr.Zero)
        {
            FreeLibrary(m_handle);
            m_handle = IntPtr.Zero;
        }

        m_runDelegate = null;
        m_runIoDelegate = null;
        m_resetDelegate = null;

        Debug.Log("Runtime.dll has been unloaded");
    }

    private delegate bool RunDelegate(string codeFileName, IntPtr logDelegate);
    private delegate bool RunIoDelegate(string codeFileName, IntPtr logDelegate, int inputsCount, float[] inputs, int outputsCount, ref float[] outputs);
    private delegate bool ResetDelegate(string codeFileName);

    private IntPtr m_handle = IntPtr.Zero;
    private RunDelegate m_runDelegate;
    private RunIoDelegate m_runIoDelegate;
    private ResetDelegate m_resetDelegate;
}

internal class CompilerDll : DllLoader
{
    public bool IsLoaded => m_handle != IntPtr.Zero;

    public bool Compile(string codeFileName, string code, Logger.LogDelegate log)
    {
        if (!IsLoaded)
        {
            Debug.LogWarning("Compiler.dll isn't loaded.");
            return false;
        }

        return m_compileDelegate(codeFileName, code, Marshal.GetFunctionPointerForDelegate(log));
    }

    public override void Load()
    {
        Debug.Log("Loading Compiler.dll");

        if (IsLoaded)
        {
            Debug.LogWarning("Compiler.dll is already loaded");
            return;
        }

        m_handle = LoadLibrary("Compiler.dll");
        if (m_handle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            IntPtr methodCompileHandle = GetProcAddress(m_handle, "Compile");
            if (methodCompileHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            m_compileDelegate = (CompileDelegate)Marshal.GetDelegateForFunctionPointer(methodCompileHandle, typeof(CompileDelegate));
        }
        catch (Exception)
        {
            Debug.LogError("An error has occurred during loading Compiler.dll.");
            Unload();
            throw;
        }

        Debug.Log("Compiler.dll has been loaded");
    }

    public override void Unload()
    {
        Debug.Log("Unloading Compiler.dll");

        if (m_handle != IntPtr.Zero)
        {
            FreeLibrary(m_handle);
            m_handle = IntPtr.Zero;
        }

        m_compileDelegate = null;

        Debug.Log("Compiler.dll has been unloaded");
    }

    private delegate bool CompileDelegate(string codeFileName, string code, IntPtr logDelegate);

    private IntPtr m_handle = IntPtr.Zero;
    private CompileDelegate m_compileDelegate;
}