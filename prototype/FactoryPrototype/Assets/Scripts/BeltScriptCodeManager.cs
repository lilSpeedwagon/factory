using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;


// managing BeltScript code files
public class BeltScriptCodeManager
{
    // singleton
    private BeltScriptCodeManager()
    {
        InitCodeDirectory();

        m_scriptList = new Dictionary<string, string>();
        RefreshScriptList();
    }
    public static BeltScriptCodeManager Instance => g_instance ?? (g_instance = new BeltScriptCodeManager());

    public bool Compile(string scriptName, string code, Logger.LogDelegate log)
    {
        if (scriptName.Length == 0)
        {
            Debug.LogWarning("Attempt to call Compile with empty script name");
            return false;
        }

        string bltPath = CodeDirectory + scriptName + BeltScriptExtension;
        Debug.Log($"Compiling \"{bltPath}\"...");

        bool result = BeltScriptAdapter.Instance.Compile(bltPath, code, log);

        // save code file in the case of successful compilation
        if (result)
        {
            string sourceCodePath = CodeDirectory + scriptName + SourceCodeExtension;
            Debug.Log($"Saving code sources with name {sourceCodePath}");

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
        }
        else
        {
            Debug.Log("Compilation failed.");
        }

        return result;
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
                bltFiles.Add(file);
            }
            else if (file.EndsWith(SourceCodeExtension))
            {
                codeFiles.Add(file);
            }
        }

        foreach (var file in bltFiles)
        {
            if (codeFiles.Contains(file))
            {
                try
                {
                    m_scriptList.Add(file, GetCodeFileHash(CodeDirectory + file));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
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
                Debug.Log("Code directory has been created");
            }
            else
            {
                Debug.Log("Code directory already exists");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private bool IsBeltScriptFilesExist(string name)
    {
        string pureName = RemoveExtension(name);

        string codeFile = pureName + SourceCodeExtension;
        string beltFile = pureName + BeltScriptExtension;

        return File.Exists(codeFile) && File.Exists(beltFile);
    }

    static string RemoveExtension(string name)
    {
        int dotIndex = name.IndexOf('.');
        if (dotIndex == -1)
        {
            return name;
        }

        return name.Remove(dotIndex);
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

    // fileName -> hash
    private Dictionary<string, string> m_scriptList;

    private const string CodeDirectory = "./code/";
    private const string SourceCodeExtension = ".code";
    private const string BeltScriptExtension = ".blt";

    private static readonly SHA256 g_sha256 = SHA256.Create();

    private static BeltScriptCodeManager g_instance;
}
