using System.Collections.Generic;
using System.Linq;

public class CorridorsBuilder
{
    private int corridorWidth;

    public CorridorsBuilder(int corridorWidth)
    {
        this.corridorWidth = corridorWidth;
    }

    public List<Node> buildCorridors(List<RoomNode> roomNodes)
    {
        List<Node> corridors = new List<Node>();
        Queue<RoomNode> roomQueue = new Queue<RoomNode>(roomNodes.OrderByDescending(node => node.treeIndex).ToList());

        while(roomQueue.Count > 1)
        {
            var node = roomQueue.Dequeue();
            if (node.children.Count == 0)
                continue;
            CorridorNode corridor = new CorridorNode(node.children[0], node.children[1], corridorWidth);
            corridors.Add(corridor);
        }
        return corridors;
    }
}