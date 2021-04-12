using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

    public List<Tile> TilePalette;

    public int Width => m_width;
    public int Height => m_height;

    public Vector2 CellSize = new Vector2Int(2, 4);

    public Vector2Int WorldToLocal(Vector2 worldCoords)
    {
        return new Vector2Int((int) (worldCoords.x * CellSize.x) - m_worldGridStart.x, (int) (worldCoords.y * CellSize.y) - m_worldGridStart.y);
    }
    public Vector2 LocalToWorld(Vector2Int localCoords)
    {
        return new Vector2((localCoords.x + m_worldGridStart.x) / (float) CellSize.x, (localCoords.y + m_worldGridStart.y) / (float) CellSize.y);
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

    public bool IsValidCoords(Vector2 worldPosition)
    {
        return IsValidCoords(WorldToLocal(worldPosition));
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

    public void SetTile(Vector2 worldPosition, Tile tile = null)
    {
        Vector3Int cellPosition = m_tileMap.WorldToCell(worldPosition);
        if (tile == null)
        {
            tile = GetRandomTileFromPalette();
        }
        m_tileMap.SetTile(cellPosition, tile);
    }

    private Tile GetRandomTileFromPalette()
    {
        int randomIndex = (int) (Random.value * TilePalette.Count);
        return TilePalette[randomIndex];
    }

    private void BuildTileField()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Vector2 worldPosition = LocalToWorld(new Vector2Int(j, i));
                SetTile(worldPosition);
            }
        }
    }

    private void Start()
    {
        m_worldGrid = GameObject.FindWithTag("grid").GetComponent<Grid>();
        m_layout = m_worldGrid.GetComponent<GridLayout>();
        m_tileMap = GameObject.Find("baseTilemap").GetComponent<Tilemap>();

        m_width = BaseWidth;
        m_height = BaseHeight;

        m_worldGridStart = new Vector2Int(-m_width / 2, -m_height / 2);

        m_tiles = new TileHolder[m_width, m_height];

        for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; j++)
            {
                m_tiles[i, j] = new TileHolder();
            }
        }

        BuildTileField();
    }

    private class TileHolder
    {
        public GameObject Object { get; set; }
        public bool IsExist = false;
    }

    private TileHolder[,] m_tiles;
    private int m_width, m_height;
    private Vector2Int m_worldGridStart;
    private Grid m_worldGrid;
    private GridLayout m_layout;
    private Tilemap m_tileMap;

    private const int BaseWidth = 50;
    private const int BaseHeight = 50;
}
