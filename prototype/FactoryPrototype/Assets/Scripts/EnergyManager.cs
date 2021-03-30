using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance
    {
        get
        {
            if (g_instance == null)
            {
                g_instance = GameObject.Find("EnergyManager").GetComponent<EnergyManager>();
            }
            return g_instance;
        }
    }

    public Text EnergyValueLabel;

    public int CurrentConsumption => m_currentConsumption;
    public int MaxConsumption => m_energyStorage;

    public void AddSource(EnergySource source)
    {
        if (m_energySources.ContainsKey(source.SourceId)) return;

        m_energySources.Add(source.SourceId, source);
        m_energyStorage += source.Power;
        m_currentConsumption += source.CurrentConsumption;
        AddPendingConsumersToSource(source);
        OnEnergyUpdated();

        m_logger.Log($"source {source.SourceId} was added");
    }

    public void RemoveSource(EnergySource source)
    {
        if (!m_energySources.ContainsKey(source.SourceId)) return;
        
        m_energySources.Remove(source.SourceId);
        m_energyStorage -= source.Power;
        m_currentConsumption -= source.CurrentConsumption;

        // make a copy to reset source without loosing consumer list
        var consumers = new List<EnergyConsumer>(source.Consumers.Values.ToList());
        source.Reset();

        // try to distribute consumers between other sources
        foreach (var consumer in consumers)
        {
            AddConsumer(consumer);
        }

        OnEnergyUpdated();
        m_logger.Log($"source {source.SourceId} was removed");
    }

    public void AddConsumer(EnergyConsumer consumer)
    {
        m_logger.Log($"Trying to find energy source for consumer {consumer.ConsumerId}");
        // looking for convenient energy source
        foreach (var source in m_energySources.Values)
        {
            if (AddConsumerToSource(source, consumer))
            {
                return;
            }
        }

        // add consumer to queue if source is not found
        m_logger.Log($"Available source is not found for consumer {consumer.ConsumerId}. Adding to queue.");
        m_pendingConsumers.Add(consumer);
    }

    public void RemoveConsumer(EnergyConsumer consumer)
    {
        try
        {
            RemoveConsumerFromSource(m_energySources[consumer.SourceId], consumer);
        }
        catch (KeyNotFoundException) { }

        m_pendingConsumers.Remove(consumer);
    }

    private void AddPendingConsumersToSource(EnergySource source)
    {
        // backward iteration to avoid list invalidation
        for (int i = m_pendingConsumers.Count - 1; i >= 0; i--)
        {
            if (AddConsumerToSource(source, m_pendingConsumers[i]))
            {
                m_pendingConsumers.RemoveAt(i);
            }
        }
    }

    private bool AddConsumerToSource(EnergySource source, EnergyConsumer consumer)
    {
        if (source.AddConsumer(consumer))
        {
            m_currentConsumption += consumer.Power;
            OnEnergyUpdated();
            m_logger.Log($"consumer {consumer.ConsumerId} was added to source {source.SourceId}");
            return true;
        }

        return false;
    }

    private bool RemoveConsumerFromSource(EnergySource source, EnergyConsumer consumer)
    {
        if (source.RemoveConsumer(consumer.ConsumerId))
        {
            m_currentConsumption -= consumer.Power;
            OnEnergyUpdated();
            m_logger.Log($"consumer {consumer.ConsumerId} was removed from source {source.SourceId}");
            return true;
        }

        return false;
    }

    private void OnEnergyUpdated()
    {
        if (EnergyValueLabel != null)
        {
            EnergyValueLabel.text = $"{m_currentConsumption} / {m_energyStorage}";
        }
    }

    private void Start()
    {
        m_logger = new LogUtils.DebugLogger("Energy manager");

        m_energySources = new Dictionary<int, EnergySource>(20);
        m_pendingConsumers = new List<EnergyConsumer>(10);
        m_currentConsumption = 0;
        m_energyStorage = 0;
    }

    private List<EnergyConsumer> m_pendingConsumers;
    private Dictionary<int, EnergySource> m_energySources;

    private int m_energyStorage;
    private int m_currentConsumption;

    private LogUtils.DebugLogger m_logger;

    private static EnergyManager g_instance;
}
