using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * Tile grid is represented as two-dimensional array with diagonal axis.
 * Every tile has size (2, 4) in cell metrics.
 */

public class TileManagerScript : MonoBehaviour
{
    private static TileManagerScript g_instance;
    public static TileManagerScript TileManager
    {
        get
        {
            if (g_instance == null)
                g_instance = GameObject.Find("TileManager").GetComponent<TileManagerScript>();
            return g_instance;
        }
    }

    public int Width => m_width;
    public int Height => m_height;

    public Vector2 CellSize = new Vector2Int(2, 4);

    public Vector2Int WorldToLocal(Vector2 worldCoords)
    {
        return new Vector2Int((int) (worldCoords.x * CellSize.x) - m_worldGridStart.x, (int) (worldCoords.y * CellSize.y) - m_worldGridStart.y);
    }
    public Vector2 LocalToWorld(Vector2Int localCoords)
    {
        return new Vector2(localCoords.x / (float) CellSize.x, localCoords.y / (float) CellSize.y) + m_worldGridStart;
    }

    public Vector2 WorldToCell(Vector2 worldPosition)
    {
        var cellPosition = m_layout.WorldToCell(worldPosition);
        return new Vector2(cellPosition.x * CellSize.x, cellPosition.y * CellSize.y);
    }

    public Vector2 CellToWorld(Vector2 cellPosition)
    {
        var worldPosition = m_layout.WorldToCell(cellPosition);
        return new Vector2(worldPosition.x / (float)CellSize.x, worldPosition.y / (float)CellSize.y);
    }

    private bool IsValidCoords(Vector2Int position)
    {
        return new Range(Width).In(position.x) && new Range(Height).In(position.y);
    }

    public bool RemoveObject(Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        if (IsValidCoords(localPos) && m_tiles[localPos.x, localPos.y].IsExist)
        {
            Destroy(m_tiles[localPos.x, localPos.y].Object);
            m_tiles[localPos.x, localPos.y].IsExist = false;
            return true;
        }

        return false;
    }

    public GameObject InstantiateObject(GameObject instance, Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        Debug.Log("Creating obj in pos " + position.ToString() + " (tile " + localPos.ToString() + ")");

        if (IsValidCoords(localPos) && m_tiles[localPos.x, localPos.y].IsExist)
        {
            throw new System.Exception("Cannot create object in pos " + localPos.x + " : " + localPos.y + ". Tile is not vacant.");
        }

        GameObject createdObject = Instantiate(instance, position, TileUtils.InitRotation);
        m_tiles[localPos.x, localPos.y].Object = createdObject;
        m_tiles[localPos.x, localPos.y].IsExist = true;

        return createdObject;
    }

    public bool IsEmpty(Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        return IsValidCoords(localPos) && !m_tiles[localPos.x, localPos.y].IsExist;
    }

    public GameObject GetGameObject(Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        if (!IsValidCoords(localPos) || !m_tiles[localPos.x, localPos.y].IsExist)
            return null;

        return  m_tiles[localPos.x, localPos.y].Object;
    }

    private void Awake()
    {
        m_worldGrid = GameObject.FindWithTag("grid").GetComponent<Grid>();
        m_layout = m_worldGrid.GetComponent<GridLayout>();

        m_width = BASE_WIDTH;
        m_height = BASE_HEIGHT;

        m_worldGridStart = new Vector2Int(-m_width / 2, -m_height / 2);

        m_tiles = new TileHolder[m_width, m_height];

        for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; j++)
            {
                m_tiles[i, j] = new TileHolder();
            }
        }
    }

    private class TileHolder
    {
        public GameObject Object { get; set; }
        public bool IsExist = false;
    }

    private TileHolder[,] m_tiles;
    private int m_width, m_height;
    private static int BASE_WIDTH = 50, BASE_HEIGHT = 50;
    private Vector2Int m_worldGridStart;
    private Grid m_worldGrid;
    private GridLayout m_layout;
}
