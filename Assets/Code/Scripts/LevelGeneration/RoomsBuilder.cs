using System.Collections.Generic;
using UnityEngine;

public class RoomsBuilder
{
    public RoomsBuilder() {}

    public List<RoomNode> buildRooms(List<Node> roomSpaces, float roomBottomModifier, float roomTopModifier, int roomOffset, int tileSize)
    {
        List<RoomNode> rooms = new List<RoomNode>();
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeft = Helper.generateBottomLeftBetween(
                space.bottomLeft, space.topRight, roomBottomModifier, roomOffset);

            Vector2Int newTopRight = Helper.generateTopRightBetween(
                space.bottomLeft, space.topRight, roomTopModifier, roomOffset);

            Vector2Int[] adjustedRoom = AdjustRoomToTileSize(newBottomLeft, newTopRight, tileSize);
            newBottomLeft = adjustedRoom[0];
            newTopRight = adjustedRoom[1];

            space.bottomLeft = newBottomLeft;
            space.topRight = newTopRight;
            space.bottomRight = new Vector2Int(newTopRight.x, newBottomLeft.y);
            space.topLeft = new Vector2Int(newBottomLeft.x, newTopRight.y);
            rooms.Add((RoomNode)space);
                
        }
        return rooms;
    }

    private Vector2Int[] AdjustRoomToTileSize(Vector2Int bottomLeft, Vector2Int topRight, int tileSize)
    {
        Vector2Int newBottomLeft = new Vector2Int(
            bottomLeft.x - (bottomLeft.x % tileSize),
            bottomLeft.y - (bottomLeft.y % tileSize)
        );

        Vector2Int newTopRight = new Vector2Int(
            topRight.x - (topRight.x % tileSize) + 1,
            topRight.y - (topRight.y % tileSize) + 1
        );

        return new Vector2Int[] { newBottomLeft, newTopRight };
    }
}