



using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private GameObject m_PlacementOutLine;
    private BuildActionSO m_BuildAction;
    private Vector3Int[] m_HighlightPosition;
    private TilemapManager m_TilemapManager;

    private Sprite m_PlacehoderTileSprite;

    private Color m_HightlightColor = new Color(0, 0.8f, 1, 0.4f);
    private Color m_BlockColor = new Color(1, 0.2f, 0, 0.8f);

    public BuildActionSO BuildAction => m_BuildAction;

    public int woodCost => m_BuildAction.WoodCost;
    public int goldCost => m_BuildAction.GoldCost;



    public PlacementProcess(
        BuildActionSO buildAction, 
        TilemapManager tilemapManager)
    {
        m_PlacehoderTileSprite = Resources.Load<Sprite>("Images/PlacehoderTileSprite");
        m_BuildAction = buildAction;
        m_TilemapManager = tilemapManager;
    }

    public void Update()
    {
        if (m_PlacementOutLine != null)
        {
            HighlightTiles(m_PlacementOutLine.transform.position);
        }

        if (GreyUtils.IsPointerOverUiElement()) return;

        if (GreyUtils.TryGetHoldPosition(out Vector3 worldPosition))
        {
            m_PlacementOutLine.transform.position = SnapToGrid(worldPosition);
        }
    }

    public void Cleanup()
    {
        ClearHightlight();
        Object.Destroy(m_PlacementOutLine);
    }

    public bool TryFinalizePlacement(out Vector3 buildposition)
    {
        if (IsPlacementvalid())
        {
            ClearHightlight();
            buildposition = m_PlacementOutLine.transform.position;
            Object.Destroy(m_PlacementOutLine);
            return true;
        }

        Debug.Log("Placement invalid");
        buildposition = Vector3.zero;
        return false;
    }

    public bool IsPlacementvalid()
    {
        foreach (var tilePos in m_HighlightPosition)
        {
            if (!m_TilemapManager.CanPlaceTile(tilePos))
            {
                return false;
            }
        }

        return true;
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

            if (m_TilemapManager.CanPlaceTile(tilePos))
            {
                tile.color = m_HightlightColor;    
            }
            else
            {
                tile.color = m_BlockColor;               
                
            }
            m_TilemapManager.SetTileOverlay(tilePos, tile);
        }
        
    }   

    void ClearHightlight()
    {
        if(m_HighlightPosition == null) return;

        foreach (var tilePos in m_HighlightPosition)
        {
            m_TilemapManager.SetTileOverlay(tilePos, null);
        }
    }

    
}      