using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorNode : Node
{
    private Node node1;
    private Node node2;
    private int corridorWidth;
    private int distanceFromWall = 1;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.node1 = node1;
        this.node2 = node2;
        this.corridorWidth = corridorWidth;
        generateCorridor();
    }

    private void generateCorridor()
    {
        var relativePos = checkPosNode2ToNode1();
        switch (relativePos)
        {
            case RelativePosition.RIGHT:
                generateRightOrLeft(node1, node2);
                break;
            case RelativePosition.UP:
                generateUpOrDown(node1, node2);
                break;
            case RelativePosition.DOWN:
                generateUpOrDown(node2, node1);
                break;
            case RelativePosition.LEFT:
                generateRightOrLeft(node2, node1);
                break;
        }
    }

    private void generateUpOrDown(Node node1, Node node2)
    {
        Node bottomNode = null;
        Node topNode = null;
        List<Node> bottomNodeLeaves = Helper.extractLeaves(node1);
        List<Node> topNodeLeaves = Helper.extractLeaves(node2);

        var sortedBottomNodeLeaves = bottomNodeLeaves.OrderByDescending(node => node.topRight.y).ToList();
        if(sortedBottomNodeLeaves.Count == 1)
            bottomNode = sortedBottomNodeLeaves[0];
        else
        {
            int maxY = sortedBottomNodeLeaves[0].topLeft.y;
            sortedBottomNodeLeaves = sortedBottomNodeLeaves.Where(node => Math.Abs(maxY - node.topLeft.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomNodeLeaves.Count);
            bottomNode = sortedBottomNodeLeaves[index];
        }

        var possibleTopNodeNeighbors = topNodeLeaves.Where(
                       node => validBottomNeighborX(
                            bottomNode.topLeft, bottomNode.topRight,
                            node.bottomLeft, node.bottomRight
                            ) != -1
                       ).OrderBy(node => node.bottomRight.y).ToList();

        if (possibleTopNodeNeighbors.Count == 0)
            topNode = node2;
        else
            topNode = possibleTopNodeNeighbors[0];

        int x = validBottomNeighborX(bottomNode.topLeft, bottomNode.topRight, topNode.bottomLeft, topNode.bottomRight);
        while(x == -1 && sortedBottomNodeLeaves.Count > 1)
        {
            sortedBottomNodeLeaves = sortedBottomNodeLeaves.Where(node => node.topLeft.x != topNode.topLeft.x).ToList();
            bottomNode = sortedBottomNodeLeaves[0];
            x = validBottomNeighborX(bottomNode.topLeft, bottomNode.topRight, topNode.bottomLeft, topNode.bottomRight);
        }
        bottomLeft = new Vector2Int(x, bottomNode.topLeft.y);
        topRight = new Vector2Int(x + corridorWidth, topNode.bottomLeft.y);
    }

    private int validBottomNeighborX(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if(topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return Helper.calculateMiddlePoint(
                               bottomNodeLeft + new Vector2Int(distanceFromWall, 0),
                               bottomNodeRight - new Vector2Int(distanceFromWall + corridorWidth, 0)
                               ).x;

        }
        if(topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return Helper.calculateMiddlePoint(
                               topNodeLeft + new Vector2Int(distanceFromWall, 0),
                               topNodeRight - new Vector2Int(distanceFromWall + corridorWidth, 0)
                               ).x;

        }
        if(bottomNodeLeft.x >= topNodeLeft.x && bottomNodeLeft.x <= topNodeRight.x)
        {
            return Helper.calculateMiddlePoint(
                                bottomNodeLeft + new Vector2Int(distanceFromWall, 0),
                                topNodeRight - new Vector2Int(distanceFromWall + corridorWidth, 0)
                                ).x;
        }
        if(bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return Helper.calculateMiddlePoint(
                               topNodeLeft + new Vector2Int(distanceFromWall, 0),
                               bottomNodeRight - new Vector2Int(distanceFromWall + corridorWidth, 0)
                               ).x;
        }
        return -1;
    }

    private void generateRightOrLeft(Node node1, Node node2)
    {
        Node leftNode = null;
        Node rightNode = null;
        List<Node> leftNodeLeaves = Helper.extractLeaves(node1);
        List<Node> rightNodeLeaves = Helper.extractLeaves(node2);

        var sortedLeftNodeLeaves = leftNodeLeaves.OrderByDescending(node => node.topRight.x).ToList();
        if(sortedLeftNodeLeaves.Count == 1)
            leftNode = sortedLeftNodeLeaves[0];
        else
        {
            int maxX = sortedLeftNodeLeaves[0].topRight.x;
            sortedLeftNodeLeaves = sortedLeftNodeLeaves.Where(node => Math.Abs(maxX - node.topRight.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftNodeLeaves.Count);
            leftNode = sortedLeftNodeLeaves[index];
        }

        var possibleRightNodeNeighbors = rightNodeLeaves.Where(
            node => validLeftNeighborY(
                leftNode.topRight, leftNode.bottomRight,
                node.topLeft, node.bottomLeft
                ) != -1
            ).OrderBy(node => node.bottomRight.x).ToList();

        if (possibleRightNodeNeighbors.Count == 0)
            rightNode = node2;
        else
            rightNode = possibleRightNodeNeighbors[0];

        int y = validLeftNeighborY(leftNode.topLeft, leftNode.bottomRight, rightNode.topLeft, rightNode.bottomLeft);
        while(y == -1 && sortedLeftNodeLeaves.Count > 1)
        {
            sortedLeftNodeLeaves = sortedLeftNodeLeaves.Where(node => node.topLeft.y != leftNode.topLeft.y).ToList();
            leftNode = sortedLeftNodeLeaves[0];
            y = validLeftNeighborY(leftNode.topLeft, leftNode.bottomRight, rightNode.topLeft, rightNode.bottomLeft);
        }

        bottomLeft = new Vector2Int(leftNode.bottomRight.x, y);
        topRight = new Vector2Int(rightNode.topLeft.x, y + corridorWidth);
    }

    private int validLeftNeighborY(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
            return Helper.calculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, distanceFromWall),
                leftNodeUp - new Vector2Int(0, distanceFromWall + corridorWidth)
                ).y;
         if(rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
            return Helper.calculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, distanceFromWall),
                rightNodeUp - new Vector2Int(0, distanceFromWall + corridorWidth)
                ).y;
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
            return Helper.calculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, distanceFromWall),
                leftNodeUp - new Vector2Int(0, distanceFromWall + corridorWidth)
                ).y;
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
            return Helper.calculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, distanceFromWall),
                rightNodeUp - new Vector2Int(0, distanceFromWall + corridorWidth)
                ).y;
        return -1;
    }

    private object checkPosNode2ToNode1()
    {
        Vector2 node1Center = (Vector2)(node1.topRight + node1.bottomLeft) / 2;
        Vector2 node2Center = (Vector2)(node2.topRight + node2.bottomLeft) / 2;
        float angle = Mathf.Atan2(node2Center.y - node1Center.y, node2Center.x - node1Center.x) * Mathf.Rad2Deg;
        if (angle < 45 && angle > -45)
            return RelativePosition.RIGHT;
        else if (angle < 135 && angle > 45)
            return RelativePosition.UP;
        else if (angle < -45 && angle > -135)
            return RelativePosition.DOWN;
        else
            return RelativePosition.LEFT;
    }
}