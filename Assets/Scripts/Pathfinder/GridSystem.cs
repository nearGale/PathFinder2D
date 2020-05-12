using UnityEngine;
using UnityEngine.UI;

public enum CellShape {
    Square,
    Hexagon,
    SquareWithWall,
}

public class GridSystem : MonoBehaviour {
    // 公有的宽、高和单元预设体变量
    public int width = 6;
    public int height = 6;

    public float gridSize = 1;

    public CellShape m_CellShape;

    public HexCell hexCellPrefab;
    public SquareCell squareCellPrefab;
    public SquareCellWithWall squareCellWithWallPrefab;

    public Text cellLabelPrefab;

    Canvas gridCanvas;

    // 可配置的默认状态的和触碰时的颜色
    public Sprite defaultType = null;

    void Start() {
        GenerateMap(m_CellShape, width, height, gridSize);
    }

    void Update() { }

    public void GenerateMap(CellShape shape, int width, int height, float gridSize)
    {
        MapManager.Instance.SetMapSize(width, height, gridSize);
        MapManager.Instance.SetCellShape(shape);

        CreateCells(shape, width, height);
        RenderCells(shape);
    }

    private void CreateCells(CellShape cellShape, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(cellShape, x, y);
            }
        }
    }

    private void CreateCell(CellShape cellShape, int x, int y) {
        Vector2 position;

        CellManager.Instance.cellShape = cellShape;
        BaseCell cell;

        if(CellManager.Instance.cellShape == CellShape.Hexagon)
        {
            // 相邻六边形单元X方向的距离是内径的2倍
            // 每一行沿着X方向都有一个内径大小的偏移，需要取消一部分的偏移
            position.x = (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f) * gridSize;
            // 到下一行的距离应该是1.5倍的外径
            position.y = y * (HexMetrics.outerRadius * 1.5f) * gridSize;

            // 将单元保存在数组中，因为默认平面是10*10单位，将每个单元偏移那么多
            cell = Instantiate<HexCell>(hexCellPrefab);
            if (cell == null)
                Debug.LogError("生成了个空的");

            // 调整HexGrid.CreateCell来配合新坐标
            cell.coordinates = Coordinates.HexCoordinatesFromOffset(x, y);
        }
        else if(CellManager.Instance.cellShape == CellShape.Square)
        {
            position.x = x * SquareMetrics.sideLength * gridSize;
            position.y = y * SquareMetrics.sideLength * gridSize;

            // 将单元保存在数组中，因为默认平面是10*10单位，将每个单元偏移那么多
            cell = Instantiate<SquareCell>(squareCellPrefab);
            if (cell == null)
                Debug.LogError("生成了个空的");

            cell.coordinates = Coordinates.SquareCoordinatesFromOffset(x, y);

        }
        else if (CellManager.Instance.cellShape == CellShape.SquareWithWall)
        {
            position.x = x * SquareMetrics.sideLength * gridSize;
            position.y = y * SquareMetrics.sideLength * gridSize;

            // 将单元保存在数组中，因为默认平面是10*10单位，将每个单元偏移那么多
            cell = Instantiate<SquareCellWithWall>(squareCellWithWallPrefab);
            if (cell == null)
                Debug.LogError("生成了个空的");

            SquareCellWithWall sqCell = cell as SquareCellWithWall;
            if (sqCell != null)
                sqCell.InitWall();

            cell.coordinates = Coordinates.SquareCoordinatesFromOffset(x, y);
        }
        else
        {
            Debug.Log(string.Format("网格形状 {cellShape} 不符合要求"));
            return;
        }

        CellManager.Instance.AddCell(cell);

        int id = CellManager.Instance.GetIdByCoordinates(x, y);
        cell.SetID(id);

        cell.transform.SetParent(transform, false);
        cell.transform.localScale *= gridSize;
        cell.transform.localPosition = position;
        ResetNeightbor(cellShape, x, y);
    }

    private void RenderCells()
    {
        CellShape cellShape = CellManager.Instance.cellShape;
        RenderCells(cellShape);
    }

    private void RenderCells(CellShape cellShape)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                RenderCell(cellShape, x, y);
            }
        }
    }

    private void RenderCell(CellShape cellShape, int x, int y)
    {
        if (CellManager.Instance.cellShape == CellShape.SquareWithWall)
        {
            var cell = CellManager.Instance.GetCellByCoordinates(x, y) as SquareCellWithWall;
            if (cell == null)
                return;

            // 显示墙
            cell.RefreshDirWall(SquareDirection.W);
            cell.RefreshDirWall(SquareDirection.S);

            if (x == MapManager.Instance.MapWidth - 1)
            {
                cell.RefreshDirWall(SquareDirection.E);
            }

            if (y == MapManager.Instance.MapHeight - 1)
            {
                cell.RefreshDirWall(SquareDirection.N);
            }

            cell.RefreshIsBlock();
        }
    }

    private void ResetNeightbors()
    {
        CellShape cellShape = CellManager.Instance.cellShape;
        ResetNeightbors(cellShape);
    }

    private void ResetNeightbors(CellShape cellShape)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                ResetNeightbor(cellShape, x, y);
            }
        }
    }

    private void ResetNeightbor(CellShape cellShape, int x, int y)
    {
        int id = x + MapManager.Instance.MapWidth * y;

        if (cellShape == CellShape.Hexagon)
        {
            HexCell cell = CellManager.Instance.GetCellByCoordinates(x,y) as HexCell;

            if (cell == null)
                return;

            // 初始化 东-西方向 邻居关系
            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, CellManager.Instance.cells[id - 1] as HexCell);
            }
            // 有另外两个双向的连接需要完成。由于它们在不同行之间，我们只能连接之前的行。
            // 需要跳过整个第一行。
            if (y > 0)
            {
                // 偶数行
                if ((y & 1) == 0)
                {
                    // 所有单元都有东南方向的邻居。
                    cell.SetNeighbor(HexDirection.SE, CellManager.Instance.cells[id - width] as HexCell);
                    // 除了第一个单元，都有西南邻居。
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, CellManager.Instance.cells[id - width - 1] as HexCell);
                    }
                }
                // 奇数行
                else
                {
                    cell.SetNeighbor(HexDirection.SW, CellManager.Instance.cells[id - width] as HexCell);
                    if (x < width - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, CellManager.Instance.cells[id - width + 1] as HexCell);
                    }
                }
            }
        }
        else if(cellShape == CellShape.Square)
        {
            SquareCell cell = CellManager.Instance.GetCellByCoordinates(x, y) as SquareCell;

            if (cell == null)
                return;

            // 初始化 东-西方向 邻居关系
            if (x > 0)
            {
                cell.SetNeighbor(SquareDirection.W, CellManager.Instance.cells[id - 1] as SquareCell);
            }

            // 有另外两个双向的连接需要完成。由于它们在不同行之间，我们只能连接之前的行。
            // 需要跳过整个第一行。
            if (y > 0)
            {
                cell.SetNeighbor(SquareDirection.S, CellManager.Instance.cells[id - width] as SquareCell);
            }
        }
        else if(cellShape == CellShape.SquareWithWall)
        {
            SquareCellWithWall cell = CellManager.Instance.GetCellByCoordinates(x, y) as SquareCellWithWall;

            if (cell == null)
                return;

            // 初始化 东-西方向 邻居关系
            if (x > 0)
            {
                cell.SetNeighbor(SquareDirection.W, CellManager.Instance.cells[id - 1] as SquareCellWithWall);
            }

            if (y > 0)
            {
                cell.SetNeighbor(SquareDirection.S, CellManager.Instance.cells[id - width] as SquareCellWithWall);
            }
        }
        else
        {
            Debug.LogError("No Such Shape.");
        }
    }

    // 将触碰位置转换为六边形坐标
    public BaseCell GetCellByPos(CellShape shape, Vector3 position) {
        if(shape == CellShape.Hexagon)
        {
            position = transform.InverseTransformPoint(position);
            Coordinates coordinates = Coordinates.HexCoordinatesFromPosition(position, gridSize);
            // 首先将单元坐标转换为合适的数组索引，对于一个正方形网格就是X加Z乘以宽度
            // 还需要加入半-Z偏移量
            int index = coordinates.GetX() + coordinates.GetZ() * width + coordinates.GetZ() / 2;
            BaseCell cell = CellManager.Instance.cells[index];
            return cell;
        }
        else if(shape == CellShape.Square || shape == CellShape.SquareWithWall)
        {
            position = transform.InverseTransformPoint(position);
            Coordinates coordinates = Coordinates.SquareCoordinatesFromPosition(position, gridSize);
            // 首先将单元坐标转换为合适的数组索引，对于一个正方形网格就是X加Z乘以宽度
            // 还需要加入半-Z偏移量
            int index = coordinates.GetX() + coordinates.GetZ() * width;
            BaseCell cell = CellManager.Instance.cells[index];
            return cell;
        }

        return null;
    }
    
    /// <summary>
    /// 随机生成障碍
    /// </summary>
    /// <param name="baseProbability">会发生的概率，百分制</param>
    public void GenerateRandomBlocks(int baseProbability = -1)
    {
        ResetNeightbors();

        if (baseProbability < 0 || baseProbability > 100)
            baseProbability = 30;

        if(CellManager.Instance.cellShape == CellShape.Hexagon || CellManager.Instance.cellShape == CellShape.Square)
        {
            foreach (var cell in CellManager.Instance.cells)
            {
                int neighborBlock = 0;
                foreach (var neighbor in cell.neighbors)
                {
                    if (neighbor == null)
                        continue;
                    if (neighbor.block)
                        neighborBlock++;
                }

                int probability = baseProbability + 10 * neighborBlock;
                if (UnityEngine.Random.Range(0, 100) <= probability)
                {
                    cell.SetIsBlock(true);
                }
            }
        }
        else if (CellManager.Instance.cellShape == CellShape.SquareWithWall)
        {
            for (int y = 0; y < MapManager.Instance.MapHeight; y++)
            {
                for (int x = 0; x < MapManager.Instance.MapWidth; x++)
                {
                    if (x > 0)
                    {
                        SquareCellWithWall cell = CellManager.Instance.GetCellByCoordinates(x, y) as SquareCellWithWall;
                        
                        if(cell.neighbors[(int)SquareDirection.W] != null)
                        {
                            SetRandomBlock(cell, SquareDirection.W, baseProbability);
                        }
                    }

                    if(y > 0)
                    {
                        SquareCellWithWall cell = CellManager.Instance.GetCellByCoordinates(x, y) as SquareCellWithWall;

                        if (cell.neighbors[(int)SquareDirection.S] != null)
                        {
                            SetRandomBlock(cell, SquareDirection.S, baseProbability);
                        }
                    }
                }
            }
        }

        RenderCells();
    }

    private void SetRandomBlock(SquareCellWithWall self, SquareDirection neighborDir, int probability)
    {
        if (self == null)
            return;
        
        SquareCellWithWall neighbor = self.neighbors[(int)neighborDir] as SquareCellWithWall;

        if (self == null)
            return;

        if (neighbor == null)
            return;

        if (UnityEngine.Random.Range(0, 100) <= probability)
        {
            self.SetNeighbor(neighborDir, null);

            SquareDirection selfDir = neighborDir.Opposite();
            neighbor.SetNeighbor(selfDir, null);
        }
    }
}
