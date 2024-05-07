using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Helper
{
    public static List<Node> getLeaves(Node parentNode)
    {
        Queue<Node> nodeQueue = new Queue<Node>();
        List<Node> leaves = new List<Node>();
        if (parentNode.children.Count == 0)
        {
            return new List<Node>() { parentNode };
        }
        foreach (var child in parentNode.children)
        {
            nodeQueue.Enqueue(child);
        }
        while (nodeQueue.Count > 0)
        {
            var currentNode = nodeQueue.Dequeue();
            if (currentNode.children.Count == 0)
            {
                leaves.Add(currentNode);
            }
            else
            {
                foreach (var child in currentNode.children)
                {
                    nodeQueue.Enqueue(child);
                }
            }
        }
        return leaves;
    }

    public static Vector2Int generateBottomLeftBetween(
        Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;
        return new Vector2Int(
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (minY - minY) * pointModifier)));
    }

    public static Vector2Int generateTopRightBetween(
        Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;
        return new Vector2Int(
            Random.Range((int)(minX+(maxX-minX)*pointModifier),maxX),
            Random.Range((int)(minY+(maxY-minY)*pointModifier),maxY)
            );
    }

    public static Vector2Int calculateMiddle(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;
        return new Vector2Int((int)tempVector.x, (int)tempVector.y);
    }
}

public enum RelativePosition
{
    Up,
    Down,
    Right,
    Left
}