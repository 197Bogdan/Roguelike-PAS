using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    List<Room> rooms = new List<Room>();
    public int dgWidth, dgLength, minRoomWidth, minRoomLength;
    public int maxIt, corridorWidth;

    public DungeonBuilder(int dgWidth, int dgLength)
    {
        this.dgWidth = dgWidth;
        this.dgLength = dgLength;
    }

    public void Start()
    {
        calculateRooms(maxIt, minRoomWidth, minRoomLength);
    }

    public List<Room> calculateRooms(int maxIterations, int minRoomWidth, int minRoomLength)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dgWidth, dgLength);
        rooms = bsp.calculateRooms(maxIterations, minRoomWidth, minRoomLength);
        return rooms;
    }

}
