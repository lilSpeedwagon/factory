using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;


// managing BeltScript code files
public class BeltScriptCodeManager
{
    public static void ForceInit()
    {
        Debug.Log("BeltScript force init");
        var beltScriptCodeManager = Instance;
        var beltScriptAdapter = BeltScriptAdapter.Instance;
    }

    // singleton
    private BeltScriptCodeManager()
    {
        m_logger = new LogUtils.DebugLogger("BeltScriptCodeManager");
        InitCodeDirectory();

        m_scriptList = new Dictionary<string, string>();
        RefreshScriptList();
    }
    public static BeltScriptCodeManager Instance => g_instance ?? (g_instance = new BeltScriptCodeManager());

    public bool Run(string scriptName, Logger.LogDelegate log)
    {
        string bltPath = GetBeltScriptPath(scriptName);
        m_logger.Log($"Running {bltPath}");
        return BeltScriptAdapter.Instance.Run(bltPath, log);
    }

    public bool RunIo(string scriptName, Logger.LogDelegate log, float[] inputs, ref float[] outputs)
    {
        string bltPath = GetBeltScriptPath(scriptName);
        m_logger.Log($"Running IO {bltPath}");
        return BeltScriptAdapter.Instance.RunIo(bltPath, inputs, ref outputs, log);
    }

    public bool Compile(string scriptName, string code, Logger.LogDelegate log)
    {
        if (scriptName.Length == 0)
        {
            m_logger.Warn("Attempt to call Compile with empty script name");
            return false;
        }

        string bltPath = GetBeltScriptPath(scriptName);
        m_logger.Log($"Compiling \"{bltPath}\"...");

        bool result = BeltScriptAdapter.Instance.Compile(bltPath, code, log);

        // save code file in the case of successful compilation
        if (result)
        {
            string sourceCodePath = GetSourceCodePath(scriptName);
            m_logger.Log($"Saving code sources with name {sourceCodePath}");

            try
            {
                StreamWriter fileStream = File.CreateText(sourceCodePath);
                fileStream.Write(code);
                fileStream.Close();

                // update or add script hash to the list
                m_scriptList[scriptName] = GetHashString(code);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            // remove cashed code from Runtime.dll
            BeltScriptAdapter.Instance.Reset(bltPath);
        }
        else
        {
            m_logger.Log("Compilation failed.");
        }

        return result;
    }

    public string LoadSourceCode(string scriptName)
    {
        string sourceCodePath = GetSourceCodePath(scriptName);

        try
        {
            return File.ReadAllText(sourceCodePath);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        return "";
    }

    public void DeleteScript(string scriptName)
    {
        if (!m_scriptList.ContainsKey(scriptName))
        {
            Debug.LogWarning($"No such script '{scriptName}' found");
        }
        
        try
        {
            File.Delete(GetSourceCodePath(scriptName));
            File.Delete(GetBeltScriptPath(scriptName));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        m_scriptList.Remove(scriptName);
    }

    public string GetHashForScript(string scriptName)
    {
        string hash = "";

        if (m_scriptList.ContainsKey(scriptName))
        {
            hash = m_scriptList[scriptName];
        }
        else
        {
            try
            {
                hash = GetCodeFileHash(GetSourceCodePath(scriptName));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        return hash;
    }

    // return list of file names
    public IEnumerable<string> ScriptList => m_scriptList.Select(pair => pair.Key);

    private void RefreshScriptList()
    {
        m_scriptList.Clear();

        string[] files = Directory.GetFiles(CodeDirectory);

        int expectedSize = files.Length / 2;
        List<string> bltFiles = new List<string>(expectedSize);
        List<string> codeFiles = new List<string>(expectedSize);

        foreach (var file in files)
        {
            if (file.EndsWith(BeltScriptExtension))
            {
                bltFiles.Add(GetScriptName(file));
            }
            else if (file.EndsWith(SourceCodeExtension))
            {
                codeFiles.Add(GetScriptName(file));
            }
        }
        
        foreach (var file in bltFiles)
        {
            if (codeFiles.Contains(file))
            {
                string hash = GetHashForScript(file);
                m_scriptList.Add(file, hash);
            }
        }
    }

    private void InitCodeDirectory()
    {
        try
        {
            if (!Directory.Exists(CodeDirectory))
            {
                Directory.CreateDirectory(CodeDirectory);
                Debug.Log("BeltScriptCodeManager: Code directory has been created");
            }
            else
            {
                Debug.Log("BeltScriptCodeManager: Code directory already exists");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private bool IsBeltScriptFilesExist(string scriptName)
    {
        return File.Exists(GetSourceCodePath(scriptName)) && File.Exists(GetBeltScriptPath(scriptName));
    }

    static string RemoveExtension(string name)
    {
        int lastDotIndex = name.LastIndexOf('.');

        if (lastDotIndex != -1)
        {
            bool gotSlashesAfterDot = (name.IndexOf('/', lastDotIndex) != -1) || (name.IndexOf('\\', lastDotIndex) != -1);
            if (!gotSlashesAfterDot)
            {
                return name.Remove(lastDotIndex);
            }
        }

        return name;
    }

    static string RemovePath(string path)
    {
        int lastSlashIndex = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
        return path.Substring(lastSlashIndex + 1);
    }

    static string GetCodeFileHash(string filePath)
    {
        return GetHashString(File.ReadAllText(filePath));
    }

    static string GetHashString(string data)
    {
        byte[] hash = g_sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

        var sBuilder = new StringBuilder();
        foreach (byte b in hash)
        {
            sBuilder.Append(b.ToString("x2"));
        }
        
        return sBuilder.ToString();
    }

    static string GetScriptName(string path) => RemoveExtension(RemovePath(path));
    static string GetSourceCodePath(string name) => CodeDirectory + name + SourceCodeExtension;
    static string GetBeltScriptPath(string name) => CodeDirectory + name + BeltScriptExtension;

    // fileName -> hash
    private Dictionary<string, string> m_scriptList;

    private const string CodeDirectory = "./code/";
    private const string SourceCodeExtension = ".code";
    private const string BeltScriptExtension = ".blt";

    private LogUtils.DebugLogger m_logger;

    private static readonly SHA256 g_sha256 = SHA256.Create();

    private static BeltScriptCodeManager g_instance;
}
