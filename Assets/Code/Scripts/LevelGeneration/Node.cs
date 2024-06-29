using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Node
{
    public List<Node> childrenNodeList;
    public List<Node> children { get => childrenNodeList; }

    public Vector2Int bottomLeft;
    public Vector2Int bottomRight; 
    public Vector2Int topRight; 
    public Vector2Int topLeft; 
    public Node parent;
    public int treeIndex;

    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        this.parent = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);

    }

    public void RemoveChild(Node node)
    {
        childrenNodeList.Remove(node);
    }
}