using UnityEngine;

public class Obstacle
{
        
    public enum EObstacleType
    {
        Box,
        Circle
    }

    private EObstacleType m_ObstacleType;

    public Vector2 Center { get; private set; }
    public float Radius { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }

    public Obstacle( Vector2 center, float radius,EObstacleType type = EObstacleType.Circle)
    {
        m_ObstacleType = type;
        Center = center;
        Radius = radius;
    }
    public Obstacle( Vector2 center, float width, float height, EObstacleType type = EObstacleType.Box)
    {
        m_ObstacleType = type;
        Center = center;
        Height = height;
        Width = width;
    }

    public float Distance(Vector2 point)
    {
        return (point - Center).magnitude;
    }
    public bool Intersact(Vector2 point, Vector2 point2)
    {
        var dir = point - Center;
        var dir2 = point2 - Center;
        if (m_ObstacleType == EObstacleType.Circle)
        {
            return dir.magnitude <= Radius || dir2.magnitude <= Radius;
        }

        if (m_ObstacleType == EObstacleType.Box)
        {
            bool YIntersact = Mathf.Abs(Vector2.Dot(dir, Vector2.up)) <= Height ||
                              Mathf.Abs(Vector2.Dot(dir2, Vector2.up)) <= Height;
            if (YIntersact)
            {
                return YIntersact;
            }

            bool XIntersact = Mathf.Abs(Vector2.Dot(dir, Vector2.right)) <= Width ||
                              Mathf.Abs(Vector2.Dot(dir2, Vector2.right)) <= Width;
            if (XIntersact)
            {
                return XIntersact;
            }
        }
        return false;
    }
    
}