using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    List<RoomNode> roomNodes = new List<RoomNode>();
    public int dgWidth, dgLength, minRoomWidth, minRoomLength;
    public int maxIt, corridorWidth;
    public Material material;
    public float cornerModifier = 0.1f;
    public int roomOffset = 1;

    public DungeonBuilder(int dgWidth, int dgLength)
    {
        this.dgWidth = dgWidth;
        this.dgLength = dgLength;
    }

    public void Start()
    {
        buildDungeon();
    }

    public void buildDungeon()
    {
        DungeonBuilder dungeonBuilder = new DungeonBuilder(dgWidth, dgLength);
        List<RoomNode> rooms = dungeonBuilder.calculateRooms(maxIt, minRoomWidth, minRoomLength);
        foreach (RoomNode room in rooms)
        {
            createMesh(room.bottomLeft, room.topRight);
        }
    }

    public List<RoomNode> calculateRooms(int maxIterations, int minRoomWidth, int minRoomLength)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dgWidth, dgLength);
        roomNodes = bsp.calculateRooms(maxIterations, minRoomWidth, minRoomLength);
        List<Node> leaves = Helper.extractLeaves(bsp.rootRoom);
        RoomBuilder roomBuilder = new RoomBuilder(maxIterations, minRoomWidth, minRoomLength);
        List<RoomNode> rooms = roomBuilder.buildRooms(leaves, cornerModifier, roomOffset);
        return rooms;
    }

    private void createMesh(Vector2 bottomLeft, Vector2 topRight)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeft.x, 0, bottomLeft.y);
        Vector3 bottomRightV = new Vector3(topRight.x, 0, bottomLeft.y);
        Vector3 topLeftV = new Vector3(bottomLeft.x, 0, topRight.y);
        Vector3 topRightV = new Vector3(topRight.x, 0, topRight.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(vertices[0].x, vertices[0].z),
            new Vector2(vertices[1].x, vertices[1].z),
            new Vector2(vertices[2].x, vertices[2].z),
            new Vector2(vertices[3].x, vertices[3].z)
        };

        int[] triangles = new int[]{0, 1, 2, 2, 1, 3};

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeft, typeof(MeshFilter), typeof(MeshRenderer));
        dungeonFloor.transform.position = new Vector3(0, 0, 0);
        dungeonFloor.transform.localScale = new Vector3(1, 1, 1);
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
    }

}
