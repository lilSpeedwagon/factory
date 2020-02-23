using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagerScript : MonoBehaviour
{
    public Grid m_worldGrid;

    public int Width
    {
        get
        {
            return m_width;
        }
    }
    public int Height
    {
        get
        {
            return m_height;
        }
    }

    private int m_width, m_height;
    private static int BASE_WIDTH = 50, BASE_HEIGHT = 50;
    private static Vector2Int TILES_PER_CELL = new Vector2Int(2, 4);    // no idea wtf is going on here (something related to Unity Grid cells)
    private Vector2Int m_worldGridStart;

    private class TileHolder
    {        
        public GameObject m_gameObject = null;
        public bool m_bExists = false;
    }

    TileHolder[,] m_tiles;

    private Vector2Int WorldToLocal(Vector2 worldCoords)
    {
        return new Vector2Int((int) (worldCoords.x * TILES_PER_CELL.x) - m_worldGridStart.x, (int) (worldCoords.y * TILES_PER_CELL.y) - m_worldGridStart.y);
    }
    private Vector2 LocalToWorld(Vector2Int localCoords)
    {
        return new Vector2Int(localCoords.x / TILES_PER_CELL.x, localCoords.y / TILES_PER_CELL.y) + m_worldGridStart;
    }

    private void ValidateCoords(Vector2Int position)
    {
        if (!new Range(Width).In(position.x) || !new Range(Height).In(position.y))
        {
            throw new System.IndexOutOfRangeException();
        }
    }

    public void RemoveObject(Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        ValidateCoords(localPos);
        if (m_tiles[localPos.x, localPos.y].m_bExists)
        {
            Destroy(m_tiles[localPos.x, localPos.y].m_gameObject);
            m_tiles[localPos.x, localPos.y].m_bExists = false;
        }
    }

    public GameObject InstantiateObject(GameObject instance, Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        Debug.Log("Creating obj in pos " + position.ToString() + " (tile " + localPos.ToString() + ")");

        ValidateCoords(localPos);

        if (m_tiles[localPos.x, localPos.y].m_bExists)
        {
            throw new System.Exception("Cannot create object in pos " + localPos.x + " : " + localPos.y + ". Tile is not vacant.");
        }

        GameObject createdObject = Instantiate(instance, position, TileUtils.qInitRotation);
        m_tiles[localPos.x, localPos.y].m_gameObject = createdObject;
        m_tiles[localPos.x, localPos.y].m_bExists = true;

        return createdObject;
    }

    public bool IsEmpty(Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        try
        {
            ValidateCoords(localPos);
        }
        catch (System.Exception)
        {
            return false;
        }
        return !m_tiles[localPos.x, localPos.y].m_bExists;        
    }

    public GameObject GetGameObject(Vector2 position)
    {
        Vector2Int localPos = WorldToLocal(position);
        try
        {
            ValidateCoords(localPos);
        }
        catch (System.Exception)
        {
            return null;
        }
        return m_tiles[localPos.x, localPos.y].m_gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
