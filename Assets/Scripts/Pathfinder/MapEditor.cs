using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public enum PathCellMark {
    StartPos,
    EndPos,
    Normal,
    Block,
    Max,
}

public class MapEditor : MonoBehaviour {

    public Color[] types; // 颜色列表，0-起点，1-终点，2-一般网格，3-障碍
    private Dictionary<PathCellMark, Color> typeColors;

    public GridSystem gridSystem;

    private PathCellMark activeType;
    private Color activeColor;
    public Button bfsBtn;
    public Button aStarBtn;
    public Button beamSearchBtn;
    public InputField beamWidth;
    public Button cleanPathBtn;
    public Button clearBtn;
    public Button randomBlockBtn;
    public Button addChaserBtn;
    public Button highGvalBtn;
    public Button mediumGvalBtn;
    public Button lowGvalBtn;

    private int m_StartCellId;
    private int m_EndCellId;

    void Awake() {
        typeColors = new Dictionary<PathCellMark, Color>();
        //TODO: 读档，配置颜色
        //TODO: 可以的话，改成遍历
        typeColors.Add(PathCellMark.StartPos, types[(int)PathCellMark.StartPos]);
        typeColors.Add(PathCellMark.EndPos, types[(int)PathCellMark.EndPos]);
        typeColors.Add(PathCellMark.Normal, types[(int)PathCellMark.Normal]);
        typeColors.Add(PathCellMark.Block, types[(int)PathCellMark.Block]);

        ChangeActiveType(PathCellMark.StartPos);

        bfsBtn.onClick.AddListener(
            () => 
            {
                ResetColor(false);
                MapManager.Instance.PathFinder.FindPath(m_StartCellId, m_EndCellId, PathFindAlg.Bfs, DrawPath);
            }
        );
        aStarBtn.onClick.AddListener(
            () =>
            {
                ResetColor(false);
                MapManager.Instance.PathFinder.FindPath(m_StartCellId, m_EndCellId, PathFindAlg.Astar, DrawPath);
            }
        );
        beamSearchBtn.onClick.AddListener(
            () => 
            {
                ResetColor(false);

                int width;
                if (!int.TryParse(beamWidth.text, out width))
                    Debug.LogError("请输入正确数字");
                else
                {
                    MapManager.Instance.PathFinder.SetBeamWidth(width);
                    MapManager.Instance.PathFinder.FindPath(m_StartCellId, m_EndCellId, PathFindAlg.BeamSearch, DrawPath);
                }
            }
        );
        cleanPathBtn.onClick.AddListener(
            () => ResetColor(false)
        );
        clearBtn.onClick.AddListener(
            () => gridSystem.ClearBlocks()
        );
        randomBlockBtn.onClick.AddListener(
            () => gridSystem.GenerateRandomBlocks()
        );
        addChaserBtn.onClick.AddListener(
            () => SoldierManager.Instance.CreateSoldier()
        );
        highGvalBtn.onClick.AddListener(
            () =>
            {
                CellManager.Instance.SetPassableDifficulty(EPassableDifficulty.High);
            }
        );
        mediumGvalBtn.onClick.AddListener(
            () =>
            {
                CellManager.Instance.SetPassableDifficulty(EPassableDifficulty.Medium);
            }
        );
        lowGvalBtn.onClick.AddListener(
            () =>
            {
                CellManager.Instance.SetPassableDifficulty(EPassableDifficulty.Low);
            }
        );
    }

    // 触碰单元
    // 当选择一个新颜色时，在UI下面的单元也会被着色，所以询问事件系统是否检测到鼠标在某对象上
    void Update() {
        if (
                Input.GetMouseButton(0) &&
                !EventSystem.current.IsPointerOverGameObject()
            )
        {
            HandleInput();
        }
    }

    /// <summary>
    /// 检查输入
    /// </summary>
    void HandleInput() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.transform != null)
        {
            switch (activeType)
            {
                case PathCellMark.StartPos:

                    BaseCell lastStartCell = CellManager.Instance.GetCellByID(m_StartCellId);
                    //if (lastStartCell != null)
                    //    lastStartCell.GetComponent<SpriteRenderer>().color = types[2];

                    var startCell = hit.transform.GetComponent<BaseCell>();
                    if (startCell != null)
                        m_StartCellId = startCell.ID;

                    break;
                case PathCellMark.EndPos:

                    BaseCell lastEndCell = CellManager.Instance.GetCellByID(m_EndCellId);
                    //if (lastEndCell != null)
                    //    lastEndCell.GetComponent<SpriteRenderer>().color = types[2];

                    var endCell = hit.transform.GetComponent<BaseCell>();
                    if (endCell != null)
                        m_EndCellId = endCell.ID;
                    break;
                case PathCellMark.Block:
                    MapManager.Instance.PathFinder.SetBlock(hit.transform.GetComponent<BaseCell>().ID);
                    break;
                default:
                    break;
            }
            EditCell(gridSystem.GetCellByPos(CellManager.Instance.cellShape, hit.point));
        }
    }

    /// <summary>
    /// 完成网格单元的编辑工作
    /// </summary>
    /// <param name="cell">目标网格</param>
    void EditCell(BaseCell cell) {
        //SetCellColor(cell, activeColor);
    }

    void SetCellColor(BaseCell cell, Color color) {
        cell.spriteRenderer.color = color;
    }

    // TODO: 很丑，得改
    public void SelectColor(int index) {
        if (index == 0)
            ChangeActiveType(PathCellMark.StartPos);
        else if (index == 1)
            ChangeActiveType(PathCellMark.EndPos);
        else if (index == 2)
            ChangeActiveType(PathCellMark.Normal);
        else if (index == 3)
            ChangeActiveType(PathCellMark.Block);
        else
            ChangeActiveType(PathCellMark.Max);
    }

    /// <summary>
    /// 设置监听点击事件时，修改的网格颜色
    /// </summary>
    /// <param name="index">颜色下标 0-起点 1-终点 2-渐变</param>
    private void ChangeActiveType(PathCellMark mark) {
        activeType = mark;
        typeColors.TryGetValue(mark, out activeColor);
        if (activeColor == null)
        {
            Debug.Log("请求的标志不合法，没有对应的颜色");
            activeColor = Color.grey;
        }
    }

    public void ResetColor(bool all)
    {
        foreach (var cell in CellManager.Instance.cells)
        {
            if (all)
            {
                cell.GetComponent<SpriteRenderer>().color = Color.white;
                cell.SetIsBlock(false);
            }
            //else
            //{
            //    if (cell.ID == m_StartCellId)
            //        cell.GetComponent<SpriteRenderer>().color = Color.red;
            //    else if (cell.ID == m_EndCellId)
            //        cell.GetComponent<SpriteRenderer>().color = Color.black;
            //    else if (!cell.block)
            //        cell.GetComponent<SpriteRenderer>().color = Color.white;
            //}
        }
    }

    public void Clear()
    {
        m_StartCellId = -1;
        m_EndCellId = -1;
        ResetColor(false);
    }

    private void DrawPath(List<int> path)
    {
        foreach(var cellId in path)
        {
            var cell = CellManager.Instance.GetCellByID(cellId);
            if (cell == null)
                continue;

            //cell.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
}
