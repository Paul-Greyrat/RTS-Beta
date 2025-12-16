


using UnityEngine;

public class Node : MonoBehaviour
{
    public int x;
    public int y;
    public float CenterX;
    public float CenterY;
    public bool isWalkable;
    public float gCost;
    public float hCost;
    public float fCost;
    public Node parent;

    public Node(Vector3Int position, Vector3 cellSize, bool isWalkable)
    {
        x = position.x;
        y = position.y;
        Vector3 halfCellSize = cellSize / 2;
        var nodeCenterPosition = position + halfCellSize;
        CenterX = nodeCenterPosition.x;
        CenterY = nodeCenterPosition.y;
        this.isWalkable = isWalkable;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}
