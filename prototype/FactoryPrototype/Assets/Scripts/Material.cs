using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;


public class Material : MonoBehaviour
{
    [SerializeField]
    private int m_cost = 0;
    public int Cost => m_cost;

    public int SellCost;

    public string Name;

    public Sprite Image;

    public float Temperature
    {
        get => m_temperature;
        set
        {
            m_temperature = value;
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

            var color = spriteRenderer.color;
            float redScale = 1.0f - m_temperature / MaxTemperature;
            color.g = redScale;
            color.b = redScale;
            spriteRenderer.color = color;
        }
    }

    private void FixedUpdate()
    {
        const float resolution = 0.5f;
        const float delta = 1.0f;
        if (Temperature > NormalTemperature + resolution)
        {
            Temperature -= delta;
        }
        else if (Temperature < NormalTemperature - resolution)
        {
            Temperature += delta;
        }
    }

    private float m_temperature;
    private const float NormalTemperature = 25.0f;
    private const float MaxTemperature = 2000.0f;
}
