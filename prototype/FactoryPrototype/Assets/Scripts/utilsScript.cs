using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUtils
{
    public enum Direction { UpLeft, UpRight, DownRight, DownLeft };
    public const int DIRECTION_COUNT = 4;
    public static Quaternion qInitRotation = new Quaternion();
}

public class InputUtils
{
    public const string buttonRotateObject = "RotateObject";
}

public class MouseUtils
{
    private static GridLayout gridLayout = GameObject.FindWithTag("grid").GetComponent<GridLayout>();

    public const int PRIMARY_MOUSE_BUTTON = 0;

    public static Vector2 MouseCellPosition()
    {
        Vector2 vMousePos = Input.mousePosition;
        if (gridLayout != null)
        {
            var vWorldPos = Camera.main.ScreenToWorldPoint(vMousePos);
            var vCellPos = gridLayout.WorldToCell(vWorldPos);
            var vWorldCellPos = gridLayout.CellToWorld(vCellPos);
            return new Vector2(vWorldCellPos.x, vWorldCellPos.y + gridLayout.cellSize.y / 2);
        }
        return new Vector2();
    }
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
}

