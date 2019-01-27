using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Point
{

    public int x { get; set; }

    public int y { get; set; }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;

    }
    public Point westNeighbour { get => new Point(x - 1, y); }
    public Point eastNeighbour { get => new Point(x + 1, y); }
    public Point northNeighbour { get => new Point(x, y+1); }
    public Point southNeighbour { get => new Point(x, y-1); }
    public Point northwestNeighbour { get => new Point(x - 1, y + 1); }
    public Point southwestNeighbour { get => new Point(x - 1, y - 1); }
    public Point northeastNeighbour { get => new Point(x + 1, y + 1); }
    public Point southeastNeighbour { get => new Point(x + 1, y - 1); }





    //implements boolean equality comparisons for Points

    public static bool operator ==(Point A, Point B)
    {
        return A.x == B.x && A.y == B.y;
    }
    // note both == and != must be defined.
    public static bool operator !=(Point A, Point B)
    {
        return A.x != B.x || A.y != B.y;
    }
    public static Point operator -(Point A, Point B)
    {
        return new Point(A.x - B.x, A.y - B.y);
    }

    public  bool Equals(Point A, Point B)
    {
        return A.x == B.x && A.y == B.y;
    }
    public int GetHashCode(Point obj)
    {
        return obj.GetHashCode();
    }
}



