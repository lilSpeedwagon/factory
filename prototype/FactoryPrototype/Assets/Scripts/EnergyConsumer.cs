using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConsumer : MonoBehaviour
{
    public int ConsumerId { get; }

    public int SourceId;
    public int Power;
    public bool IsEnergized => SourceId != EmptySourceId;

    public void Reset()
    {
        SourceId = EmptySourceId;
    }

    public EnergyConsumer()
    {
        ConsumerId = g_idCounter++;
        Reset();
    }

    private const int EmptySourceId = -1;
    private static int g_idCounter = 0;
}
