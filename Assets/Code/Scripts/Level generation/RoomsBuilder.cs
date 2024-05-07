using System.Collections.Generic;
using UnityEngine;

public class RoomsBuilder
{
    public RoomsBuilder() {}

    public List<RoomNode> buildRooms(List<Node> roomSpaces, float roomBottomModifier, float roomTopModifier, int roomOffset)
    {
        List<RoomNode> rooms = new List<RoomNode>();
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeft = Helper.generateBottomLeftBetween(
                space.bottomLeft, space.topRight, roomBottomModifier, roomOffset);

            Vector2Int newTopRight = Helper.generateTopRightBetween(
                space.bottomLeft, space.topRight, roomTopModifier, roomOffset);
            space.bottomLeft = newBottomLeft;
            space.topRight = newTopRight;
            space.bottomRight = new Vector2Int(newTopRight.x, newBottomLeft.y);
            space.topLeft = new Vector2Int(newBottomLeft.x, newTopRight.y);
            rooms.Add((RoomNode)space);
                
        }
        return rooms;
    }
}