using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

internal class Helper
{
    public static List<Node> extractLeaves(Node rootRoom)
    {
        Queue<Node> nodes = new Queue<Node>();
        List<Node> leaves = new List<Node>();
        if (rootRoom.children.Count == 0)
            return new List<Node> { rootRoom };
        foreach (Node child in rootRoom.children)
        {
            nodes.Enqueue(child);
        }
        while (nodes.Count > 0)
        {
            Node node = nodes.Dequeue();
            if (node.children.Count == 0)
            {
                leaves.Add(node);
            }
            else
            {
                foreach (Node child in node.children)
                {
                    nodes.Enqueue(child);
                }
            }
        }
        return leaves;
    }

    public static Vector2Int generateBottomLeft(Vector2Int bottomLeft, Vector2Int topRight, float modifier, int offset)
    {
        int minX = bottomLeft.x + offset;
        int maxX = topRight.x - offset;
        int minY = bottomLeft.y + offset;
        int maxY = topRight.y - offset;

        int x = Random.Range(minX, (int)(minX + (maxX - minX) * modifier));
        int y = Random.Range(minY, (int)(minY + (maxY - minY) * modifier));
        return new Vector2Int(x, y);
    }

    public static Vector2Int generateTopRight(Vector2Int bottomLeft, Vector2Int topRight, float modifier, int offset)
    {
        int minX = bottomLeft.x + offset;
        int maxX = topRight.x - offset;
        int minY = bottomLeft.y + offset;
        int maxY = topRight.y - offset;

        int x = Random.Range((int)(maxX - (maxX - minX) * modifier), maxX);
        int y = Random.Range((int)(maxY - (maxY - minY) * modifier), maxY);
        return new Vector2Int(x, y);
    }
}