using UnityEngine;

internal class Line
{
    public Orientation orientation;
    public Vector2Int point;

    public Line(Orientation orientation, Vector2Int point)
    {
        this.orientation = orientation;
        this.point = point;
    }
}

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}
