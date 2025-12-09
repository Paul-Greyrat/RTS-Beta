  


using UnityEngine;

public class Buildingprocess
{
    private BuildActionSO m_BuildAction;
    public Buildingprocess(
        BuildActionSO buildAction,
        Vector3 placementPosition
    )
    {
        m_BuildAction = buildAction;
        var structure = Object.Instantiate(m_BuildAction.StructurePrefab); 
        structure.Renderer.sprite = m_BuildAction.FoundationSprite;
        structure.transform.position = placementPosition;

    }
}
