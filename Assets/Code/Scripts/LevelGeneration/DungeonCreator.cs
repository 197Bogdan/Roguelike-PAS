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

    private List<Node> dungeonRooms;
    private List<Vector3Int> possibleDoorsTopAndBottom;
    private List<Vector3Int> possibleDoorsLeftAndRight;
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
        // foreach (var room in dungeonRooms)
        // {
        //     Debug.Log("Room: " + room.bottomLeft + " " + room.topRight + "" + (room.topRight.x - room.bottomLeft.x) + " " + (room.topRight.y - room.bottomLeft.y));
        //     if 
        // }
    }

    public void createFloor()
    {
        foreach (var room in dungeonRooms)
        {
            int floorWidth = (int)(room.topRight.x - room.bottomLeft.x);
            int floorLength = (int)(room.topRight.y - room.bottomLeft.y);

            GameObject floor = new GameObject("Mesh" + room.bottomLeft, typeof(MeshFilter), typeof(MeshRenderer));

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
        foreach (var room in dungeonRooms.GetRange(1, dungeonRooms.Count - 1))
        {
            Vector3 enemyPosition = new Vector3((room.topRight.x + room.bottomLeft.x) / 2, 0, (room.topRight.y + room.bottomLeft.y) / 2);
            Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        }
    }

}


