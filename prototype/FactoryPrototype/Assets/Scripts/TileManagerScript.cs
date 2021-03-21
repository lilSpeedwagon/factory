using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private class TileHolder
    {        
        public GameObject Object { get; set; }
        public bool IsExist = false;
    }

    private Vector2Int WorldToLocal(Vector2 worldCoords)
    {
        return new Vector2Int((int) (worldCoords.x * TILES_PER_CELL.x) - m_worldGridStart.x, (int) (worldCoords.y * TILES_PER_CELL.y) - m_worldGridStart.y);
    }
    private Vector2 LocalToWorld(Vector2Int localCoords)
    {
        return new Vector2Int(localCoords.x / TILES_PER_CELL.x, localCoords.y / TILES_PER_CELL.y) + m_worldGridStart;
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

        GameObject createdObject = Instantiate(instance, position, TileUtils.qInitRotation);
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

    private TileHolder[,] m_tiles;
    private int m_width, m_height;
    private static int BASE_WIDTH = 50, BASE_HEIGHT = 50;
    private static Vector2Int TILES_PER_CELL = new Vector2Int(2, 4);    // no idea wtf is going on here (something related to Unity Grid cells)
    private Vector2Int m_worldGridStart;
    private Grid m_worldGrid;
}
