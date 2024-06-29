using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CorridorNode : Node
{
    private Node node1;
    private Node node2;
    private int corridorWidth;
    private int wallDistance=1;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.node1 = node1;
        this.node2 = node2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = getRelativePosNode2ToNode1();
        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                generateBetweenUpAndDown(node1, node2);
                break;
            case RelativePosition.Down:
                generateBetweenUpAndDown(node2, node1);
                break;
            case RelativePosition.Right:
                generateBetweenLeftAndRight(node1, node2);
                break;
            case RelativePosition.Left:
                generateBetweenLeftAndRight(node2, node1);
                break;
        }
    }

    private void generateBetweenLeftAndRight(Node node1, Node node2)
    {
        Node leftNode = null;
        List<Node> leftNodeLeaves = Helper.getLeaves(node1);

        Node rightNode = null;
        List<Node> rightNodeLeaves = Helper.getLeaves(node2);

        var sortedLeftNodeLeaves = leftNodeLeaves.OrderByDescending(child => child.topRight.x).ToList();
        if (sortedLeftNodeLeaves.Count == 1)
            leftNode = sortedLeftNodeLeaves[0];
        else
        {
            int maxX = sortedLeftNodeLeaves[0].topRight.x;
            sortedLeftNodeLeaves = sortedLeftNodeLeaves.Where(children => Math.Abs(maxX - children.topRight.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftNodeLeaves.Count);
            leftNode = sortedLeftNodeLeaves[index];
        }

        var possibleRightNeighbors = rightNodeLeaves.Where(
            child => getRightNeighborY(
                leftNode.topRight, leftNode.bottomRight,
                child.topLeft, child.bottomLeft
                ) != -1
            ).OrderBy(child => child.bottomRight.x).ToList();

        if (possibleRightNeighbors.Count <= 0)
            rightNode = node2;
        else
            rightNode = possibleRightNeighbors[0];

        int y = getRightNeighborY(leftNode.topLeft, leftNode.bottomRight, rightNode.topLeft, rightNode.bottomLeft);
        while(y==-1 && sortedLeftNodeLeaves.Count > 1)
        {
            sortedLeftNodeLeaves = sortedLeftNodeLeaves.Where(child => child.topLeft.y != leftNode.topLeft.y).ToList();
            leftNode = sortedLeftNodeLeaves[0];
            y = getRightNeighborY(leftNode.topLeft, leftNode.bottomRight, rightNode.topLeft, rightNode.bottomLeft);
        }

        bottomLeft = new Vector2Int(leftNode.bottomRight.x, y);
        topRight = new Vector2Int(rightNode.topLeft.x, y + corridorWidth);
    }

    private int getRightNeighborY(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if(rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return Helper.calculateMiddle(
                leftNodeDown + new Vector2Int(0, wallDistance),
                leftNodeUp - new Vector2Int(0, wallDistance + corridorWidth)
                ).y;
        }
        if(rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return Helper.calculateMiddle(
                rightNodeDown+new Vector2Int(0, wallDistance),
                rightNodeUp - new Vector2Int(0, wallDistance + corridorWidth)
                ).y;
        }
        if(leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return Helper.calculateMiddle(
                rightNodeDown+new Vector2Int(0, wallDistance),
                leftNodeUp-new Vector2Int(0, wallDistance)
                ).y;
        }
        if(leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return Helper.calculateMiddle(
                leftNodeDown+new Vector2Int(0 ,wallDistance),
                rightNodeUp-new Vector2Int(0, wallDistance + corridorWidth)
                ).y;
        }
        return- 1;
    }

    private void generateBetweenUpAndDown(Node node1, Node node2)
    {
        Node bottomNode = null;
        List<Node> bottomNodeLeaves = Helper.getLeaves(node1);

        Node topNode = null;
        List<Node> topNodeLeaves = Helper.getLeaves(node2);

        var sortedBottomLeaves = bottomNodeLeaves.OrderByDescending(child => child.topRight.y).ToList();

        if (sortedBottomLeaves.Count == 1)
            bottomNode = bottomNodeLeaves[0];
        else
        {
            int maxY = sortedBottomLeaves[0].topLeft.y;
            sortedBottomLeaves = sortedBottomLeaves.Where(child => Mathf.Abs(maxY - child.topLeft.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomLeaves.Count);
            bottomNode = sortedBottomLeaves[index];
        }

        var possibleTopNeighbors = topNodeLeaves.Where(
            child => getUpNeighborX(
                bottomNode.topLeft, bottomNode.topRight,
                child.bottomLeft, child.bottomRight)
            != -1).OrderBy(child => child.bottomRight.y).ToList();

        if (possibleTopNeighbors.Count == 0)
            topNode = node2;
        else
            topNode = possibleTopNeighbors[0];

        int x = getUpNeighborX(bottomNode.topLeft, bottomNode.topRight, topNode.bottomLeft, topNode.bottomRight);
        while(x==-1 && sortedBottomLeaves.Count > 1)
        {
            sortedBottomLeaves = sortedBottomLeaves.Where(child => child.topLeft.x != topNode.topLeft.x).ToList();
            bottomNode = sortedBottomLeaves[0];
            x = getUpNeighborX(bottomNode.topLeft, bottomNode.topRight, topNode.bottomLeft, topNode.bottomRight);
        }

        bottomLeft = new Vector2Int(x, bottomNode.topLeft.y);
        topRight = new Vector2Int(x + corridorWidth, topNode.bottomLeft.y);
    }

    private int getUpNeighborX(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if(topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return Helper.calculateMiddle(
                bottomNodeLeft + new Vector2Int(wallDistance, 0),
                bottomNodeRight - new Vector2Int(corridorWidth + wallDistance, 0)
                ).x;
        }
        if(topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return Helper.calculateMiddle(
                topNodeLeft+new Vector2Int(wallDistance, 0),
                topNodeRight - new Vector2Int(corridorWidth + wallDistance, 0)
                ).x;
        }
        if(bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return Helper.calculateMiddle(
                bottomNodeLeft + new Vector2Int(wallDistance, 0),
                topNodeRight - new Vector2Int(corridorWidth + wallDistance, 0)

                ).x;
        }
        if(bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return Helper.calculateMiddle(
                topNodeLeft + new Vector2Int(wallDistance, 0),
                bottomNodeRight - new Vector2Int(corridorWidth + wallDistance, 0)

                ).x;
        }
        return -1;
    }

    private RelativePosition getRelativePosNode2ToNode1()
    {
        Vector2 node1Middle = ((Vector2)node1.topRight + node1.bottomLeft) / 2;
        Vector2 node2Middle = ((Vector2)node2.topRight + node2.bottomLeft) / 2;
        float angle = Mathf.Atan2(node2Middle.y - node1Middle.y, node2Middle.x - node1Middle.x) * Mathf.Rad2Deg;
        if (angle < 45 && angle > -45)
        {
            return RelativePosition.Right;
        }
        else if(angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if(angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }
}