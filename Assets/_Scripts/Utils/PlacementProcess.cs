


using Unity.VisualScripting;
using UnityEngine;

public class PlacementProcess
{
    private GameObject m_PlacementOutLine;
    public BuildActionSO m_BuildAction;
    public Vector3Int[] m_HighlightPosition;

    public PlacementProcess(BuildActionSO buildAction)
    {
        m_BuildAction = buildAction;
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
        Vector2Int buildingSize = new Vector2Int(2,3);
        m_HighlightPosition = new Vector3Int[buildingSize.x * buildingSize.y];

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                m_HighlightPosition[x + y * buildingSize.x] = new Vector3Int((int)(outLinePosition.x + x), (int)(outLinePosition.y + y), 0);
            }
        }

        foreach ( var tilePos in m_HighlightPosition)
        {
            Debug.Log(tilePos);
        }
        Debug.Log("----------------");
    }
}