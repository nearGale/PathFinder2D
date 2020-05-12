using System.Collections;
using System.Collections.Generic;
using Application;
using UnityEngine;

public class FollowController : EntityBaseController
{
    public Transform FollowTarget;
    
    private List<int> m_Path = new List<int>();
    private WalkState m_State;

    private bool m_PathfindingRequested;

    protected override void OnStart()
    {
        base.OnStart();
        if(FollowTarget!= null){
            followTargetController = FollowTarget.GetComponent<SceneObjController>();
        }
        m_MoveSpeed_Max = 0.1f;
        m_ForceSteer_Max = 1f;

        m_StopRadius = 0.5f;
        m_SlowingRadius = 1f;

        m_CurrentVelocity = Vector2.zero;
        m_TempMass = 1;
        m_Steering = new SteeringManager(this);

        m_PathfindingRequested = false;
    }

    public void SetFollowTarget(Transform followTarget){
        FollowTarget = followTarget;
        followTargetController = FollowTarget.GetComponent<SceneObjController>();
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
            

            // Step 1. 平滑转向

            //Vector2 steeringVector = CalSteeringVector(desiredVelocity, m_CurrentVelocity);

            //Vector2 finalVelocity = AddSteering(m_CurrentVelocity, steeringVector);


            // Step 2. 随机漫步

            //Vector2 wanderVector = CalWanderVector(finalVelocity);

            //finalVelocity = AddSteering(finalVelocity, wanderVector);

            //
            Vector2 targetPos = GetNextTargetPos();
//            m_Steering.Seek(targetPos);
            m_Steering.Wander();
            m_Steering.CollisionAvoidance();
            m_Steering.Update();
            transform.position = m_Position;
            
            
            if (m_CurrentVelocity.x == 0 && m_CurrentVelocity.y == 0)
            {
                Debug.Log($"m_CurrentCellId: {m_CurrentCellId}           path: {Logger.ListToString(m_Path)}");
            }

            if (m_Steering.Steering.magnitude == 0 && m_Path != null && m_Path.Count != 0)
                Debug.LogError("11");
//            Debug.Log($"m_CurrentVelocity: {m_Steering.Steering}     transform:{transform.position}");
        }
    }

    private void CalFollowPath()
    {
        if (followTargetController == null)
            return;

        if (m_PathfindingRequested)
            return;

//        Debug.Log($"FindPathRequest - m_CurrentCellId: {m_CurrentCellId}            m_PathfindingRequested： {m_PathfindingRequested}");
        m_PathfindingRequested = true;

        MapManager.Instance.PathFinder.FindPathRequest(CurrentCellId, followTargetController.CurrentCellId, PathFindAlg.Astar, SetPath);
    }

    private Vector2 GetNextTargetPos()
    {
        int idx = m_Path.IndexOf(m_CurrentCellId);

        if(m_Path.Count <= idx + 1)
        {
            return Vector2.zero;
        }

        var targetPos = CellManager.Instance.GetCellByID(m_Path[idx + 1]).transform.position;

        return targetPos;
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
//        Debug.Log($"SetPath - m_CurrentCellId: {m_CurrentCellId}          path: {Logger.ListToString(path)}");
        m_PathfindingRequested = false;

        if (path == null)
            return;

        if (!path.Contains(m_CurrentCellId))
            return;

        m_Path.Clear();
        m_Path.AddRange(path);
    }
    
}
