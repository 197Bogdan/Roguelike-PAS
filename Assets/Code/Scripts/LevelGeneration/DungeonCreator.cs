using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    [Range(0.0f, 0.3f)]
    public float roomBottomModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopModifier;
    [Range(0, 2)]
    public int roomOffset;
    public int tileSize;

    
    [SerializeField] private Material floorMaterial;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private NavMeshSurface navMeshPrefab;

    public List<Node> dungeonRooms;
    public List<Vector3Int> possibleDoorsTopAndBottom;
    public List<Vector3Int> possibleDoorsLeftAndRight;
    private List<Vector3Int> possibleWallsLeftAndRight;
    private List<Vector3Int> possibleWallsTopAndBottom;
    private LayerMask wallLayerMask;

    void Start()
    {
        possibleDoorsTopAndBottom = new List<Vector3Int>();
        possibleDoorsLeftAndRight = new List<Vector3Int>();
        possibleWallsLeftAndRight = new List<Vector3Int>();
        possibleWallsTopAndBottom = new List<Vector3Int>();
        wallLayerMask = LayerMask.NameToLayer("Wall");


        createDungeonStructure();   // generate tree of rooms and corridors
        createFloorCollider();
        createFloor();              // create floor for each room/corridor
        getWallPositions();         // get wall positions
        getDoorPositions();         // set door positions
        createWalls();              // create walls and doors


        navMeshPrefab.BuildNavMesh();

        InstantiateEnemies();
        InstantiatePlayer();

    }

    private void createFloorCollider()
    {
        int offset = 10;
        transform.gameObject.AddComponent<BoxCollider>();
        transform.gameObject.GetComponent<BoxCollider>().size = new Vector3(dungeonWidth + offset, 0f, dungeonLength + offset);
        transform.gameObject.GetComponent<BoxCollider>().center = new Vector3(dungeonWidth / 2, 0f, dungeonLength / 2);

    }


    public void createDungeonStructure()
    {
        DungeonBuilder generator = new DungeonBuilder(dungeonWidth, dungeonLength);
        dungeonRooms = generator.buildRoomsAndCorridors(
            maxIterations,
            roomWidthMin, roomLengthMin,
            roomBottomModifier, roomTopModifier, roomOffset,
            corridorWidth, tileSize);
    }

    public void createFloor()
    {
        foreach (var room in dungeonRooms)
        {
            int floorWidth = (int)(room.topRight.x - room.bottomLeft.x);
            int floorLength = (int)(room.topRight.y - room.bottomLeft.y);

            GameObject floor = new GameObject("Mesh" + room.bottomLeft + " " + room.topRight, typeof(MeshFilter), typeof(MeshRenderer));

            floor.transform.localPosition = new Vector3(room.bottomLeft.x, 0, room.bottomLeft.y);
            floor.transform.localScale = Vector3.one;
            floor.transform.parent = transform;
            floor.GetComponent<MeshFilter>().mesh = createFloorMesh(floorWidth, floorLength);
            floor.GetComponent<MeshRenderer>().material = floorMaterial;
            BoxCollider boxCollider = floor.AddComponent<BoxCollider>(); 
            boxCollider.size = floor.GetComponent<MeshRenderer>().bounds.size;
        }
    }

    private void getWallPositions()
    {
        foreach (var room in dungeonRooms)
        {
            Vector2 bottomLeft = room.bottomLeft;
            int floorWidth = (int)(room.topRight.x - room.bottomLeft.x);
            int floorLength = (int)(room.topRight.y - room.bottomLeft.y);

            if (room is RoomNode)
            {
                for (int row = (int)bottomLeft.x; row <= (int)bottomLeft.x + floorWidth - 1; row++)
                {
                    var bottomWallPosition = new Vector3(row, 0, bottomLeft.y);
                    addWallPositionToList(bottomWallPosition, possibleWallsLeftAndRight, possibleDoorsLeftAndRight);
                    var topWallPosition = new Vector3(row, 0, bottomLeft.y + floorLength - 1);
                    addWallPositionToList(topWallPosition, possibleWallsLeftAndRight, possibleDoorsLeftAndRight);
                }

                for (int col = (int)bottomLeft.y; col <= (int)bottomLeft.y + floorLength - 1; col++)
                {
                    var leftWallPosition = new Vector3(bottomLeft.x, 0, col);
                    addWallPositionToList(leftWallPosition, possibleWallsTopAndBottom, possibleDoorsTopAndBottom);
                    var rightWallPosition = new Vector3(bottomLeft.x + floorWidth - 1, 0, col);
                    addWallPositionToList(rightWallPosition, possibleWallsTopAndBottom, possibleDoorsTopAndBottom);
                }
            }
            else if (room is CorridorNode)
            {
                for (int row = (int)bottomLeft.x; row <= (int)bottomLeft.x + floorWidth - 1; row++)
                {
                    var bottomWallPosition = new Vector3(row, 0, bottomLeft.y - 1);
                    addWallPositionToList(bottomWallPosition, possibleWallsLeftAndRight, possibleDoorsLeftAndRight);
                    var topWallPosition = new Vector3(row, 0, bottomLeft.y + floorLength);
                    addWallPositionToList(topWallPosition, possibleWallsLeftAndRight, possibleDoorsLeftAndRight);
                }

                for (int col = (int)bottomLeft.y; col <= (int)bottomLeft.y + floorLength - 1; col++)
                {
                    var leftWallPosition = new Vector3(bottomLeft.x - 1, 0, col);
                    addWallPositionToList(leftWallPosition, possibleWallsTopAndBottom, possibleDoorsTopAndBottom);
                    var rightWallPosition = new Vector3(bottomLeft.x + floorWidth, 0, col);
                    addWallPositionToList(rightWallPosition, possibleWallsTopAndBottom, possibleDoorsTopAndBottom);
                }
            }

        }

    }

    private void createWalls()
    {
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;

        foreach (var wallPosition in possibleWallsLeftAndRight)
        {
            GameObject newWall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
            newWall.layer = wallLayerMask;
        }
        foreach (var wallPosition in possibleWallsTopAndBottom)
        {
            GameObject newWall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
            newWall.layer = wallLayerMask;
        }
    }

    // private void createWalls()
    // {
    //     GameObject wallParent = new GameObject("WallParent");
    //     wallParent.transform.parent = transform;

    //     foreach (var room in dungeonRooms)
    //     {
    //         if (room is RoomNode)
    //         {
    //             RoomNode roomNode = (RoomNode)room;
    //             Vector3Int bottomLeft = new Vector3Int(roomNode.bottomLeft.x, 0, roomNode.bottomLeft.y);
    //             Vector3Int topRight = new Vector3Int(roomNode.topRight.x, 0, roomNode.topRight.y);
    //             Vector3Int bottomRight = new Vector3Int(roomNode.topRight.x, 0, roomNode.bottomLeft.y);
    //             Vector3Int topLeft = new Vector3Int(roomNode.bottomLeft.x, 0, roomNode.topRight.y);

    //             createWall(bootomLeft, bottomRight, roomNode.bottomDoor, new Vector3(0, 0, tileSize), tileSize, Orientation.Horizontal, Quaternion.identity);
    //             createWall(topLeft, topRight, roomNode.topDoor, new Vector3(0, 0, tileSize), tileSize, Orientation.Horizontal, Quaternion.identity);
    //             createWall(bottomLeft, topLeft, roomNode.leftDoor, new Vector3(tileSize, 0, 0), tileSize, Orientation.Vertical, Quaternion.identity);
    //             createWall(bottomRight, topRight, roomNode.rightDoor, new Vector3(tileSize, 0, 0), tileSize, Orientation.Vertical, Quaternion.identity);
    //         }
    //         // else if (room is CorridorNode)
    //         // {
    //         //     CorridorNode corridorNode = (CorridorNode)room;
    //         //     createWall(corridorNode.bottomLeft, corridorNode.topRight, corridorNode.topDoor, Vector3.right, corridorNode.Width, Orientation.Horizontal, Quaternion.identity);
    //         //     createWall(corridorNode.bottomLeft, corridorNode.topRight, corridorNode.bottomDoor, Vector3.right, corridorNode.Width, Orientation.Horizontal, Quaternion.identity);
    //         //     createWall(corridorNode.bottomLeft, corridorNode.topRight, corridorNode.leftDoor, Vector3.forward, corridorNode.Length, Orientation.Vertical, Quaternion.identity);
    //         //     createWall(corridorNode.bottomLeft, corridorNode.topRight, corridorNode.rightDoor, Vector3.forward, corridorNode.Length, Orientation.Vertical, Quaternion.identity);
    //         // }
    //     }
    // }

    private void createWall(Vector3Int startingPoint, Vector3Int endingPoint, Vector3Int doorPosition, Vector3Int increment, int wallSize, Orientation orientation, Quaternion rotation)
    {
        Vector3Int currentPoint = startingPoint;
        if(orientation == Orientation.Horizontal)
        {
            GameObject door = Instantiate(wallPrefab, doorPosition, rotation);
            door.layer = wallLayerMask;

            while ((currentPoint + increment).x < doorPosition.x)
            {
                GameObject newWall = Instantiate(wallPrefab, currentPoint, rotation);
                newWall.layer = wallLayerMask;
                currentPoint += increment;
            }

            int margin = doorPosition.x - currentPoint.x;       
            if (currentPoint != doorPosition)               // walls next to door don't fit perfectly
            {
                GameObject wall1 = Instantiate(wallPrefab, currentPoint, rotation);
                wall1.layer = wallLayerMask;
                wall1.transform.localScale = new Vector3(margin / wallSize, 1, 1);
                

                GameObject wall2 = Instantiate(wallPrefab, currentPoint + increment + new Vector3Int(margin, 0, 0), rotation);
                wall2.layer = wallLayerMask;
                wall2.transform.localScale = new Vector3((endingPoint.x - currentPoint.x) / wallSize, 1, 1);
            }
            currentPoint = doorPosition + new Vector3Int(margin, 0, 0);

            while (currentPoint != endingPoint)
            {
                GameObject newWall = Instantiate(wallPrefab, currentPoint, rotation);
                newWall.layer = wallLayerMask;
                currentPoint += increment;
            }
        }
        else if(orientation == Orientation.Vertical)
        {
            GameObject door = Instantiate(wallPrefab, doorPosition, rotation);
            door.layer = wallLayerMask;

            while ((currentPoint + increment).z < doorPosition.z)
            {
                GameObject newWall = Instantiate(wallPrefab, currentPoint, rotation);
                newWall.layer = wallLayerMask;
                currentPoint += increment;
            }

            int margin = doorPosition.z - currentPoint.z;
            if (currentPoint != doorPosition)               // walls next to door don't fit perfectly
            {
                GameObject wall1 = Instantiate(wallPrefab, currentPoint, rotation);
                wall1.layer = wallLayerMask;
                wall1.transform.localScale = new Vector3(1, 1, margin / wallSize);

                GameObject wall2 = Instantiate(wallPrefab, currentPoint + increment + new Vector3Int(0, 0, margin), rotation);
                wall2.layer = wallLayerMask;
                wall2.transform.localScale = new Vector3(1, 1, (endingPoint.z - currentPoint.z) / wallSize);
            }
            currentPoint = doorPosition + new Vector3Int(0, 0, margin);

            while (currentPoint != endingPoint)
            {
                GameObject newWall = Instantiate(wallPrefab, currentPoint, rotation);
                newWall.layer = wallLayerMask;
                currentPoint += increment;
            }
        }
    }



    private Mesh createFloorMesh(int floorWidth, int floorLength)
    {
        Vector3 bottomLeftV = Vector3.zero;
        Vector3 bottomRightV = new Vector3(floorWidth, 0, 0);
        Vector3 topLeftV = new Vector3(0, 0, floorLength);
        Vector3 topRightV = new Vector3(floorWidth, 0, floorLength);

        Vector3[] vertices = new Vector3[]{topLeftV, topRightV, bottomLeftV, bottomRightV};

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);

        int[] triangles = new int[]{0, 1, 2, 2, 1, 3};
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void addWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
            wallList.Add(point);
    }

    private void getDoorPositions()
    {
        foreach(var node in dungeonRooms)
        {
            if (node is CorridorNode)
                continue;

            RoomNode room = (RoomNode)node;
            room.bottomDoor = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            room.topDoor = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
            room.leftDoor = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
            room.rightDoor = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

            Vector2 topRight = new Vector3(room.topRight.x - 1, room.topRight.y - 1);
            foreach (var doorPos in possibleDoorsTopAndBottom)
            {
                if (room.bottomLeft.x == doorPos.x && room.bottomLeft.y < doorPos.z && topRight.y > doorPos.z )    // is part of door on left wall
                {
                    if(room.leftDoor.y < doorPos.z)
                        room.leftDoor = doorPos;
                }
                else if (topRight.x == doorPos.x && room.bottomLeft.y < doorPos.z && topRight.y > doorPos.z)    // is part of door on right wall
                {
                    if (room.rightDoor.y > doorPos.z)
                        room.rightDoor = doorPos;
                }
            }

            foreach (var doorPos in possibleDoorsLeftAndRight)
            {
                if (room.bottomLeft.y == doorPos.z && room.bottomLeft.x < doorPos.x && topRight.x > doorPos.x)    // is part of door on bottom wall
                {
                    if (room.bottomDoor.x > doorPos.x)
                        room.bottomDoor = doorPos;
                }
                else if (topRight.y == doorPos.z && room.bottomLeft.x < doorPos.x && topRight.x > doorPos.x)    // is part of door on top wall
                {
                    if (room.topDoor.x < doorPos.x)
                        room.topDoor = doorPos;
                }

            
            }

            Vector3Int max = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            Vector3Int min = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
            if(room.bottomDoor == max) room.bottomDoor = Vector3Int.zero; else room.bottomDoor.x -= 1;
            if(room.rightDoor == max) room.rightDoor = Vector3Int.zero; else room.rightDoor.z -= 1;
            if(room.topDoor == min) room.topDoor = Vector3Int.zero; else room.topDoor.x += 1;
            if(room.leftDoor == min) room.leftDoor = Vector3Int.zero; else room.leftDoor.z += 1;

            Debug.Log("Room: " + room.bottomLeft + " " + topRight + " Doors: " + room.bottomDoor + " " + room.topDoor + " " + room.leftDoor + " " + room.rightDoor);
        }

    }

    private void InstantiatePlayer()
    {
        Vector3 playerStartingPos = new Vector3((dungeonRooms[0].topRight.x + dungeonRooms[0].bottomLeft.x) / 2, 0, (dungeonRooms[0].topRight.y + dungeonRooms[0].bottomLeft.y) / 2);
        Instantiate(playerPrefab, playerStartingPos, Quaternion.identity);

        Quaternion cameraRotation = Quaternion.Euler(30, 45, 0);
        Vector3 cameraPosition = playerStartingPos + new Vector3(-15, 20, -15);
        Instantiate(cameraPrefab, cameraPosition, cameraRotation);
    }

    private void InstantiateEnemies()
    {
        GameObject enemyParent = new GameObject("EnemyParent");
        foreach (var room in dungeonRooms.GetRange(1, dungeonRooms.Count - 1))
        {
            Vector3 enemyPosition = new Vector3((room.topRight.x + room.bottomLeft.x) / 2, 0, (room.topRight.y + room.bottomLeft.y) / 2);
            GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            enemy.transform.parent = enemyParent.transform;
            
        }
    }

}


