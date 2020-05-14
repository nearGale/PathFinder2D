using System.Collections;
using System.Collections.Generic;
using Application;
using UnityEngine;

public class EnemyController : EntityBaseController
{
    public IEntity FollowTarget;
    

    protected override void OnStart()
    {
        base.OnStart();
        if(FollowTarget!= null){
            followTargetController = FollowTarget.GetHostGameObject().GetComponent<SceneObjController>();
        }
        m_MoveSpeed_Max = 0.1f;
        m_ForceSteer_Max = 1f;
        
        m_StopRadius = 0.5f;
        m_SlowingRadius = 1f;

        m_CurrentVelocity = Vector2.zero;
        m_TempMass = 1;
        m_Steering = new SteeringManager(this);
        m_Position = new Vector2(transform.position.x, transform.position.y);
        
        InitAnimComponent();
    }



    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKey(KeyCode.G))
        {

            if (!CheckNeedFollow() )
            {
                return;
            }
            
            if (FollowTarget != null)
            {
                m_Steering.Pursuit(FollowTarget);
                m_Steering.CollisionAvoidance();
                m_Steering.Update();
                transform.position = m_Position;

            }
      
        }
    }

    
    private bool CheckNeedFollow()
    {
        bool needFollow = false;

        if (FollowTarget != null && followTargetController != null)
        {
            if (followTargetController.CurrentCellId == m_CurrentCellId)
            {
                var curPos = new Vector2(transform.position.x, transform.position.y);
                if ((FollowTarget.GetPosition() - curPos).magnitude > m_StopRadius)
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
    
}
