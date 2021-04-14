using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public enum TileFlipType
    {
        NoFlip = 0,
        FlipXY = 1,
        FlipX = 2,
        FlipAlternativeAnimationX = 3
    }

    public bool IsShadow = false;
    public int ZPosition = 1;
    public TileFlipType FlipType = TileFlipType.NoFlip;
    public Sprite AlternativeSprite; // only for FlipAlternativeAnimationX
    public Sprite AlternativeMask; // only for FlipAlternativeAnimationX

    public TileUtils.Direction Direction
    {
        get => m_dir;
        set
        {
            m_dir = value;

            switch (FlipType)
            {
                case TileFlipType.NoFlip:
                    break;
                case TileFlipType.FlipXY:
                {
                    FlipX_Y();
                    break;
                }
                case TileFlipType.FlipX:
                {
                    bool flipped = m_dir == TileUtils.Direction.DownLeft || m_dir == TileUtils.Direction.UpRight;
                    m_spriteRenderer.flipX = flipped;
                    FlipMask = flipped;
                    break;
                }
                case TileFlipType.FlipAlternativeAnimationX:
                {
                    bool flipped = m_dir == TileUtils.Direction.DownLeft || m_dir == TileUtils.Direction.UpRight;
                    m_spriteRenderer.flipX = flipped;
                    FlipMask = flipped;

                    // use alternative sprite (and animation) for Y flip
                    bool alternative = IsAlternativeSpriteNeeded;
                    UseAlternativeSprite = alternative;
                    break;
                }
                default:
                {
                    Debug.LogWarning("Tile: Undefined flip type");
                    break;
                }
            }
        }
    }

    public Vector2 GetVector()
    {
        switch (m_dir)
        {
            case TileUtils.Direction.DownLeft:
                return new Vector2(-2.0f, -1.0f).normalized;                
            case TileUtils.Direction.DownRight:
                return new Vector2(2.0f, -1.0f).normalized;
            case TileUtils.Direction.UpLeft:
                return new Vector2(-2.0f, 1.0f).normalized;
            case TileUtils.Direction.UpRight:
                return new Vector2(2.0f, 1.0f).normalized;
            default:
                return new Vector2();
        }
    }

    public Vector2 GetPosition()
    {
        return (Vector2)transform.position - TileUtils.LevelOffset(ZPosition);
    }

    public Vector2 GetNextPosition()
    {
        return GetPosition() + GetVector() * TileUtils.TileSideSize;
    }

    public Vector2 GetPrevPosition()
    {
        return GetPosition() - GetVector() * TileUtils.TileSideSize;
    }

    public void Rotate(bool bClockwise = true)
    {
        TileUtils.Direction newDir;

        if (bClockwise)
        {
            newDir = m_dir + 1;
            if ((int)newDir >= TileUtils.DIRECTION_COUNT)
                newDir = TileUtils.Direction.UpLeft;
        }
        else
        {
            newDir = m_dir - 1;
            if ((int) newDir < 0)
                newDir = TileUtils.Direction.DownLeft;
        }

        Direction = newDir;
    }


    protected void FlipX_Y()
    {
        switch (m_dir)
        {
            case TileUtils.Direction.DownLeft:
                m_spriteRenderer.flipX = true;
                m_spriteRenderer.flipY = false;
                break;
            case TileUtils.Direction.DownRight:
                m_spriteRenderer.flipX = false;
                m_spriteRenderer.flipY = false;
                break;
            case TileUtils.Direction.UpLeft:
                m_spriteRenderer.flipX = true;
                m_spriteRenderer.flipY = true;
                break;
            case TileUtils.Direction.UpRight:
                m_spriteRenderer.flipX = false;
                m_spriteRenderer.flipY = true;
                break;
        }
    }

    protected bool FlipMask
    {
        set
        {
            if (m_mask == null)
                return;

            const float reversedScale = -1.0f;
            const float directScale = 1.0f;

            var scale = m_mask.transform.localScale;
            scale.x = value ? reversedScale : directScale;
            m_mask.transform.localScale = scale;
        }
    }

    protected bool UseAlternativeSprite
    {
        set
        {
            if (!m_isAlternativeSpriteUsed && value)
            {
                m_isAlternativeSpriteUsed = true;

                if (m_originalSprite == null)
                {
                    m_originalSprite = m_spriteRenderer.sprite;
                    m_originalMask = m_mask?.sprite;
                }
                m_spriteRenderer.sprite = AlternativeSprite;
                if (m_mask != null) m_mask.sprite = AlternativeMask;
            }
            else if (m_isAlternativeSpriteUsed && !value)
            {
                m_isAlternativeSpriteUsed = false;
                m_spriteRenderer.sprite = m_originalSprite;
                if (m_mask != null) m_mask.sprite = m_originalMask;
            }

            try
            {
                GetComponent<Animator>()?.SetBool("UseAlternative", value);
            }
            catch (MissingComponentException) { }
        }
    }

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_mask = GetComponentInChildren<SpriteMask>();
    }

    protected bool IsAlternativeSpriteNeeded => m_dir == TileUtils.Direction.UpLeft || m_dir == TileUtils.Direction.UpRight;

    protected TileUtils.Direction m_dir = TileUtils.Direction.DownRight;
    private bool m_isAlternativeSpriteUsed = false;
    private Sprite m_originalSprite;
    private Sprite m_originalMask;

    private SpriteRenderer m_spriteRenderer;
    private SpriteMask m_mask;
    //protected ContactFilter2D m_filter = new ContactFilter2D();
}
