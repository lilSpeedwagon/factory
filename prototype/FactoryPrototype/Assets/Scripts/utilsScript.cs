using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtils
{
    static GameObject m_player;
    public static GameObject Player
    {
        get
        {
            if (m_player == null)
            {
                m_player = GameObject.FindWithTag("Player");
            }
            return m_player;
        }
    }    
}

public class TileUtils
{
    private static GridLayout gridLayout = GameObject.FindWithTag("grid").GetComponent<GridLayout>();

    public enum Direction { UpLeft, UpRight, DownRight, DownLeft };
    public const int DIRECTION_COUNT = 4;
    public static readonly Quaternion qInitRotation = new Quaternion();
    // size of tile side
    public static readonly float tileSize = Mathf.Sqrt(Mathf.Pow(gridLayout.cellSize.x / 2, 2) + Mathf.Pow(gridLayout.cellSize.y / 2, 2));

    public static Vector2 MouseCellPosition()
    {
        Vector2 vMousePos = Input.mousePosition;
        if (gridLayout != null)
        {
            var vWorldPos = Camera.main.ScreenToWorldPoint(vMousePos);
            vWorldPos.y += gridLayout.cellSize.y / 2; // some tiles magic
            var vCellPos = gridLayout.WorldToCell(vWorldPos);
            return gridLayout.CellToWorld(vCellPos);
        }
        return new Vector2();
    }

    public static Vector2 CellPosition(Vector2 worldPosition)
    {
        if (gridLayout != null)
        {
            worldPosition.y += gridLayout.cellSize.y / 2; // some tiles magic
            var vCellPos = gridLayout.WorldToCell(worldPosition);
            return gridLayout.CellToWorld(vCellPos);
        }
        return new Vector2();
    }

    public static Vector2 LevelOffset(int zLevel)
    {
        return new Vector2(0, zLevel * gridLayout.cellSize.y / 2);
    }

}

public class TimeUtils
{
    public static IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}

public class InputUtils
{
    public const string buttonRotateObject = "RotateObject";
}

public class MouseUtils
{
    public const int PRIMARY_MOUSE_BUTTON = 0;
    public const int ALTER_MOUSE_BUTTON = 1;
}

public class ColorUtils
{
    public static Color colorTransparentRed = new Color(1.0f, 0.5f, 0.5f, 0.3f);
    public static Color colorTransparentGreen = new Color(0.5f, 1.0f, 0.5f, 0.3f);
}

public class LayerUtils
{
    public static int OBJECTS_LAYER = 9;
}

public class QuaternionUtils
{
    public static Quaternion qRotate90 = Quaternion.Euler(0, 0, 90);
}

public class Range
{
    public float from, to;

    public Range(float from, float to)
    {
        this.from = from;
        this.to = to;
    }
    public Range(float to)
    {
        from = 0;
        this.to = to;
    }

    public bool In(float value)
    {
        return from <= value && value < to;
    }

    public bool Less(float value)
    {
        return value < from;
    }

    public bool More(float value)
    {
        return value >= to;
    }
}

