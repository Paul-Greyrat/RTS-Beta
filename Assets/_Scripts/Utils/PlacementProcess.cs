


using UnityEngine;

public class PlacementProcess
{
    private GameObject m_PlacementOutLine;
    public BuildActionSO m_BuildAction;
    public Vector3 m_HighlightPosition;

    public PlacementProcess(BuildActionSO buildAction)
    {
        m_BuildAction = buildAction;
    }

    public void Update()
    {
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

    void HighlightTiles()
    {
        
    }
}