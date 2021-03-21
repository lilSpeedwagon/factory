using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;

public class EnergySource : MonoBehaviour
{
    public int SourceId => m_id;

    public float EnergyDistributionCellRadius;

    public int Power;

    public int CurrentConsumption => m_consumption;
    public Dictionary<int, EnergyConsumer> Consumers => m_consumers;

    public EnergySource()
    {
        m_id = g_idCounter++;
        m_consumption = 0;
        m_consumers = new Dictionary<int, EnergyConsumer>(10);
    }

    public bool IsEnoughPower(EnergyConsumer consumer)
    {
        return m_consumption + consumer.Power <= Power;
    }

    public bool AddConsumer(EnergyConsumer consumer)
    {
        if (!IsInArea(consumer) || !IsEnoughPower(consumer) || m_consumers.ContainsKey(consumer.ConsumerId)) return false;
        
        m_consumers.Add(consumer.ConsumerId, consumer);
        m_consumption += consumer.Power;
        consumer.SourceId = SourceId;
        return true;
    }

    public bool RemoveConsumer(int consumerId)
    {
        try
        {
            EnergyConsumer consumer = m_consumers[consumerId];
            m_consumers.Remove(consumerId);
            m_consumption -= consumer.Power;
            consumer.SourceId = -1;
            return true;
        }
        catch (KeyNotFoundException) { }

        return false;
    }


    public bool IsInArea(EnergyConsumer consumer)
    {
        Vector2 consumerPosition = TileUtils.CellPosition(consumer.GetComponent<Transform>().position);
        Vector2 position = TileUtils.CellPosition(gameObject.GetComponent<Transform>().position);
        Vector2 diff = position - consumerPosition;
        return diff.magnitude <= EnergyDistributionCellRadius;
    }

    private int m_id;
    private readonly Dictionary<int, EnergyConsumer> m_consumers;
    private int m_consumption;

    private static int g_idCounter = 0;
}
