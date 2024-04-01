using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioner : MonoBehaviour
{ 
    Room rootRoom;

    public BinarySpacePartitioner(int dgWidth, int dgLength)
    {
       this.rootRoom = new Room(null, 0, new Vector2Int(0, 0), new Vector2Int(dgWidth, dgLength));
    }

    public List<Room> calculateRooms(int maxIt, int minRoomWidth, int minRoomLength)
    {
        Queue<Room> roomQueue = new Queue<Room>();
        List<Room> rooms = new List<Room>();
        rooms.Add(rootRoom);
        roomQueue.Enqueue(rootRoom);
        int it = 0;

        while (it < maxIt && roomQueue.Count > 0)
        {
            it++;
            Room currentRoom = roomQueue.Dequeue();
            if(currentRoom.width > minRoomWidth * 2 && currentRoom.length > minRoomLength * 2)
                splitRoom(currentRoom, roomQueue, rooms, minRoomLength, minRoomWidth);
        }
        return rooms;
    }

    private void splitRoom(Room currentRoom, Queue<Room> roomQueue, List<Room> rooms, int minRoomLength, int minRoomWidth)
    {
        Line divideLine = createDivideLine(currentRoom, minRoomLength, minRoomWidth);
        Room room1, room2;
        if(divideLine.orientation == Orientation.Horizontal)
        {
            room1 = new Room(currentRoom, currentRoom.treeIndex + 1, currentRoom.bottomLeft, new Vector2Int(currentRoom.upperRight.x, divideLine.point.y));
            room2 = new Room(currentRoom, currentRoom.treeIndex + 1, new Vector2Int(currentRoom.bottomLeft.x, divideLine.point.y), currentRoom.upperRight);
        }
        else
        {
            room1 = new Room(currentRoom, currentRoom.treeIndex + 1, currentRoom.bottomLeft, new Vector2Int(divideLine.point.x, currentRoom.upperRight.y));
            room2 = new Room(currentRoom, currentRoom.treeIndex + 1, new Vector2Int(divideLine.point.x, currentRoom.bottomLeft.y), currentRoom.upperRight);
        }
        rooms.Add(room1);
        rooms.Add(room2);
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }

    private Line createDivideLine(Room currentRoom, int minRoomLength, int minRoomWidth)
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
            int x = UnityEngine.Random.Range(currentRoom.bottomLeft.x + minRoomWidth, currentRoom.upperRight.x - minRoomWidth);
            point = new Vector2Int(x, currentRoom.bottomLeft.y);
        }
        else
        {
            int y = UnityEngine.Random.Range(currentRoom.bottomLeft.y + minRoomLength, currentRoom.upperRight.y - minRoomLength);
            point = new Vector2Int(currentRoom.bottomLeft.x, y);
        }

        return new Line(orientation, point);
    }
}
