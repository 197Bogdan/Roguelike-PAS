using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : Node
{
    public RoomNode(Node parent, int treeIndex, Vector2Int bottomLeft, Vector2Int upperRight): base(parent)
    {
        this.treeIndex = treeIndex;
        this.bottomLeft = bottomLeft;
        this.topRight = upperRight;
        this.bottomRight = new Vector2Int(upperRight.x, bottomLeft.y);
        this.topLeft = new Vector2Int(bottomLeft.x, upperRight.y);
    }
    public int width { get { return topRight.x - bottomLeft.x; } }
    public int length { get { return topRight.y - bottomLeft.y; } }
    

}
