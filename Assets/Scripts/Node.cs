using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    public Node parent;
    public int treeIndex;
    public bool visited;
    public Vector2Int bottomLeft, bottomRight, topLeft, topRight;
    public List<Node> children;

    public Node(Node parentNode)
    {
        this.children = new List<Node>();
        this.parent = parentNode;
        
        if (parentNode != null)
        {
            parentNode.children.Add(this);
        }
    }
    }
