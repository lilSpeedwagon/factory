using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConsumer : MonoBehaviour
{
    public int ConsumerId => g_idCounter;

    public int SourceId { get; set; }

    public int Power;

    public EnergyConsumer()
    {
        m_id = g_idCounter++;
        SourceId = -1;
    }

    private int m_id;

    private static int g_idCounter = 0;
}
