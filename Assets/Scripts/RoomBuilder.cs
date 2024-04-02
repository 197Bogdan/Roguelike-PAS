using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder
{
    public int maxIt, minRoomWidth, minRoomLength;

    public RoomBuilder(int maxIt, int minRoomWidth, int minRoomLength)
    {
        this.maxIt = maxIt;
        this.minRoomWidth = minRoomWidth;
        this.minRoomLength = minRoomLength;
        
    }

    public List<RoomNode> buildRooms(List<Node> leaves, float leftModifier, float rightModifier, int offset)
    {
        List<RoomNode> rooms = new List<RoomNode>();
        foreach(Node leaf in leaves)
        {
            Vector2Int newBottomLeftPoint = Helper.generateBottomLeft(leaf.bottomLeft, leaf.topRight, leftModifier, offset);
            Vector2Int newTopRightPoint = Helper.generateTopRight(leaf.bottomLeft, leaf.topRight, rightModifier, offset);
            leaf.bottomLeft = newBottomLeftPoint;
            leaf.topRight = newTopRightPoint;
            leaf.bottomRight = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            leaf.topLeft = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            rooms.Add((RoomNode)leaf);
        }
        return rooms;
    }
}