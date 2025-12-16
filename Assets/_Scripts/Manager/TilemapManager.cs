


using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SingertonManager<TilemapManager>
{
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTilemap;
    [SerializeField] private Tilemap[] m_UnreachableTilemap;

    [Header("Testing")]
    [SerializeField] private Transform m_StartTransform;
    [SerializeField] private Transform m_DestinationTransform;


    public Tilemap PathfindingTilemap => m_WalkableTilemap;

    private Pathfinding m_PathFinding;

    void Start()
    {
        m_PathFinding = new Pathfinding(
            this
        );
    }

    void Update()
    {
        m_PathFinding.FindPath(
            m_StartTransform.position, 
            m_DestinationTransform.position
        );
    }

    public bool CanWalkAtTile(Vector3Int TilePos)
    {
        return 
            m_WalkableTilemap.HasTile(TilePos) &&
            !IsInUnreachableTilemap(TilePos);
    }

    public bool CanPlaceTile(Vector3Int TilePos)
    {
        return 
            m_WalkableTilemap.HasTile(TilePos) &&
            !IsInUnreachableTilemap(TilePos) &&
            !IsBlockedByGameobject(TilePos);
    }

    public bool IsInUnreachableTilemap(Vector3Int TilePos)
    {
        foreach(var tilemap in m_UnreachableTilemap)
        {
            if(tilemap.HasTile(TilePos)) return true;
        }

        return false;
    }

    public bool IsBlockedByGameobject(Vector3Int TilePos)
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

    public void SetTileOverlay(Vector3Int tilePos, Tile tile)
    {
        m_OverlayTilemap.SetTile(tilePos, tile);
    }
}