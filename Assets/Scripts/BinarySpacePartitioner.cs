using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioner : MonoBehaviour
{ 
    public RoomNode rootRoom;

    public BinarySpacePartitioner(int dgWidth, int dgLength)
    {
       this.rootRoom = new RoomNode(null, 0, new Vector2Int(0, 0), new Vector2Int(dgWidth, dgLength));
    }

    public List<RoomNode> calculateRooms(int maxIt, int minRoomWidth, int minRoomLength)
    {
        Queue<RoomNode> roomQueue = new Queue<RoomNode>();
        List<RoomNode> rooms = new List<RoomNode>();
        rooms.Add(rootRoom);
        roomQueue.Enqueue(rootRoom);
        int it = 0;

        while (it < maxIt && roomQueue.Count > 0)
        {
            it++;
            RoomNode currentRoom = roomQueue.Dequeue();
            if(currentRoom.width > minRoomWidth * 2 && currentRoom.length > minRoomLength * 2)
                splitRoom(currentRoom, roomQueue, rooms, minRoomLength, minRoomWidth);
        }
        return rooms;
    }

    private void splitRoom(RoomNode currentRoom, Queue<RoomNode> roomQueue, List<RoomNode> rooms, int minRoomLength, int minRoomWidth)
    {
        Line divideLine = createDivideLine(currentRoom, minRoomLength, minRoomWidth);
        RoomNode room1, room2;
        if(divideLine.orientation == Orientation.Horizontal)
        {
            room1 = new RoomNode(currentRoom, currentRoom.treeIndex + 1, currentRoom.bottomLeft, new Vector2Int(currentRoom.topRight.x, divideLine.point.y));
            room2 = new RoomNode(currentRoom, currentRoom.treeIndex + 1, new Vector2Int(currentRoom.bottomLeft.x, divideLine.point.y), currentRoom.topRight);
        }
        else
        {
            room1 = new RoomNode(currentRoom, currentRoom.treeIndex + 1, currentRoom.bottomLeft, new Vector2Int(divideLine.point.x, currentRoom.topRight.y));
            room2 = new RoomNode(currentRoom, currentRoom.treeIndex + 1, new Vector2Int(divideLine.point.x, currentRoom.bottomLeft.y), currentRoom.topRight);
        }
        rooms.Add(room1);
        rooms.Add(room2);
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
        currentRoom.children.Add(room1);
        currentRoom.children.Add(room2);
    }

    private Line createDivideLine(RoomNode currentRoom, int minRoomLength, int minRoomWidth)
    {
        Orientation orientation;
        Vector2Int point;
        bool lengthDivide = currentRoom.length > 2 * minRoomLength;
        bool widthDivide = currentRoom.width > 2 * minRoomWidth;

        if(lengthDivide && widthDivide)
        {
            orientation = (Orientation)UnityEngine.Random.Range(0, 2);
        }
        else if(lengthDivide)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }
        
        if(orientation == Orientation.Vertical)
        {
            int x = UnityEngine.Random.Range(currentRoom.bottomLeft.x + minRoomWidth, currentRoom.topRight.x - minRoomWidth);
            point = new Vector2Int(x, currentRoom.bottomLeft.y);
        }
        else
        {
            int y = UnityEngine.Random.Range(currentRoom.bottomLeft.y + minRoomLength, currentRoom.topRight.y - minRoomLength);
            point = new Vector2Int(currentRoom.bottomLeft.x, y);
        }

        return new Line(orientation, point);
    }
}
