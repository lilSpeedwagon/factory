using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;


public class DataPublisher : MonoBehaviour
{
    public float PipeConnectionRadius = 1.0f;

    public class DataPipeRenderer
    {
        public Vector2 Position;
        public float Radius;
    }

    public class DataPort
    {
        public string Name;
        public DataValue CurrentValue;
        public bool IsPublisher => m_source == null && m_destination != null;
        public bool IsConnected => m_source != null || m_destination != null;

        public DataPort(string name)
        {
            Name = name;
        }

        public void SetSource(DataPort source)
        {
            m_destination = null;
            m_source = source;
        }

        public void SetDestination(DataPort dest)
        {
            m_destination = dest;
            m_source = null;
        }

        public void Reset()
        {
            m_destination = null;
            m_source = null;
        }

        private DataPort m_source;
        private DataPort m_destination;
    }

    public void SetPort(DataPort port)
    {
        m_ports[port.Name] = port;
    }

    public DataPort GetPort(string name)
    {
        return m_ports[name];
    }

    public bool RemovePort(string name)
    {
        return m_ports.Remove(name);
    }

    public List<DataPort> PortList => m_ports.Values.ToList();

    public int PortCount => m_ports.Count;

    private void Start()
    {
        m_renderer = new DataPipeRenderer { Radius = PipeConnectionRadius, Position = transform.position };
    }

    private readonly Dictionary<string, DataPort> m_ports = new Dictionary<string, DataPort>();
    private DataPipeRenderer m_renderer;
}

