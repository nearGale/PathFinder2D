using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Application;
//防止随机移动的时候超过了领队
public class EvadeController : MonoBehaviour
{
    public IEntity leader;
    private IEntity pawn;
    private Vector2 leaderAhead;
    private float LEADER_BEHIND_DIST;
    private Vector2 dist;
    public float evadeDistance;
    private float sqrEvadeDistance;


    void Start()
    {
        pawn = GetComponent<IEntity>();
        LEADER_BEHIND_DIST = 2.0f;
        sqrEvadeDistance = evadeDistance * evadeDistance;
    }


    void Update()
    {
        //计算领队前方的一个点
        leaderAhead = leader.GetPosition() + leader.GetVelocity().normalized * LEADER_BEHIND_DIST;
        //计算角色当前位置与领队前方某店的距离，小于某个值，就需要躲避
        dist = pawn.GetPosition() - leaderAhead;
       
        if (dist.sqrMagnitude < sqrEvadeDistance)
        {
            //小于躲避距离，激活躲避行为

        }
        else
        {
            //躲避行为不用激活
        }
    }
}
