using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BinarySpacePartitioner
{
    RoomNode rootNode;

    public RoomNode RootNode { get => rootNode; }
    public BinarySpacePartitioner(int dungeonWidth, int dungeonLength)
    {
        this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonLength), null, 0);
    }

    public List<RoomNode> getSplitNodes(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<RoomNode> nodeQueue = new Queue<RoomNode>();
        List<RoomNode> nodes = new List<RoomNode>();
        nodeQueue.Enqueue(this.rootNode);
        nodes.Add(this.rootNode);
        int iterations = 0;
        while (iterations < maxIterations && nodeQueue.Count>0)
        {
            iterations++;
            RoomNode currentNode = nodeQueue.Dequeue();
            if(currentNode.Width>=roomWidthMin*2 || currentNode.Length >= roomLengthMin * 2)
                splitSpace(currentNode, nodes, roomLengthMin, roomWidthMin, nodeQueue);
        }
        return nodes;
    }

    private void splitSpace(RoomNode currentNode, List<RoomNode> nodes, int roomLengthMin, int roomWidthMin, Queue<RoomNode> nodeQueue)
    {
        Vector2Int newTopRight, newBottomLeft;
        Line line = getDivideLine(currentNode.bottomLeft, currentNode.topRight, roomWidthMin, roomLengthMin);
        RoomNode node1, node2;
        if(line.Orientation == Orientation.Horizontal)
        {
            newTopRight = new Vector2Int(currentNode.topRight.x, line.Coordinates.y);
            node1 = new RoomNode(
                currentNode.bottomLeft,
                newTopRight,
                currentNode,
                currentNode.treeIndex + 1);
            newBottomLeft = new Vector2Int(currentNode.bottomLeft.x, line.Coordinates.y);
            node2 = new RoomNode(
                newBottomLeft,
                currentNode.topRight,
                currentNode,
                currentNode.treeIndex + 1);
        }
        else
        {
            newTopRight = new Vector2Int(line.Coordinates.x, currentNode.topRight.y);
            node1 = new RoomNode(
                currentNode.bottomLeft,
                newTopRight,
                currentNode,
                currentNode.treeIndex + 1);
            newBottomLeft = new Vector2Int(line.Coordinates.x, currentNode.bottomLeft.y);
            node2 = new RoomNode(
                newBottomLeft,
                currentNode.topRight,
                currentNode,
                currentNode.treeIndex + 1);
        }
        addToProcess(nodes, nodeQueue, node1);
        addToProcess(nodes, nodeQueue, node2);
    }

    private void addToProcess(List<RoomNode> nodes, Queue<RoomNode> nodeQueue, RoomNode node)
    {
        nodes.Add(node);
        nodeQueue.Enqueue(node);
    }

    private Line getDivideLine(Vector2Int bottomLeft, Vector2Int topRight, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;
        bool lengthDivide = (topRight.y - bottomLeft.y) >= 2 * roomLengthMin;
        bool widthDivide = (topRight.x - bottomLeft.x) >= 2 * roomWidthMin;

        if (lengthDivide && widthDivide)
            orientation = (Orientation)(Random.Range(0,2));
        else if (widthDivide)
            orientation = Orientation.Vertical;
        else
            orientation = Orientation.Horizontal;

        return new Line(
            orientation,
            getPoint(orientation, bottomLeft, topRight, roomWidthMin, roomLengthMin)
            );
    }

    private Vector2Int getPoint(Orientation orientation, Vector2Int bottomLeft, Vector2Int topRight, int roomWidthMin, int roomLengthMin)
    {
        Vector2Int point = Vector2Int.zero;
        if (orientation == Orientation.Horizontal)
        {
            int newY = Random.Range((bottomLeft.y + roomLengthMin), (topRight.y - roomLengthMin));
            point = new Vector2Int(0, newY);
        }
        else
        {
            int newX = Random.Range((bottomLeft.x + roomWidthMin), (topRight.x - roomWidthMin));
            point = new Vector2Int(newX, 0);
        }
        return point;
    }
}