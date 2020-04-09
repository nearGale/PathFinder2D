using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : SceneObjController
{
    public Transform FollowTarget;
    private SceneObjController followTargetController;

    public float FollowDistance;

    public float MoveSpeed { get { return m_MoveSpeed; } }
    private float m_MoveSpeed;

    private List<int> m_Path = new List<int>();
    private WalkState m_State;

    protected override void OnStart()
    {
        base.OnStart();

        followTargetController = FollowTarget.GetComponent<SceneObjController>();

        m_MoveSpeed = 0.2f;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Input.GetKey(KeyCode.G))
        {
            DoFindPath();
            DoFollow();
        }
    }

    public void DoFollow()
    {
        bool needFollow = CheckNeedFollow();

        if (!needFollow)
        {
            return;
        }

        if(m_Path == null)
        {
            return;
        }

        int idx = m_Path.IndexOf(m_CurrentCellId);

        for(int i = idx; i >= 0; i--)
        {
            m_Path.RemoveAt(i);
        }

        if(m_Path.Count == 0)
        {
            return;
        }

        var targetPos = CellManager.Instance.GetCellByID(m_Path[0]).transform.position;

        var moveDir = (targetPos - transform.position).normalized * m_MoveSpeed;

        transform.Translate(moveDir);
    }

    private bool CheckNeedFollow()
    {
        bool needFollow = false;

        if (FollowTarget != null && followTargetController != null)
        {
            if (followTargetController.CurrentCellId == m_CurrentCellId)
            {
                if ((FollowTarget.position - transform.position).magnitude > FollowDistance)
                {
                    needFollow = true;
                }
            }
            else
            {
                needFollow = true;
            }
        }

        return needFollow;
    }

    private void DoFindPath()
    {
        MapManager.Instance.PathFinder.FindPathRequest(CurrentCellId, followTargetController.CurrentCellId, PathFindAlg.Astar, SetPath);
    }

    private void SetPath(List<int> path)
    {
        Debug.Log("SetPath F   " + Logger.ListToString(path) + "   " + path.Count);

        m_Path.Clear();
        m_Path.AddRange(path);
    }
}
