using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathFindAlg {
    Bfs,
    Astar,
    BeamSearch,
}

public enum SearchState
{
    Searching,
    Finish,
    None,
}

public class PathFindReq
{
    public int startCellId;
    public int endCellId;
    public PathFindAlg type;
    public Action<List<int>> action;
}

public class PathFinder {

    public bool needDemonstration = false;
    public float searchInterval = 0.01f;

    private List<BaseCell> m_CellsSearched;
    private List<BaseCell> m_CellsToSearch;
    private List<int> m_Path;

    public SearchState SearchState { get { return m_SearchState; } }
    private SearchState m_SearchState;

    public int BeamWidth { get { return m_BeamWidth; } }
    private int m_BeamWidth;

    private List<BaseCell> m_CellsToSearchBackup;

    private Queue<PathFindReq> m_Requests;

    public void Init() {
        m_CellsSearched = new List<BaseCell>();
        m_CellsToSearch = new List<BaseCell>();
        m_Path = new List<int>();
        m_BeamWidth = 100;
        m_CellsToSearchBackup = new List<BaseCell>();
        m_SearchState = SearchState.None;

        m_Requests = new Queue<PathFindReq>();
    }

    private bool CheckCanSearch(int startCellId, int endCellId) {
        var startCell = CellManager.Instance.GetCellByID(startCellId);
        if (startCell == null)
            return false;

        var endCell = CellManager.Instance.GetCellByID(endCellId);
        if (endCell == null)
            return false;

        return true;
    }

    public void FindPathRequest(int startCellId, int endCellId, PathFindAlg type, Action<List<int>> action = null)
    {
        PathFindReq req = new PathFindReq();
        req.startCellId = startCellId;
        req.endCellId = endCellId;
        req.type = type;
        req.action = action;

        foreach(var request in m_Requests)
        {
            if (request.startCellId == startCellId &&
                request.endCellId == endCellId &&
                request.type == type &&
                request.action == action)
                return;
        }

        m_Requests.Enqueue(req);
    }

    public void ConductFindPath(int num)
    {
        if (m_Requests.Count == 0)
            return;

        int conductNum = Math.Min(m_Requests.Count, num);

        for (int i = 0; i < conductNum; i++)
        {
            PathFindReq req = m_Requests.Dequeue();
            FindPath(req);
        }
    }

    private void FindPath(PathFindReq req)
    {
        if (req == null)
            return;

        FindPath(req.startCellId, req.endCellId, req.type, req.action);
    }

    public void FindPath(int startCellId, int endCellId, PathFindAlg type, Action<List<int>> action = null)
    {
        switch (type)
        {
            case PathFindAlg.Bfs:
                BfsSearch(startCellId, endCellId, action);
                break;
            case PathFindAlg.Astar:
                AStarSearch(startCellId, endCellId, action);
                break;
            case PathFindAlg.BeamSearch:
                BeamSearch(startCellId, endCellId, action);
                break;
            default:
                break;
        }
    }

    private BaseCell OnSearchStart(int startCellId)
    {
        m_SearchState = SearchState.Searching;

        m_CellsSearched.Clear();
        m_CellsToSearch.Clear();
        m_CellsToSearchBackup.Clear();
        m_Path.Clear();

        BaseCell searchingCell = CellManager.Instance.GetCellByID(startCellId); // Cell To Search Right Now
        if (searchingCell != null)
        {
            searchingCell.SetParent(-1);
            m_CellsToSearch.Add(searchingCell);
        }

        return searchingCell;
    }

    private void OnSearchEnd(Action<List<int>> action = null)
    {
        m_SearchState = SearchState.Finish;

        action?.Invoke(m_Path);
    }


    public void SetBlock(int cellID) {
        BaseCell blockCell = CellManager.Instance.GetCellByID(cellID);
        if (blockCell == null)
        {
            Debug.Log("blockCell = null");
            return;
        }
        blockCell.SetIsBlock(true);
    }

    #region BFS
    // TODO: 路径标记有问题，非最短路径
    private void BfsSearch(int startCellId, int endCellId, Action<List<int>> action = null)
    {
        if (!CheckCanSearch(startCellId, endCellId))
            return;

        BaseCell searchingCell = OnSearchStart(startCellId);

        if (searchingCell == null)
            return;

        bool keepSearching = true;
        while (keepSearching)
        {
            foreach (BaseCell neighbor in searchingCell.neighbors)
            {
                if (neighbor == null || neighbor.block)
                    continue;
                if (m_CellsSearched.Contains(neighbor))
                    continue;
                neighbor.SetParent(searchingCell.ID);

                if (neighbor.ID == endCellId)
                {
                    // 走到了

                    m_Path.Add(neighbor.ID);

                    if (needDemonstration)
                        neighbor.GetComponent<SpriteRenderer>().color = Color.blue;

                    BaseCell traceNeighbor = neighbor;
                    int i = 0;
                    while (CellManager.Instance.GetCellByID(traceNeighbor.parentID) != null && i < 100)
                    {
                        Debug.Log(traceNeighbor.parentID);
                        traceNeighbor = CellManager.Instance.GetCellByID(traceNeighbor.parentID);

                        m_Path.Add(traceNeighbor.ID);
                        if (needDemonstration)
                            traceNeighbor.GetComponent<SpriteRenderer>().color = Color.blue;
                        i++;
                    }

                    m_Path.Reverse();

                    OnSearchEnd(action);
                    return;
                }
                if (!m_CellsSearched.Contains(neighbor) && !m_CellsToSearch.Contains(neighbor))
                {
                    m_CellsToSearch.Add(neighbor);

                    if(needDemonstration)
                        neighbor.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }
            m_CellsToSearch.Remove(searchingCell);
            m_CellsSearched.Add(searchingCell);
            if (m_CellsToSearch.Count == 0)
                break;
            searchingCell = m_CellsToSearch[0];
            m_CellsToSearch.RemoveAt(0);
        }
    }
    #endregion

    #region AStar
    private void AStarSearch(int startCellId, int endCellId, Action<List<int>> action = null)
    {
        if (!CheckCanSearch(startCellId, endCellId))
            return;

        BaseCell searchingCell = OnSearchStart(startCellId);

        if (searchingCell == null)
            return;

        bool keepSearching = true;
        while (keepSearching)
        {
            if (searchingCell.ID == endCellId)
                keepSearching = false;
            m_CellsToSearch.Remove(searchingCell);
            m_CellsSearched.Add(searchingCell);

            foreach (BaseCell neighbor in searchingCell.neighbors)
            {
                if (neighbor == null || neighbor.block)
                    continue;
                if (m_CellsToSearch.Contains(neighbor) || m_CellsSearched.Contains(neighbor))
                    continue;
                m_CellsToSearch.Add(neighbor);
                neighbor.SetParent(searchingCell.ID);
                if (neighbor.ID == endCellId)
                {
                    // 走到了
                    m_Path.Add(neighbor.ID);
                    if (needDemonstration)
                        neighbor.GetComponent<SpriteRenderer>().color = Color.blue;

                    BaseCell traceNeighbor = neighbor;
                    int i = 0;
                    while (CellManager.Instance.GetCellByID(traceNeighbor.parentID) != null && i < 100)
                    {
                        traceNeighbor = CellManager.Instance.GetCellByID(traceNeighbor.parentID);

                        if(needDemonstration)
                            traceNeighbor.GetComponent<SpriteRenderer>().color = Color.blue;

                        m_Path.Add(traceNeighbor.ID);
                        i++;
                    }

                    m_Path.Reverse();

                    OnSearchEnd(action);

                    return;
                }
            }

            BaseCell newSearchingCell = null;
            int minF = -1;
            foreach (var cell in m_CellsToSearch)
            {
                int f = CalF(endCellId, cell.ID);
                if (f < 0)
                {
                    Debug.Log("这货距离小于零啊，有问题");
                    continue;
                }
                else if (f <= minF || minF < 0)
                {
                    minF = f;
                    newSearchingCell = cell;
                }
            }

            if (m_CellsToSearch.Count == 0)
            {
                Debug.Log("Open列表为空，搜索结束");
                break;
            }
            if (newSearchingCell == null)
            {
                Debug.Log("找不到下一个搜索节点，搜索结束");
                break;
            }

            if(needDemonstration)
                newSearchingCell.GetComponent<SpriteRenderer>().color = Color.yellow;
            searchingCell = newSearchingCell;
        }
    }

    private int CalF(int targetCellID, int presentCellID) {
        BaseCell presentCell = CellManager.Instance.GetCellByID(presentCellID);
        if (presentCell == null)
        {
            Debug.LogError("当前网格ID不合法");
            return -1;
        }
        int g = presentCell.CalG();
        int h = CalH(targetCellID, presentCellID);
        if (h == -1)
            return -1;

        return g + h;
    }

    private int CalH(int targetCellID, int presentCellID) {

        BaseCell endPos = CellManager.Instance.GetCellByID(targetCellID);
        BaseCell nowPos = CellManager.Instance.GetCellByID(presentCellID);
        if (endPos == null)
        {
            Debug.LogError("终点网格ID不合法");
            return -1;
        }

        switch (CellManager.Instance.cellShape)
        {
            case CellShape.Hexagon:
                return Mathf.Abs(endPos.coordinates.GetX() - nowPos.coordinates.GetX()) +
                       //Mathf.Abs(endPos.coordinates.GetY() - nowPos.coordinates.GetY()) +
                       Mathf.Abs(endPos.coordinates.GetZ() - nowPos.coordinates.GetZ());
            case CellShape.Square:
                return Mathf.Abs(endPos.coordinates.GetX() - nowPos.coordinates.GetX()) +
                       Mathf.Abs(endPos.coordinates.GetZ() - nowPos.coordinates.GetZ());
            case CellShape.SquareWithWall:
                return Mathf.Abs(endPos.coordinates.GetX() - nowPos.coordinates.GetX()) +
                       Mathf.Abs(endPos.coordinates.GetZ() - nowPos.coordinates.GetZ());
            default:
                return -1;
        }
    }

    #endregion

    #region BeamSearch
    private void BeamSearch(int startCellId, int endCellId, Action<List<int>> action = null)
    {
        if (!CheckCanSearch(startCellId, endCellId))
            return;

        BaseCell searchingCell = OnSearchStart(startCellId);

        if (searchingCell == null)
            return;

        int openSize = m_BeamWidth;
        if (openSize <= 0)
            openSize = 100;

        bool keepSearching = true;
        while (keepSearching)
        {
            if (searchingCell.ID == endCellId)
                keepSearching = false;
            m_CellsToSearch.Remove(searchingCell);
            m_CellsSearched.Add(searchingCell);
            foreach (BaseCell neighbor in searchingCell.neighbors)
            {
                if (neighbor == null || neighbor.block)
                    continue;
                if (m_CellsSearched.Contains(neighbor) || m_CellsToSearch.Contains(neighbor))
                    continue;

                neighbor.SetParent(searchingCell.ID);
                neighbor.beamFVal = CalF(endCellId, neighbor.ID);

                m_CellsToSearch.Add(neighbor);
                QuicksortListByFVal(ref m_CellsToSearch, 0, m_CellsToSearch.Count - 1);

                while (m_CellsToSearch.Count > openSize)
                {
                    if(needDemonstration)
                        m_CellsToSearch[m_CellsToSearch.Count - 1].GetComponent<SpriteRenderer>().color = Color.grey;
                    m_CellsToSearchBackup.Add(m_CellsToSearch[m_CellsToSearch.Count - 1]);
                    m_CellsToSearch.RemoveAt(m_CellsToSearch.Count - 1);
                }

                if (neighbor.ID == endCellId)
                {
                    // 走到了
                    m_Path.Add(neighbor.ID);

                    if (needDemonstration)
                        neighbor.GetComponent<SpriteRenderer>().color = Color.blue;

                    BaseCell traceNeighbor = neighbor;
                    int i = 0;
                    while (CellManager.Instance.GetCellByID(traceNeighbor.parentID) != null && i < 100)
                    {
                        traceNeighbor = CellManager.Instance.GetCellByID(traceNeighbor.parentID);

                        m_Path.Add(traceNeighbor.ID);

                        if (needDemonstration)
                            traceNeighbor.GetComponent<SpriteRenderer>().color = Color.blue;
                        i++;
                    }

                    m_Path.Reverse();
                    OnSearchEnd(action);

                    return;
                }
            }

            // 进入死胡同，找回剪枝剪掉的节点
            if (m_CellsToSearch.Count == 0)
            {
                while (true)
                {
                    if (m_CellsToSearchBackup.Count > 0)
                    {
                        if (m_CellsSearched.Contains(m_CellsToSearchBackup[0]))
                            m_CellsToSearchBackup.RemoveAt(0);
                        else
                            break;
                    }
                }
                if (m_CellsToSearchBackup.Count > 0)
                {
                    QuicksortListByFVal(ref m_CellsToSearchBackup, 0, m_CellsToSearchBackup.Count - 1);
                    Debug.Log("Remove Backup: " + m_CellsToSearchBackup[0].ID);

                    if(needDemonstration)
                        m_CellsToSearchBackup[0].GetComponent<SpriteRenderer>().color = Color.magenta;
                    m_CellsToSearch.Add(m_CellsToSearchBackup[0]);
                    m_CellsToSearchBackup.RemoveAt(0);
                }
                else
                {
                    Debug.Log("Open列表为空，搜索结束");
                    break;
                }
            }
            searchingCell = m_CellsToSearch[0];
            if (searchingCell == null)
            {
                Debug.Log("找不到下一个搜索节点，搜索结束");
                break;
            }

            if(needDemonstration)
                searchingCell.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public void SetBeamWidth(int width) {
        m_BeamWidth = width;
    }

    private static void QuicksortListByFVal(ref List<BaseCell> a, int low, int high) {
        if (low >= high)
        {
            return;
        }

        int first = low, last = high;
        //此时a[low]被保存到key，所以元素a[low]可以当作是一个空位，用于保存数据，之后每赋值一次，也会有一个位置空出来，直到last==first，此时a[last]==a[first]=key

        int key = a[low].beamFVal;
        BaseCell lowCell = a[low];
        while (first < last)
        {
            while (first < last && a[last].beamFVal >= key)
            {
                last--;
            }
            a[first] = a[last];
            while (first < last && a[first].beamFVal <= key)
            {
                first++;
            }
            a[last] = a[first];
        }

        a[first] = lowCell;

        //递归排序数组左边的元素
        QuicksortListByFVal(ref a, low, first - 1);

        //递归排序右边的元素
        QuicksortListByFVal(ref a, first + 1, high);
    }

    #endregion
}
