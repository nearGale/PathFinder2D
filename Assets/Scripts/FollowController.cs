using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : SceneObjController
{
    public Transform FollowTarget;
    private SceneObjController followTargetController;

    public float MoveSpeed_Max { get { return m_MoveSpeed_Max; } }
    private float m_MoveSpeed_Max;

    private List<int> m_Path = new List<int>();
    private WalkState m_State;

    private Vector2 m_CurrentVelocity;
    private Vector2 m_Steering;

    private float m_ForceSteer_Max;

    private float m_TempMass;

    public float m_StopRadius;
    private float m_SlowingRadius;

    protected override void OnStart()
    {
        base.OnStart();

        followTargetController = FollowTarget.GetComponent<SceneObjController>();

        m_MoveSpeed_Max = 0.5f;
        m_ForceSteer_Max = 1f;

        m_StopRadius = 0.5f;
        m_SlowingRadius = 1f;

        m_CurrentVelocity = Vector2.zero;
        m_TempMass = 1;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Input.GetKey(KeyCode.G))
        {
            CalFollowPath();

            if (!CheckNeedFollow() || m_Path == null)
            {
                return;
            }

            Vector2 desiredVelocity = CalDesiredVelocity();
            
            Vector2 finalVelocity = AddSteering(desiredVelocity, m_CurrentVelocity);
            
            transform.Translate(finalVelocity);
            
            m_CurrentVelocity = finalVelocity;

            Debug.Log($"finalVelocity: {finalVelocity}     transform:{transform.position}");
        }
    }

    private void CalFollowPath()
    {
        MapManager.Instance.PathFinder.FindPathRequest(CurrentCellId, followTargetController.CurrentCellId, PathFindAlg.Astar, SetPath);
    }

    private Vector2 CalDesiredVelocity()
    {
        int idx = m_Path.IndexOf(m_CurrentCellId);

        if(idx == -1)
        {
            return Vector2.zero;
        }

        for(int i = idx; i >= 0; i--)
        {
            m_Path.RemoveAt(i);
        }

        if(m_Path.Count == 0)
        {
            return Vector2.zero;
        }

        var targetPos = CellManager.Instance.GetCellByID(m_Path[0]).transform.position;

        Vector2 desiredVelocity = targetPos - transform.position;

        if(desiredVelocity.magnitude < m_SlowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * m_MoveSpeed_Max * ((desiredVelocity.magnitude - m_SlowingRadius) / (m_SlowingRadius - m_SlowingRadius));
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * m_MoveSpeed_Max;
        }

        return targetPos - transform.position;
    }

    private Vector2 AddSteering(Vector2 desiredVelocity, Vector2 currentVelocity)
    {
        Vector2 steeringForce = desiredVelocity - currentVelocity;
        
        VectorHandler.Truncate(ref steeringForce, m_ForceSteer_Max);

        Vector2 steeringVector = steeringForce / m_TempMass;



        Vector2 moveDir = m_CurrentVelocity + steeringVector;

        VectorHandler.Truncate(ref moveDir, m_MoveSpeed_Max);

        return moveDir;
    }

    private bool CheckNeedFollow()
    {
        bool needFollow = false;

        if (FollowTarget != null && followTargetController != null)
        {
            if (followTargetController.CurrentCellId == m_CurrentCellId)
            {
                if ((FollowTarget.position - transform.position).magnitude > m_StopRadius)
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

    private void SetPath(List<int> path)
    {
        if (!path.Contains(m_CurrentCellId))
            return;

        m_Path.Clear();
        m_Path.AddRange(path);
    }
}
