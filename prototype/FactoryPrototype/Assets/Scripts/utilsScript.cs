using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public static GridLayout GridLayout = GameObject.FindWithTag("grid").GetComponent<GridLayout>();

    public enum Direction { UpLeft, UpRight, DownRight, DownLeft };
    public const int DIRECTION_COUNT = 4;
    public static readonly Quaternion InitRotation = new Quaternion();

    public static readonly Vector2 TileSizeRelative = new Vector2(1.0f, 0.5f);
    public static readonly Vector2 CellSize = GridLayout.cellSize;
    public static readonly float TileSideSize = Mathf.Sqrt(Mathf.Pow(GridLayout.cellSize.x / 2, 2) + Mathf.Pow(GridLayout.cellSize.y / 2, 2));

    public static Vector2 NormalizedMousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return NormalizedPosition(worldPos);
    }

    public static Vector2 NormalizedPosition(Vector2 worldPosition)
    {
        if (GridLayout != null)
        {
            var cellPos = GridLayout.WorldToCell(worldPosition);
            return GridLayout.CellToWorld(cellPos) + new Vector3(0, CellSize.y / 2);
        }
        return new Vector2();
    }

    public static Vector2 LevelOffset(int zLevel)
    {
        return new Vector2(0, zLevel * GridLayout.cellSize.y / 2);
    }

    public static Direction GetReversedDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.UpLeft:
                return Direction.DownRight;
            case Direction.DownRight:
                return Direction.UpLeft;
            case Direction.DownLeft:
                return Direction.UpRight;
            case Direction.UpRight:
                return Direction.DownLeft;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid TileUtils.Direction value");
        }
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
    public static Color GetRandomColor()
    {
        const int colorsCount = 10;
        int r = (int) (Random.value * colorsCount);

        switch (r)
        {
            case 0:
                return Color.blue;
            case 1:
                return Color.black;
            case 2:
                return Color.clear;
            case 3:
                return Color.cyan;
            case 4:
                return Color.gray;
            case 5:
                return Color.green;
            case 6:
                return Color.grey;
            case 7:
                return Color.magenta;
            case 8:
                return Color.red;
            case 9:
                return Color.yellow;
            default:
                return Color.white;
        }
    }
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

public class LogUtils
{
    public class DebugLogger
    {
        public DebugLogger(string name = "")
        {
            Name = name;
        }

        public string Name;

        public void Log(string message)
        {
            Debug.Log(Name + ": " + message);
        }

        public void Warn(string message)
        {
            Debug.LogWarning(Name + ": " + message);
        }

        public void Error(string message)
        {
            Debug.LogError(Name + ": " + message);
        }

    }
}

public class GameObjectUtils
{
    public static void SetActiveForChildren(GameObject obj, bool isActive)
    {
        obj.SetActive(isActive);
        foreach (Transform t in obj.GetComponent<Transform>())
        {
            if (t != obj)
                t.gameObject?.SetActive(isActive);
        }
    }
}