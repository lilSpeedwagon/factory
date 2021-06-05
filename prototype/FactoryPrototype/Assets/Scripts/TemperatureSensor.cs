using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureSensor : MonoBehaviour
{
    public Sprite ActiveSprite;

    // scan the temperature of the object on the tile in front of sensor
    public Material DetectedMaterial
    {
        get
        {
            var baseObject = TileManagerScript.TileManager.GetGameObject(DetectablePosition);
            var mover = baseObject?.GetComponent<IMover>();
            var material = mover?.Motion?.GetComponent<Material>();
            return material;
        }
    }

    public Vector2 DetectablePosition => GetComponent<TileObject>().GetNextPosition();

    public bool IsEnabled => GetComponent<EnergyConsumer>().IsEnergized;

    private void ChangeSprite(bool isActive)
    {
        if (GetComponent<TileObject>().IsAlternativeSpriteUsed) return;

        var spriteRenderer = GetComponent<SpriteRenderer>();

        if (m_isActiveSpriteUsed && !isActive)
        {
            m_isActiveSpriteUsed = false;
            spriteRenderer.sprite = m_originalSprite;
        }
        else if (!m_isActiveSpriteUsed && isActive)
        {
            m_isActiveSpriteUsed = true;

            if (m_originalSprite == null)
            {
                m_originalSprite = spriteRenderer.sprite;
            }
            spriteRenderer.sprite = ActiveSprite;
        }
    }

    private void Start()
    {
        DataPublisher publisher = GetComponent<DataPublisher>();
        m_temperaturePort = new DataPublisher.DataPort("Temperature");
        publisher.SetPort(m_temperaturePort);
    }

    private void FixedUpdate()
    {
        var material = DetectedMaterial;
        float value = (material != null && IsEnabled) ? material.Temperature : 0.0f;
        m_temperaturePort.CurrentValue.SetValue(value);

        ChangeSprite(IsEnabled);
    }

    private bool m_isActiveSpriteUsed;
    private Sprite m_originalSprite;

    private DataPublisher.DataPort m_temperaturePort;
}
