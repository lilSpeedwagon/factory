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

    public TileUtils.Direction Direction
    {
        get => m_dir;
        set
        {
            m_dir = value;
            var sprite = GetComponent<SpriteRenderer>();

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
                    sprite.flipX = m_dir == TileUtils.Direction.DownLeft || m_dir == TileUtils.Direction.UpRight;
                    break;
                }
                case TileFlipType.FlipAlternativeAnimationX:
                {
                    sprite.flipX = m_dir == TileUtils.Direction.DownLeft || m_dir == TileUtils.Direction.UpRight;
                    // used for changing animations in objects like conveyer
                    bool alternative = IsAlternativeSpriteNeeded;

                    try
                    {
                        GetComponent<Animator>()?.SetBool("UseAlternative", alternative);
                    }
                    catch (MissingComponentException) { }

                    var spriteRenderer = GetComponent<SpriteRenderer>();
                    if (!m_isAlternativeSpriteUsed && alternative)
                    {
                        m_isAlternativeSpriteUsed = true;

                        if (m_originalSprite == null)
                        {
                            m_originalSprite = spriteRenderer.sprite;
                        }
                        spriteRenderer.sprite = AlternativeSprite;
                    }
                    else if (m_isAlternativeSpriteUsed && !alternative)
                    {
                        m_isAlternativeSpriteUsed = false;

                        spriteRenderer.sprite = m_originalSprite;
                    }
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
        var sprite = GetComponent<SpriteRenderer>();
        switch (m_dir)
        {
            case TileUtils.Direction.DownLeft:
                sprite.flipX = true;
                sprite.flipY = false;
                break;
            case TileUtils.Direction.DownRight:
                sprite.flipX = false;
                sprite.flipY = false;
                break;
            case TileUtils.Direction.UpLeft:
                sprite.flipX = true;
                sprite.flipY = true;
                break;
            case TileUtils.Direction.UpRight:
                sprite.flipX = false;
                sprite.flipY = true;
                break;
        }
    }

    protected bool IsAlternativeSpriteNeeded => m_dir == TileUtils.Direction.UpLeft || m_dir == TileUtils.Direction.UpRight;

    protected TileUtils.Direction m_dir = TileUtils.Direction.DownRight;
    private bool m_isAlternativeSpriteUsed = false;
    private Sprite m_originalSprite;
    //protected ContactFilter2D m_filter = new ContactFilter2D();
}
