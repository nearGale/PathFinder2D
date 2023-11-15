using UnityEngine;

[System.Serializable]
public class Coordinates
{

    [SerializeField]
    private int x, z;

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return -x - z;
    }

    public int GetZ()
    {
        return z;
    }

    public Coordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override string ToString()
    {
        return "(" +
            GetX().ToString() + ", " + GetY().ToString() + ", " + GetZ().ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return GetX().ToString() + "\n" + GetY().ToString() + "\n" + GetZ().ToString();
    }

    public static Coordinates SquareCoordinatesFromOffset(int x, int z)
    {
        // 修正那些X坐标让它们沿直线排开
        // 想要这样我们可以取消水平调整。结果我们得到了轴坐标
        return new Coordinates(x, z);
    }

    // 计算出哪个坐标属于某一位置
    public static Coordinates SquareCoordinatesFromPosition(Vector2 position, float size)
    {
        float x = position.x / (SquareMetrics.sideLength * size);
        float z = position.y / (SquareMetrics.sideLength * size);
        int iX = Mathf.RoundToInt(x);
        int iZ = Mathf.RoundToInt(z);
        return new Coordinates(iX, iZ);
    }

    public static Coordinates HexCoordinatesFromOffset(int x, int z)
    {
        // 修正那些X坐标让它们沿直线排开
        // 想要这样我们可以取消水平调整。结果我们得到了轴坐标
        return new Coordinates(x - z / 2, z);
    }

    public static Coordinates HexCoordinatesFromPosition(Vector2 position, float size)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f * size);
        float y = -x;
        float offset = position.y / (HexMetrics.outerRadius * 3f * size);
        x -= offset;
        y -= offset;
        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        if (iX + iY + iZ != 0)
        {
            // 问题貌似只会出现在靠近六边形相邻边的位置，所以是对坐标的取整导致了问题
            // Debug.LogWarning("rounding error!");

            // 解法就变成了要摒弃取整幅度最大的坐标，然后从另外两个重新计算它。
            // 由于我们只需要X和Z，我们不需要重建Y。
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);
            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }
        return new Coordinates(iX, iZ);
    }
}
