using System.Collections.Generic;
using System.Linq;

public class CorridorsBuilder
{
    public List<Node> buildCorridors(List<RoomNode> nodes, int corridorWidth)
    {
        List<Node> corridors = new List<Node>();
        Queue<RoomNode> nodeQueue = new Queue<RoomNode>(
            nodes.OrderByDescending(node => node.treeIndex).ToList());
        while (nodeQueue.Count > 0)
        {
            var node = nodeQueue.Dequeue();

            if (node.children.Count == 0)
                continue;

            CorridorNode corridor = new CorridorNode(node.children[0], node.children[1], corridorWidth);
            corridors.Add(corridor);
        }
        return corridors;
    }
}