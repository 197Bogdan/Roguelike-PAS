using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public Material material;
    [Range(0.0f, 0.3f)]
    public float roomBottomModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopModifier;
    [Range(0, 2)]
    public int roomOffset;
    public GameObject wallVertical, wallHorizontal;
    List<Vector3Int> possibleDoorsTopAndBottom;
    List<Vector3Int> possibleDoorsLeftAndRight;
    List<Vector3Int> possibleWallsLeftAndRight;
    List<Vector3Int> possibleWallsTopAndBottom;

    void Start()
    {
        createDungeon();
    }

    public void createDungeon()
    {
        DugeonBuilder generator = new DugeonBuilder(dungeonWidth, dungeonLength);
        var rooms = generator.buildRoomsAndCorridors(
            maxIterations,
            roomWidthMin, roomLengthMin,
            roomBottomModifier, roomTopModifier, roomOffset,
            corridorWidth);
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorsTopAndBottom = new List<Vector3Int>();
        possibleDoorsLeftAndRight = new List<Vector3Int>();
        possibleWallsLeftAndRight = new List<Vector3Int>();
        possibleWallsTopAndBottom = new List<Vector3Int>();
        for (int i = 0; i < rooms.Count; i++)
        {
            createMesh(rooms[i].bottomLeft, rooms[i].topRight);
        }
        createWalls(wallParent);
    }

    private void createWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallsLeftAndRight)
        {
            createWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallsTopAndBottom)
        {
            createWall(wallParent, wallPosition, wallVertical);
        }
    }

    private void createWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        GameObject newWall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
        BoxCollider boxCollider = newWall.AddComponent<BoxCollider>();
        boxCollider.size = newWall.GetComponent<MeshRenderer>().bounds.size;
    }

    private void createMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]{topLeftV, topRightV, bottomLeftV, bottomRightV};

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);

        int[] triangles = new int[]{0, 1, 2, 2, 1, 3};
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject floor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        floor.transform.position = Vector3.zero;
        floor.transform.localScale = Vector3.one;
        floor.transform.parent = transform;
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material = material;
        BoxCollider boxCollider = floor.AddComponent<BoxCollider>(); 
        boxCollider.size = floor.GetComponent<MeshRenderer>().bounds.size;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            addWallPositionToList(wallPosition, possibleWallsLeftAndRight, possibleDoorsLeftAndRight);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            addWallPositionToList(wallPosition, possibleWallsLeftAndRight, possibleDoorsLeftAndRight);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            addWallPositionToList(wallPosition, possibleWallsTopAndBottom, possibleDoorsTopAndBottom);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            addWallPositionToList(wallPosition, possibleWallsTopAndBottom, possibleDoorsTopAndBottom);
        }
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
}
