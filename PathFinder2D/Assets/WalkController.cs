using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WalkState
{
    StartFind,
    OnFind,
    Walk,
    Stop,
}

public class WalkController : EntityBaseController
{
    public float ArriveDistance;

    public float MoveSpeed { get { return m_MoveSpeed; } }
    private float m_MoveSpeed;

    public WalkState WalkingState { get { return m_WalkingState; } }
    private WalkState m_WalkingState;

    private Vector2 m_EndPosition;

    private List<int> m_Path = new List<int>();

    protected override void OnStart()
    {
        base.OnStart();

        m_MoveSpeed = 0.15f;
        MessageManager.Instance.Register(Event.ClickOnCell, ClickOnCell);
        InitAnimComponent();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_WalkingState == WalkState.OnFind || m_WalkingState == WalkState.Walk)
        {
            DoWalk();
        }
        else
        {
            SetVelocity(Vector2.zero);
        }
    }


    public void DoWalk()
    {
        SetWalkState(WalkState.Walk);

        if (m_Path == null)
        {
            StopWalk();
            return;
        }

        int idx = m_Path.IndexOf(m_CurrentCellId);

        for (int i = idx; i >= 0; i--)
        {
            m_Path.RemoveAt(i);
        }

        Vector3 targetPos;

        if (m_Path.Count == 0)
        {
            BaseCell endPointCell = MapManager.Instance.GridSystem.GetCellByPos(CellShape.SquareWithWall, m_EndPosition);

            if (endPointCell.ID == CurrentCellId && ((Vector2)GetHostGameObject().transform.position - m_EndPosition).magnitude > m_StopRadius)
            {
                targetPos = m_EndPosition;
            }
            else
            {
                StopWalk();
                return;
            }
        }
        else
        {
            targetPos = CellManager.Instance.GetCellByID(m_Path[0]).transform.position;
        }

        var moveDir = (targetPos - transform.position).normalized * m_MoveSpeed;
        SetVelocity(new Vector2(moveDir.x, moveDir.y));

        transform.Translate(moveDir);
    }

    private void StopWalk()
    {
        m_Path?.Clear();
        SetWalkState(WalkState.Stop);
        SetVelocity(Vector2.zero);
    }

    private void DoFindPath(int targetCellId, Vector2 clickPoint)
    {
        SetEndPosition(clickPoint);
        
        if (targetCellId == CurrentCellId)
        {
            SetWalkState(WalkState.OnFind);
            return;
        }

        SetWalkState(WalkState.StartFind);
        MapManager.Instance.PathFinder.FindPath(CurrentCellId, targetCellId, PathFindAlg.Astar, SetPath);
    }

    private void SetEndPosition(Vector2 endPoint)
    {
        m_EndPosition = endPoint;
    }

    private void SetPath(List<int> path)
    {
        m_Path.Clear();
        m_Path.AddRange(path);

        SetWalkState(WalkState.OnFind);
    }

    public void ClickOnCell(params object[] param)
    {
        if (param == null || param.Length < 2)
            return;

        int cellId = (int)param[0];
        Vector2 clickPoint = (Vector2)param[1];

        DoFindPath(cellId, clickPoint);
    }

    private void SetWalkState(WalkState state)
    {
        m_WalkingState = state;
    }
    
}
