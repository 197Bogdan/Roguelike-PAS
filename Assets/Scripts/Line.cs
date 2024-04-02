using UnityEngine;
public class Line
{
    Orientation orientation;
    Vector2Int point;

    public Line(Orientation orientation, Vector2Int point)
    {
        this.orientation = orientation;
        this.point = point;
    }

    public Orientation Orientation { get => orientation; set => orientation = value; }
    public Vector2Int Coordinates { get => point; set => point = value; }
}

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}