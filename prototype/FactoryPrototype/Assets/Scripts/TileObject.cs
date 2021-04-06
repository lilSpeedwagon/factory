using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TileObject : MonoBehaviour
{
    public bool isShadow = false;
    public int ZPosition = 1;
    public bool OnlyXFlip = false;

    public Sprite AlternativeImage;
    public AnimatorOverrideController AlternativeAnimation;

    public TileUtils.Direction Direction
    {
        get => m_dir;
        set
        {
            m_dir = value;

            if (!OnlyXFlip && (AlternativeImage == null || AlternativeAnimation == null))
            {
                switch (m_dir)
                {
                    case TileUtils.Direction.DownLeft:
                        m_sprite.flipX = true;
                        m_sprite.flipY = false;
                        break;
                    case TileUtils.Direction.DownRight:
                        m_sprite.flipX = false;
                        m_sprite.flipY = false;
                        break;
                    case TileUtils.Direction.UpLeft:
                        m_sprite.flipX = true;
                        m_sprite.flipY = true;
                        break;
                    case TileUtils.Direction.UpRight:
                        m_sprite.flipX = false;
                        m_sprite.flipY = true;
                        break;
                }
            }
            else
            {
                m_sprite.flipX = m_dir == TileUtils.Direction.DownLeft || m_dir == TileUtils.Direction.UpRight;
                ChangeSprite();
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

    private bool IsAlternativeSpriteNeeded()
    {
        return m_dir == TileUtils.Direction.UpLeft || m_dir == TileUtils.Direction.UpRight;
    }

    private void ChangeSprite()
    {
        if (AlternativeAnimation != null || AlternativeImage != null)
        {
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = m_isAlternative ? m_originalController : AlternativeAnimation;
            }

            GetComponent<SpriteRenderer>().sprite = m_isAlternative ? m_originalImage : AlternativeImage;
        }
    }

    private void Awake()
    {
        m_isAlternative = false;
        m_originalImage = GetComponent<SpriteRenderer>().sprite;
        m_sprite = GetComponent<SpriteRenderer>();

        var animator = GetComponent<Animator>();
        if (AlternativeAnimation != null && animator != null)
        {
            m_originalController = animator.runtimeAnimatorController;
        }
    }

    protected TileUtils.Direction m_dir = TileUtils.Direction.DownRight;
    protected ContactFilter2D m_filter = new ContactFilter2D();

    protected SpriteRenderer m_sprite;
    protected bool m_isAlternative;
    protected Sprite m_originalImage;
    protected RuntimeAnimatorController m_originalController;
}
