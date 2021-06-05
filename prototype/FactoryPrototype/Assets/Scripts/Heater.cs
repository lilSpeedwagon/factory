using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heater : MonoBehaviour
{
    public float HeatingRate = 10.0f;
    public bool IsEnabled => GetComponent<EnergyConsumer>().IsEnergized;

    public IMover HeatedMover => TileManagerScript.TileManager
        .GetGameObject(GetComponent<TileObject>().GetNextPosition())?.GetComponent<IMover>();

    public float Temperature
    {
        get => m_temperature;
        private set
        {
            m_temperature = value;
            bool enableAnimation = m_temperature > AnimationThreshold;
            GetComponent<Animator>().SetBool("isEnabled", enableAnimation);
        }
    }

    private void Start()
    {
        var publisher = GetComponent<DataPublisher>();

        m_tempPort = new DataPublisher.DataPort("Temperature");
        publisher.SetPort(m_tempPort);
    }

    private void FixedUpdate()
    {
        Temperature = m_tempPort.CurrentValue.GetNumber();

        var material = HeatedMover?.Motion?.GetComponent<Material>();
        if (material != null)
        {
            if (material.Temperature < Temperature)
            {
                material.Temperature += HeatingRate;
            }
        }
    }

    private float m_temperature;
    private DataPublisher.DataPort m_tempPort;

    private const float AnimationThreshold = 25.0f;
}
