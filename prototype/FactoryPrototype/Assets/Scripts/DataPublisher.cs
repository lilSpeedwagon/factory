using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;


public class DataPublisher : MonoBehaviour
{
    public float PipeConnectionRadius = 1.0f;

    public class DataPort
    {
        public string Name;

        public DataValue CurrentValue
        {
            set => m_value = value;
            get
            {
                if (!m_isSource && m_otherEnd != null)
                {
                    return m_otherEnd.m_value;
                }
                return m_value;
            }
        }
        public bool IsPublisher => m_isSource && m_otherEnd != null;
        public bool IsConnected => m_otherEnd != null;

        public DataPort(string name)
        {
            Name = name;
            m_value = new DataValue(0.0f);
        }

        public static void WirePorts(DataPort from, DataPort to, LineRenderer line)
        {
            from.SetDestination(to);
            to.SetSource(from);

            from.m_line = line;
            to.m_line = line;
        }

        public void Reset()
        {
            if (m_otherEnd != null)
            {
                m_otherEnd.m_otherEnd = null;
                m_otherEnd.m_line = null;
            }
            m_otherEnd = null;

            if (m_line != null)
            {
                Destroy(m_line);
                m_line = null;
            }
        }

        private void SetSource(DataPort source)
        {
            m_otherEnd = source;
            m_isSource = false;
        }

        private void SetDestination(DataPort dest)
        {
            m_otherEnd = dest;
            m_isSource = true;
        }

        private DataPort m_otherEnd;
        private bool m_isSource;
        private LineRenderer m_line;
        private DataValue m_value;
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

    private readonly Dictionary<string, DataPort> m_ports = new Dictionary<string, DataPort>();
}

