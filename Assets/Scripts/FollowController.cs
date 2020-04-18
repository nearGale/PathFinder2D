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

    /// <summary>
    /// 本次移动方向
    /// </summary>
    private Vector2 m_CurrentVelocity;
    
    /// <summary>
    /// 本次 wander 的角度
    /// </summary>
    private float m_CurrentWanderAngle;

    /// <summary>
    /// wander 转向最大角度
    /// </summary>
    private float m_AngleChange_Max;

    /// <summary>
    /// 最大转向力（用以平滑转向）
    /// </summary>
    private float m_ForceSteer_Max;

    /// <summary>
    /// 本物体的质量
    /// </summary>
    private float m_TempMass;

    /// <summary>
    /// 距离目的地 停下的距离
    /// </summary>
    public float m_StopRadius;

    /// <summary>
    /// 距离目的地 开始减速的距离
    /// </summary>
    private float m_SlowingRadius;

    /// <summary>
    /// 漫步圆的距离
    /// </summary>
    private float m_CircleDistance;

    /// <summary>
    /// 漫步圆的大小
    /// </summary>
    private float m_CircleRadius;

    protected override void OnStart()
    {
        base.OnStart();
        if(FollowTarget!= null){
            followTargetController = FollowTarget.GetComponent<SceneObjController>();
        }
        m_MoveSpeed_Max = 0.1f;
        m_ForceSteer_Max = 1f;
        m_AngleChange_Max = 50;

        m_CircleDistance = 0.3f;
        m_CircleRadius = 0.1f;

        m_StopRadius = 0.5f;
        m_SlowingRadius = 1f;

        m_CurrentVelocity = Vector2.zero;
        m_TempMass = 1;
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

            Vector2 desiredVelocity = CalDesiredVelocity();
            Vector2 finalVelocity = CalDesiredVelocity();

            // Step 1. 平滑转向

            //Vector2 steeringVector = CalSteeringVector(desiredVelocity, m_CurrentVelocity);

            //Vector2 finalVelocity = AddSteering(m_CurrentVelocity, steeringVector);


            // Step 2. 随机漫步

            //Vector2 wanderVector = CalWanderVector(finalVelocity);

            //finalVelocity = AddSteering(finalVelocity, wanderVector);

            //
            
            transform.Translate(finalVelocity);

            m_CurrentVelocity = finalVelocity;

            if (finalVelocity.x == 0 && finalVelocity.y == 0)
            {
                Debug.Log($"m_CurrentCellId: {m_CurrentCellId}           path: {Logger.ListToString(m_Path)}");
            }

            if (finalVelocity.magnitude == 0 && m_Path != null && m_Path.Count != 0)
                Debug.LogError("11");
            Debug.Log($"finalVelocity: {finalVelocity}     transform:{transform.position}");
        }
    }

    private void CalFollowPath()
    {
        Debug.Log($"FindPathRequest - m_CurrentCellId: {m_CurrentCellId}");
        MapManager.Instance.PathFinder.FindPathRequest(CurrentCellId, followTargetController.CurrentCellId, PathFindAlg.Astar, SetPath);
    }

    private Vector2 CalDesiredVelocity()
    {
        int idx = m_Path.IndexOf(m_CurrentCellId);

        if(m_Path.Count <= idx + 1)
        {
            return Vector2.zero;
        }

        var targetPos = CellManager.Instance.GetCellByID(m_Path[idx + 1]).transform.position;

        Vector2 desiredVelocity = targetPos - transform.position;

        if(desiredVelocity.magnitude < m_SlowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * m_MoveSpeed_Max * ((desiredVelocity.magnitude - m_SlowingRadius) / (m_SlowingRadius - m_SlowingRadius));
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * m_MoveSpeed_Max;
        }

        return desiredVelocity;
    }

    private Vector2 CalSteeringVector(Vector2 desiredVelocity, Vector2 currentVelocity)
    {
        Vector2 steeringForce = desiredVelocity - currentVelocity;
        
        VectorHandler.Truncate(ref steeringForce, m_ForceSteer_Max);

        Vector2 steeringVector = steeringForce / m_TempMass;

        return steeringVector;
    }

    private Vector2 AddSteering(Vector2 moveVector, Vector2 steeringVector)
    {
        Vector2 moveDir = moveVector + steeringVector;

        VectorHandler.Truncate(ref moveDir, m_MoveSpeed_Max);

        return moveDir;
    }

    private Vector2 CalWanderVector(Vector2 moveDir)
    {
        Vector2 circleCenter = moveDir.normalized * m_CircleDistance;

        Vector2 displacement = Vector2.down * m_CircleRadius;

        // 通过修改当前角度，随机改变向量的方向，m_CurrentWanderAngle 初始为 0，默认向上
        SetAngle(ref displacement, m_CurrentWanderAngle);

        m_CurrentWanderAngle += (Random.Range(0f, 1f) * m_AngleChange_Max) - (m_AngleChange_Max * 0.5f);

        Vector2 wanderForce = circleCenter + displacement;

        VectorHandler.Truncate(ref wanderForce, m_ForceSteer_Max);

        Vector2 wanderVector = wanderForce / m_TempMass;

        return wanderVector;
    }

    private void SetAngle(ref Vector2 vector, float value)
    {
        float len = vector.magnitude;
        vector.x = Mathf.Cos(value) * len;
        vector.y = Mathf.Sin(value) * len;
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
        Debug.Log($"SetPath - m_CurrentCellId: {m_CurrentCellId}          path: {Logger.ListToString(path)}");

        if (!path.Contains(m_CurrentCellId))
            return;

        m_Path.Clear();
        m_Path.AddRange(path);
    }
}
