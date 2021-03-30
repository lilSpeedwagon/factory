using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.Tilemaps;

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
            consumer.Reset();
            return true;
        }
        catch (KeyNotFoundException) { }

        return false;
    }

    public void Reset()
    {
        foreach (var consumer in m_consumers.Values)
        {
            consumer.Reset();
        }
        m_consumers.Clear();
        m_consumption = 0;
    }

    public bool IsInArea(EnergyConsumer consumer)
    {
        Vector2 consumerPosition = consumer.GetComponent<Transform>().position;
        return IsInArea(consumerPosition);
    }

    public bool IsInArea(Vector2 position)
    {
        Vector2 cellPosition = TileManagerScript.TileManager.WorldToCell(position);
        Vector2 sourcePosition = TileManagerScript.TileManager.WorldToCell(GetComponent<Transform>().position);
        Vector2 diff = sourcePosition - cellPosition;
        Vector2 radiusRelative = TileManagerScript.TileManager.CellSize * EnergyDistributionCellRadius;
        return Math.Abs(diff.x) <= radiusRelative.x && Math.Abs(diff.y) <= radiusRelative.y;
    }

    private readonly int m_id;
    private readonly Dictionary<int, EnergyConsumer> m_consumers;
    private int m_consumption;

    private static int g_idCounter = 0;
}
