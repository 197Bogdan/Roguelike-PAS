using UnityEngine;

public class RoomNode : Node
{
    public Vector3Int leftDoor;
    public Vector3Int rightDoor;
    public Vector3Int topDoor;
    public Vector3Int bottomDoor;

    public RoomNode(Vector2Int bottomLeft, Vector2Int topRight, Node parentNode, int index) : base(parentNode)
    {
        this.bottomLeft = bottomLeft;
        this.topRight = topRight;
        this.bottomRight = new Vector2Int(topRight.x, bottomLeft.y);
        this.topLeft = new Vector2Int(bottomLeft.x, base.topRight.y);
        this.treeIndex = index;
    }

    public int Width { get => (int)(topRight.x - bottomLeft.x); }
    public int Length { get => (int)(topRight.y - bottomLeft.y); }
}