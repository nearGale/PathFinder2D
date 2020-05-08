using System.Collections;
using System.Collections.Generic;
using Application;
using UnityEngine;

public class EntityBaseController : SceneObjController, IEntity
{
    public IEntity FollowTarget;
    protected SceneObjController followTargetController;

    protected float m_MoveSpeed_Max;
    
    /// <summary>
    /// 本次移动方向
    /// </summary>
    protected Vector2 m_CurrentVelocity;
    
    /// <summary>
    /// 本次 wander 的角度
    /// </summary>
    protected float m_CurrentWanderAngle;
    
    /// <summary>
    /// 最大转向力（用以平滑转向）
    /// </summary>
    protected float m_ForceSteer_Max;

    /// <summary>
    /// 本物体的质量
    /// </summary>
    protected float m_TempMass;

    /// <summary>
    /// 距离目的地 停下的距离
    /// </summary>
    public float m_StopRadius;

    /// <summary>
    /// 距离目的地 开始减速的距离
    /// </summary>
    protected float m_SlowingRadius;

    protected Vector2 m_Position;
    protected SteeringManager m_Steering;
    
    
#region IEntity Interface
    public Vector2 GetVelocity()
    {
        return m_CurrentVelocity;
    }

    public float GetMaxVelocity()
    {
        return m_MoveSpeed_Max;
    }

    public Vector2 GetPosition()
    {
        return m_Position;
    }

    public float GetMass()
    {
        return m_TempMass;
    }

    public void SetPosition(Vector2 pos)
    {
        m_Position = pos;
    }

    public void SetVelocity(Vector2 velocity)
    {
        m_CurrentVelocity = velocity;
    }
    public float GetSlowingRadius()
    {
        return m_SlowingRadius;
    }
    public GameObject GetHostGameObject()
    {
        return gameObject;
    }
    #endregion
}
