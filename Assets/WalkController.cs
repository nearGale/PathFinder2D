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

        if (m_Path.Count == 0)
        {
            StopWalk();
            return;
        }

        var targetPos = CellManager.Instance.GetCellByID(m_Path[0]).transform.position;

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

    private void DoFindPath(int targetCellId)
    {
        SetWalkState(WalkState.StartFind);
        MapManager.Instance.PathFinder.FindPath(CurrentCellId, targetCellId, PathFindAlg.Astar, SetPath);
    }

    private void SetPath(List<int> path)
    {
        m_Path.Clear();
        m_Path.AddRange(path);

        SetWalkState(WalkState.OnFind);
    }

    public void ClickOnCell(params object[] param)
    {
        if (param == null || param.Length < 1)
            return;

        int cellId = (int)param[0];
        DoFindPath(cellId);
    }

    private void SetWalkState(WalkState state)
    {
        m_WalkingState = state;
    }
    
}
