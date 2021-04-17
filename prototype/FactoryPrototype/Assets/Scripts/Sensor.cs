using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public Sprite ActiveSprite;

    // scan the tile in front of sensor for a presence of some conveyer belt
    // or processor with a material on it
    public bool IsMaterialDetected
    {
        get
        {
            var baseObject = TileManagerScript.TileManager.GetGameObject(DetectablePosition);
            var mover = baseObject?.GetComponent<IMover>();
            return !mover?.IsFree() ?? false;
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
        m_presencePort = new DataPublisher.DataPort("IsDetected");
        publisher.SetPort(m_presencePort);
    }

    private void FixedUpdate()
    {
        bool isMaterialDetected = IsEnabled && IsMaterialDetected;
        m_presencePort.CurrentValue = new DataValue(isMaterialDetected);
        ChangeSprite(IsEnabled);
    }

    private bool m_isActiveSpriteUsed;
    private Sprite m_originalSprite;

    private DataPublisher.DataPort m_presencePort;
}
