using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : Node
{
    public Room(Node parent, int treeIndex, Vector2Int bottomLeft, Vector2Int upperRight): base(parent)
    {
        this.treeIndex = treeIndex;
        this.bottomLeft = bottomLeft;
        this.upperRight = upperRight;
        this.bottomRight = new Vector2Int(upperRight.x, bottomLeft.y);
        this.upperLeft = new Vector2Int(bottomLeft.x, upperRight.y);
    }
    public int width { get { return upperRight.x - bottomLeft.x; } }
    public int length { get { return upperRight.y - bottomLeft.y; } }
    

}
