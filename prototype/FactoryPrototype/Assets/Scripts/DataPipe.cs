using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;


public class DataPipeRenderer
{
    public Vector2 Position;
    public float PipeConnectionRadius;
}

public class DataPublisher : MonoBehaviour
{
    DataPublisher(DataPipeRenderer renderer)
    {
        m_renderer = renderer;
    }

    public void AddPort(DataPort<float> port)
    {
        m_numPorts.Add(port.Name, port);
    }

    public void AddPort(DataPort<bool> port)
    {
        m_boolPorts.Add(port.Name, port);
    }

    public void AddPort(DataPort<string> port)
    {
        m_strPorts.Add(port.Name, port);
    }

    public DataPort<float> GetPort(string name, float param)
    {
        return m_numPorts[name];
    }

    public DataPort<bool> GetPort(string name, bool param)
    {
        return m_boolPorts[name];
    }

    public DataPort<string> GetPort(string name, string param)
    {
        return m_strPorts[name];
    }

    public void RemovePort(string name)
    {
        m_numPorts.Remove(name);
        m_boolPorts.Remove(name);
        m_strPorts.Remove(name);
    }

    public List<string> GetPortList()
    {
        List<string> portList = new List<string>(PortCount);

       portList.AddRange(m_numPorts.Keys);
       portList.AddRange(m_boolPorts.Keys);
       portList.AddRange(m_strPorts.Keys);

        return portList;
    }

    public int PortCount => m_numPorts.Count + m_boolPorts.Count + m_strPorts.Count;


    private readonly Dictionary<string, DataPort<float>> m_numPorts = new Dictionary<string, DataPort<float>>();
    private readonly Dictionary<string, DataPort<bool>> m_boolPorts = new Dictionary<string, DataPort<bool>>();
    private readonly Dictionary<string, DataPort<string>> m_strPorts = new Dictionary<string, DataPort<string>>();

    private DataPipeRenderer m_renderer;
}

public class DataPort<T>
{
    public string Name;
    public T CurrentValue;
    public bool IsPublisher => m_source == null && m_destination != null;
    public bool IsConnected => m_source != null || m_destination != null;

    public DataPort(string name)
    {
        Name = name;
    }

    public void SetSource(DataPort<T> source)
    {
        m_destination = null;
        m_source = source;
    }

    public void SetDestination(DataPort<T> dest)
    {
        m_destination = dest;
        m_source = null;
    }

    public void Reset()
    {
        m_destination = null;
        m_source = null;
    }

    private DataPort<T> m_source;
    private DataPort<T> m_destination;
}

