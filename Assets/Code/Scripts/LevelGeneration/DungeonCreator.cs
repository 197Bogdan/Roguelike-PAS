﻿using System;
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

    public int tileSize = 4;
    public int floorTileSize = 2;
    public int wallLength = 4;

    
    [SerializeField] private Material floorMaterial;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] wallPrefabs;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject cornerPrefab;
    [SerializeField] private GameObject[] floorPrefabs;
    [SerializeField] private GameObject[] tileDecoratorPrefabs;
    [SerializeField] private GameObject[] wallDecoratorPrefabs;
    [SerializeField] private GameObject winningCheckpointPrefab;
    [SerializeField] private GameObject startingCheckpointPrefab;
    [SerializeField] private NavMeshSurface navMeshPrefab;

    [SerializeField] private double wallDecoratorChance = 0.1f;
    [SerializeField] private double tileDecoratorChance = 0.01f;
    [SerializeField] private int defaultPrefabChance = 90;

    public List<Node> dungeonRooms;
    public List<Vector3Int> possibleDoorsTopAndBottom;
    public List<Vector3Int> possibleDoorsLeftAndRight;
    private List<Vector3Int> possibleWallsLeftAndRight;
    private List<Vector3Int> possibleWallsTopAndBottom;

    public GameObject player;
    public List<GameObject> enemies;
    public GameObject winningCheckpoint;

    public SaveManager saveManager;
    public CharacterSaveData playerSaveData;
    public List<CharacterSaveData> enemySaveData;
    public Vector3 winningCheckpointPosition;
    
    private LayerMask wallLayerMask;
    private LayerMask defaultLayerMask;

    private System.Random random = new System.Random();

    void Start()
    {
        dungeonRooms = new List<Node>();
        possibleDoorsTopAndBottom = new List<Vector3Int>();
        possibleDoorsLeftAndRight = new List<Vector3Int>();
        possibleWallsLeftAndRight = new List<Vector3Int>();
        possibleWallsTopAndBottom = new List<Vector3Int>();
        wallLayerMask = LayerMask.NameToLayer("Wall");
        defaultLayerMask = LayerMask.NameToLayer("Default");

        if(saveManager.IsSavedGame())
            saveManager.LoadSave();     // setup dungeon variables from save
        else
        {
            createDungeonStructure();   // generate tree of rooms and corridors
            getWallPositions();         
            getDoorPositions();         
        }

        createFloorCollider();      // create big collider for the whole dungeon
        createAllFloors();              // create floor for each room/corridor
        createAllWalls();              // create walls and doors

        navMeshPrefab.BuildNavMesh();
        InstantiateEnemies(enemySaveData);
        InstantiatePlayer(playerSaveData);
        InstantiateWinningCheckpoint();

    }

    private void createFloorCollider()
    {
        int offset = 10;
        GameObject floorCollider = new GameObject("FloorCollider");
        floorCollider.transform.parent = transform;
        floorCollider.layer = LayerMask.NameToLayer("InvisibleFloor");
        floorCollider.AddComponent<BoxCollider>();
        floorCollider.GetComponent<BoxCollider>().size = new Vector3(dungeonWidth + offset, 0f, dungeonLength + offset);
        floorCollider.GetComponent<BoxCollider>().center = new Vector3(dungeonWidth / 2, 1f, dungeonLength / 2);
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

    public void createAllFloors()
    {
        Vector3 floorCenterOffset = floorTileSize / 2 * (Vector3.right + Vector3.forward);
        foreach (var room in dungeonRooms)
        {
            GameObject roomParent = new GameObject("RoomParent " + room.bottomLeft + " " + room.topRight);
            roomParent.transform.parent = transform;

            if(room is CorridorNode)
            {
                int floorWidth = (int)(room.topRight.x - room.bottomLeft.x);
                int floorLength = (int)(room.topRight.y - room.bottomLeft.y);

                GameObject floor = new GameObject("Mesh" + room.bottomLeft + " " + room.topRight, typeof(MeshFilter), typeof(MeshRenderer));

                floor.transform.localPosition = new Vector3(room.bottomLeft.x, 0, room.bottomLeft.y);
                floor.transform.localScale = Vector3.one;
                floor.transform.parent = roomParent.transform;
                floor.GetComponent<MeshFilter>().mesh = createFloorMesh(floorWidth, floorLength);
                floor.GetComponent<MeshRenderer>().material = floorMaterial;
                BoxCollider boxCollider = floor.AddComponent<BoxCollider>(); 
                boxCollider.size = floor.GetComponent<MeshRenderer>().bounds.size;
            }
            else
            {
                RoomNode roomNode = (RoomNode)room;
                GameObject floorParent = new GameObject("FloorParent " + roomNode.bottomLeft + " " + roomNode.topRight);
                floorParent.transform.parent = roomParent.transform;
                floorParent.transform.localPosition = new Vector3(roomNode.bottomLeft.x, 0, roomNode.bottomLeft.y);

                GameObject decorationParent = new GameObject("DecorationParent " + roomNode.bottomLeft + " " + roomNode.topRight);
                decorationParent.transform.parent = roomParent.transform;
                for(int row = roomNode.bottomLeft.x; row < roomNode.topRight.x; row = row + floorTileSize)
                {
                    for(int col = roomNode.bottomLeft.y; col < roomNode.topRight.y; col = col + floorTileSize)
                    {
                        Vector3 floorPosition = new Vector3(row, 0, col) + floorCenterOffset;
                        GameObject floor = Instantiate(getRandomPrefab(floorPrefabs), floorPosition, Quaternion.identity);
                        floor.transform.parent = floorParent.transform;
                        floor.layer = wallLayerMask;

                        if(random.NextDouble() < tileDecoratorChance)
                        {
                            Vector3 decoratorPosition = new Vector3(row, 0, col) + floorCenterOffset;
                            GameObject decoration = Instantiate(getRandomPrefab(tileDecoratorPrefabs), decoratorPosition, Quaternion.identity);
                            decoration.transform.parent = decorationParent.transform;
                        }
                    }
                }
            }

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

    private void createAllWalls()
    {
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;

        foreach (var room in dungeonRooms)
        {
            if (room is RoomNode)
            {
                RoomNode roomNode = (RoomNode)room;
                Vector3Int bottomLeft = new Vector3Int(roomNode.bottomLeft.x, 0, roomNode.bottomLeft.y);
                Vector3Int topRight = new Vector3Int(roomNode.topRight.x, 0, roomNode.topRight.y);
                Vector3Int bottomRight = new Vector3Int(roomNode.topRight.x, 0, roomNode.bottomLeft.y);
                Vector3Int topLeft = new Vector3Int(roomNode.bottomLeft.x, 0, roomNode.topRight.y);
                Quaternion bottomWallRotation = Quaternion.identity;
                Quaternion leftWallRotation = Quaternion.Euler(0, 90, 0);
                Quaternion topWallRotation = Quaternion.Euler(0, 180, 0);
                Quaternion rightWallRotation = Quaternion.Euler(0, 270, 0);

                createWallsAndDoor(bottomLeft + Vector3Int.right, bottomRight + Vector3Int.left, roomNode.bottomDoor, Vector3Int.right, bottomWallRotation, wallParent.transform);
                createWallsAndDoor(bottomRight + Vector3Int.forward, topRight + Vector3Int.back, roomNode.rightDoor, Vector3Int.forward, rightWallRotation, wallParent.transform);
                createWallsAndDoor(topRight + Vector3Int.left, topLeft + Vector3Int.right, roomNode.topDoor, Vector3Int.left, topWallRotation, wallParent.transform);
                createWallsAndDoor(topLeft + Vector3Int.back, bottomLeft + Vector3Int.forward, roomNode.leftDoor, Vector3Int.back, leftWallRotation, wallParent.transform);

                createCorners(bottomLeft, bottomRight, topLeft, topRight, wallParent.transform);
            }
            // else if (room is CorridorNode)
            // {
            //     TO DO
            // }
        }
    }

    public void createCorners(Vector3Int bottomLeft, Vector3Int bottomRight, Vector3Int topLeft, Vector3Int topRight, Transform parent = null)
    {
        createGameObject(cornerPrefab, bottomLeft, Quaternion.identity, parent, wallLayerMask);
        createGameObject(cornerPrefab, bottomRight, Quaternion.Euler(0, 270, 0), parent, wallLayerMask);
        createGameObject(cornerPrefab, topLeft, Quaternion.Euler(0, 90, 0), parent, wallLayerMask);
        createGameObject(cornerPrefab, topRight, Quaternion.Euler(0, 180, 0), parent, wallLayerMask);

    }

    private void createWallsAndDoor(Vector3Int startingPoint, Vector3Int endingPoint, Vector3Int doorPosition, Vector3Int direction, Quaternion rotation, Transform parent = null)
    {
        if(doorPosition != Vector3Int.zero)
        {
            int distance = (int) (doorPosition - startingPoint).magnitude;
            int margin = distance % wallLength;       

            Vector3Int doorCenterOffset = wallLength / 2 * direction;
            createDoor(doorPosition + doorCenterOffset, direction, rotation, parent);
            createWallLine(startingPoint, doorPosition - margin * direction, direction, rotation, parent);
            createWallLine(doorPosition + (wallLength + margin) * direction, endingPoint, direction, rotation, parent);
           
            if(margin != 0)     // if walls don't fit perfectly
                createDoorAdjacentWalls(doorPosition, direction, rotation, margin, parent);
        }
        else
        {
            createWallLine(startingPoint, endingPoint, direction, rotation, parent);
        }
    }

    private void createWallLine(Vector3Int startingPoint, Vector3Int endingPoint, Vector3Int direction, Quaternion rotation, Transform parent = null)
    {
        Vector3Int increment = wallLength * direction;
        Vector3Int wallCenterOffset = (wallLength / 2) * direction;
        float wallWidth = 1;
        int wallHeight = 4;
        Vector3 wallDecoratorOffset = new Vector3(0, wallHeight / 2, 0) + Quaternion.Euler(0, -90, 0) * (Vector3)direction  * wallWidth / 2;

        Vector3Int currentPoint = startingPoint;
        while (Vector3.Dot(currentPoint - endingPoint, direction) < 0)
        {
            createGameObject(getRandomPrefab(wallPrefabs), currentPoint + wallCenterOffset, rotation, parent, wallLayerMask);
            if(random.NextDouble() < wallDecoratorChance)
                createGameObject(getRandomPrefab(wallDecoratorPrefabs), currentPoint + wallDecoratorOffset, rotation, parent, defaultLayerMask);
            currentPoint += increment;
        }
    }

    private void createGameObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, LayerMask layer = default, float wallScale = 1)
    {
        GameObject newObj = Instantiate(prefab, position, rotation);
        newObj.layer = layer;
        newObj.transform.localScale = new Vector3(wallScale, 1, 1);
        newObj.transform.parent = parent;
    }

    private void createDoor(Vector3Int doorPosition, Vector3Int direction, Quaternion rotation, Transform parent = null)
    {
        GameObject door = Instantiate(doorPrefab, doorPosition, rotation);
        door.layer = wallLayerMask;
        door.transform.parent = parent;
    }

    private void createDoorAdjacentWalls(Vector3Int doorPosition, Vector3Int direction, Quaternion rotation, int margin, Transform parent = null)
    {
        Vector3Int wallCenterOffset = margin / 2 * direction;
        Vector3Int increment = wallLength * direction;
        float wallScale = (float)margin / wallLength;

        createGameObject(getRandomPrefab(wallPrefabs), doorPosition - wallCenterOffset, rotation, parent, wallLayerMask, wallScale);
        createGameObject(getRandomPrefab(wallPrefabs), doorPosition + increment + wallCenterOffset, rotation, parent, wallLayerMask, wallScale);
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
            Vector3Int maxVec = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            Vector3Int minVec = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

            room.bottomDoor = maxVec;
            room.rightDoor = maxVec;
            room.topDoor = minVec;
            room.leftDoor = minVec;
            Vector2 topRight = new Vector3(room.topRight.x - 1, room.topRight.y - 1);   // -1 for offset of door points in possibleDoorsTopAndBottom and possibleDoorsLeftAndRight

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
                        room.rightDoor = doorPos + Vector3Int.right;
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
                        room.topDoor = doorPos + Vector3Int.forward;
                }

            
            }

            if(room.bottomDoor == maxVec) room.bottomDoor = Vector3Int.zero; 
            if(room.rightDoor == maxVec) room.rightDoor = Vector3Int.zero; 
            if(room.topDoor == minVec) room.topDoor = Vector3Int.zero; else room.topDoor = room.topDoor + Vector3Int.right; // door points are offset by 1
            if(room.leftDoor == minVec) room.leftDoor = Vector3Int.zero; else room.leftDoor = room.leftDoor + Vector3Int.forward; // door points are offset by 1
        }

    }

    private void InstantiatePlayer(CharacterSaveData playerData = null)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerStartingPos = new Vector3((dungeonRooms[0].topRight.x + dungeonRooms[0].bottomLeft.x) / 2, 0, (dungeonRooms[0].topRight.y + dungeonRooms[0].bottomLeft.y) / 2);
        Quaternion playerStartingRot = Quaternion.identity;
        if(saveManager.IsSavedGame() && playerData != null)
        {
            playerStartingPos = playerData.position;
            playerStartingRot = playerData.rotation;
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            playerStats.SetHealth(playerData.health);
            playerStats.SetMana(playerData.mana);
            playerStats.SetExp(playerData.exp);
            playerStats.SetLevel(playerData.level);
        }

        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.enabled = false;
        player.transform.position = playerStartingPos;
        player.transform.rotation = playerStartingRot;
        characterController.enabled = true;
    }

    private void InstantiateEnemies(List<CharacterSaveData> enemySaveData = null)
    {
        GameObject enemyParent = new GameObject("EnemyParent");
        if(!saveManager.IsSavedGame())
        {
            foreach (var node in dungeonRooms.GetRange(1, dungeonRooms.Count - 1))
            {
                int enemyCount;
                if(node is RoomNode room)
                {
                    enemyCount = (int)Mathf.Round(room.Width * room.Length / 700f);
                    enemyCount += (int)Mathf.Round(UnityEngine.Random.Range(-1f, 1.0f));
                    for(int i = 0; i < enemyCount; i++)
                    {
                        int enemyIndex = random.Next(0, enemyPrefabs.Length);
                        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(room.bottomLeft.x, room.topRight.x), 0, UnityEngine.Random.Range(room.bottomLeft.y, room.topRight.y));
                        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], randomPosition, Quaternion.identity);
                        enemies.Add(enemy);
                        enemy.transform.parent = enemyParent.transform;
                    }
                }
                
            }
        }
        else
        {
            foreach (var enemyData in enemySaveData)
            {
                GameObject enemy = Instantiate(enemyPrefabs[enemyData.type], enemyData.position, enemyData.rotation);
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                enemyStats.SetHealth(enemyData.health);
                enemyStats.SetMana(enemyData.mana);
                enemyStats.SetExp(enemyData.exp);
                enemyStats.SetLevel(enemyData.level);

                enemies.Add(enemy);
                enemy.transform.parent = enemyParent.transform;
            }
        
        }

    }

    public void InstantiateWinningCheckpoint()
    {
        if(!saveManager.IsSavedGame())
        {
            Vector3 winningPosition = player.transform.position;
            Vector3 roomCenter;
            foreach(var node in dungeonRooms)
            {
                if(node is RoomNode room)
                {
                    roomCenter = new Vector3((room.topRight.x + room.bottomLeft.x) / 2, 2, (room.topRight.y + room.bottomLeft.y) / 2);
                    if(Vector3.Distance(player.transform.position, roomCenter) > Vector3.Distance(player.transform.position, winningPosition))
                        winningPosition = roomCenter;
                }
            }

        winningCheckpoint = Instantiate(winningCheckpointPrefab, winningPosition, Quaternion.identity);
        GameObject startingCheckpoint = Instantiate(startingCheckpointPrefab, player.transform.position + player.transform.forward * -2 + new Vector3(0,2,0), Quaternion.identity);
        }
        else
        {
            winningCheckpoint = Instantiate(winningCheckpointPrefab, winningCheckpointPosition, Quaternion.identity);
        }

    }

    private GameObject getRandomPrefab(GameObject[] prefabs)
    {
        if(random.Next(0, 100) < defaultPrefabChance)
            return prefabs[0];
        else
            return prefabs[random.Next(1, prefabs.Length)];
    }
}


