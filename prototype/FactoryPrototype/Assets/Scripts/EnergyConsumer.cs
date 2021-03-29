using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConsumer : MonoBehaviour
{
    public int ConsumerId { get; }

    public int SourceId;
    public int Power;

    public EnergyConsumer()
    {
        ConsumerId = g_idCounter++;
        SourceId = -1;
    }

    private static int g_idCounter = 0;
}
