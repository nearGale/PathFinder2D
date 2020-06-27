using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Application;

public class CheckAroundEntityController : MonoBehaviour
{
    public IEntity Entity;
    //碰撞体的数组
    private Collider[] colliders;
    //计时器
    private float timer = 0;
    //邻居列表
    public List<GameObject> neighbors;
    //时间间隔
    public float checkInterval = 0.3f;
    //自身领域半径
    public float detectRadius = 10f;
    //设置检测哪一层的游戏对象 
    public LayerMask layersChecked;


    void Start()
    {
        //初始化邻居列表
        neighbors = new List<GameObject>();
    }


    void Update()
    {
        timer += Time.deltaTime;

        //如果上次检测的时间大于所设置的检测时间间隔，那么再次检测
        if (timer > checkInterval)
        {
            //清除邻居列表
            neighbors.Clear();
            //查找当前AI角色领域内的全部碰撞体
            colliders = Physics.OverlapSphere(transform.position, detectRadius);//, layersChecked);
                                                                                //对于每个检测到的碰撞体，加入邻居列表
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<IEntity>() != null)
                    neighbors.Add(colliders[i].gameObject);
            }

            timer = 0;
        }
    }
}
