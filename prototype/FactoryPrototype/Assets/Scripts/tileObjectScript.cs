using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileObjectScript : MonoBehaviour
{
    public bool isShadow = false;
    public int ZPosition = 1;
    public bool OnlyXFlip = false;

    public TileUtils.Direction direction
    {
        get => m_dir;
        set
        {
            m_dir = value;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();

            if (!OnlyXFlip)
            {
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
            else
            {
                sprite.flipX = m_dir == TileUtils.Direction.DownLeft || m_dir == TileUtils.Direction.UpRight;                
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

    public Vector2 GetNextPostion()
    {
        return GetPosition() + GetVector() * TileUtils.tileSize;
    }

    public Vector2 GetPrevPostion()
    {
        return GetPosition() - GetVector() * TileUtils.tileSize;
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

        direction = newDir;
    }

    protected TileUtils.Direction m_dir = TileUtils.Direction.DownRight;
    protected ContactFilter2D m_filter = new ContactFilter2D();
}
