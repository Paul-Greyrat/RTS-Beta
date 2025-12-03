



using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private GameObject m_PlacementOutLine;
    private BuildActionSO m_BuildAction;
    private Vector3Int[] m_HighlightPosition;
    private Tilemap m_WalkableTilemap;
    private Tilemap m_OverlayTilemap;
    private Tilemap[] m_UnreachableTilemap;

    private Color m_HightlightColor = new Color(0, 0.8f, 1, 0.4f);
    private Color m_BlockColor = new Color(1, 0.2f, 0, 0.8f);

    private Sprite m_PlacehoderTileSprite;



    public PlacementProcess(BuildActionSO buildAction, Tilemap walkableTilemap, Tilemap overlayTilemap, Tilemap[] UnreachableTilemap)
    {
        m_PlacehoderTileSprite = Resources.Load<Sprite>("Images/PlacehoderTileSprite");
        m_BuildAction = buildAction;
        m_WalkableTilemap = walkableTilemap;
        m_OverlayTilemap = overlayTilemap;
        m_UnreachableTilemap = UnreachableTilemap;
    }

    public void Update()
    {
        if (m_PlacementOutLine != null)
        {
            HighlightTiles(m_PlacementOutLine.transform.position);
        }

        if (GreyUtils.TryGetHoldPosition(out Vector3 worldPosition))
        {
            m_PlacementOutLine.transform.position = SnapToGrid(worldPosition);
        }
    }

    public void ShowPlacementOutLine()
    {
        m_PlacementOutLine = new GameObject("PlacementOutLine");
        var renderer = m_PlacementOutLine.AddComponent<SpriteRenderer>();
        renderer.sprite = m_BuildAction.PlacementSprite;
        renderer.color = new Color(1, 1, 1, 0.6f);
        renderer.sortingOrder = 999;
    }

    Vector3 SnapToGrid(Vector3 worldposition)
    {
        return new Vector3(Mathf.Round(worldposition.x), Mathf.Round(worldposition.y), 0);
    }

    void HighlightTiles(Vector3 outLinePosition)
    {
        Vector3Int buildingSize = m_BuildAction.BuildingSize;
        Vector3 PivotPosition = outLinePosition + m_BuildAction.OriginOffset;

        ClearHightlight();
        m_HighlightPosition = new Vector3Int[buildingSize.x * buildingSize.y];

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                m_HighlightPosition[x + y * buildingSize.x] = new Vector3Int((int)(PivotPosition.x + x), (int)(PivotPosition.y + y), 0);
            }
        }

        foreach ( var tilePos in m_HighlightPosition)
        {

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = m_PlacehoderTileSprite;

            if (CanPlaceTile(tilePos))
            {
                tile.color = m_HightlightColor;    
            }
            else
            {
                tile.color = m_BlockColor;               
                
            }
            m_OverlayTilemap.SetTile(tilePos, tile);
        }
        
    }   

    void ClearHightlight()
    {
        if(m_HighlightPosition == null) return;

        foreach (var tilePosition in m_HighlightPosition)
        {
            m_OverlayTilemap.SetTile(tilePosition, null);
        }
    }

    bool CanPlaceTile(Vector3Int TilePos)
    {
        return 
            m_WalkableTilemap.HasTile(TilePos) &&
            !IsInUnreachableTilemap(TilePos) &&
            !IsBlockedByGameobject(TilePos);
    }

    bool IsInUnreachableTilemap(Vector3Int TilePos)
    {
        foreach(var tilemap in m_UnreachableTilemap)
        {
            if(tilemap.HasTile(TilePos)) return true;
        }

        return false;
    }

    bool IsBlockedByGameobject(Vector3Int TilePos)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(TilePos + tileSize / 2, tileSize * 0.9f, 0);

        foreach (var collider in colliders)
        {
            var layer = collider.gameObject.layer;
            if ( layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }

        return false;  
    }
}     