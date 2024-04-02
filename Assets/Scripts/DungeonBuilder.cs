using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    List<RoomNode> roomNodes = new List<RoomNode>();
    public int dgWidth, dgLength, minRoomWidth, minRoomLength;
    public int maxIt, corridorWidth;
    public Material material;
    [Range(0.0f, 0.3f)]
    public float leftModifier = 0.1f;
    [Range(0.0f, 0.3f)]
    public float rightModifier = 0.1f;
    [Range(0, 2)]
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
        List<Node> rooms = dungeonBuilder.buildRoomsAndCorridors(maxIt, minRoomWidth, minRoomLength);
        foreach (Node room in rooms)
        {
            createMesh(room.bottomLeft, room.topRight);
        }
    }

    public List<Node> buildRoomsAndCorridors(int maxIterations, int minRoomWidth, int minRoomLength)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dgWidth, dgLength);
        roomNodes = bsp.calculateRooms(maxIterations, minRoomWidth, minRoomLength);
        List<Node> leaves = Helper.extractLeaves(bsp.rootRoom);
        RoomBuilder roomBuilder = new RoomBuilder(maxIterations, minRoomWidth, minRoomLength);
        List<RoomNode> rooms = roomBuilder.buildRooms(leaves, leftModifier, rightModifier, roomOffset);

        CorridorsBuilder corridorsBuilder = new CorridorsBuilder(corridorWidth);
        var corridors = corridorsBuilder.buildCorridors(roomNodes);
        return new List<Node>(rooms).Concat(corridors).ToList();
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
