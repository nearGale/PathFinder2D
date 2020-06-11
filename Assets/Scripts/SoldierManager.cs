using System.Collections;
using System.Collections.Generic;
using Application;
using UnityEngine;

public class SoldierManager : MonoSingleton<SoldierManager>
{
    public enum EFollowType {
        Follow,
        Pursuit,
        Queue,
    }

    public int NumOfSoldiers;
    public EFollowType followType;
    public Object SoldierResource;
    public Transform FollowTarget;
    
    public float PosInterval;
    public int NumPerLine;

    private List<GameObject> m_SoldierList = new List<GameObject>();
    private string Enemy_Resource_Path = "Enemy";

    public void CreateSoldiers()
    {
        GameObject enemy = Instantiate(Resources.Load(Enemy_Resource_Path, typeof(GameObject)), transform) as GameObject;
        enemy.transform.position = new Vector2(Random.Range(0, 20), Random.Range(0, 20));
        for (int i = 0; i < NumOfSoldiers; i++){
            GameObject soldier = Instantiate(SoldierResource, transform) as GameObject;
            soldier.transform.Translate(new Vector2(i % NumPerLine, i / NumPerLine) * PosInterval);

            soldier.GetComponent<FollowController>().SetFollowTarget(FollowTarget);
            soldier.GetComponent<FollowController>().SetPosition(soldier.transform.position);
            
            m_SoldierList.Add(soldier);
            if (i == 0)
            {
                // temp: pursuit the first soldier
                var soldierCtrl = soldier.GetComponent<FollowController>();
                if (soldierCtrl && soldierCtrl is IEntity)
                {
                    var enemyCtrl =enemy.GetComponent<EnemyController>();
                    if (enemyCtrl != null)
                    {
                        enemyCtrl.FollowTarget = soldierCtrl;
                    }
                }
            }
        }
        

    }

    public void CreateSoldier()
    {
        GameObject soldier = Instantiate(SoldierResource, transform) as GameObject;
        int i = Random.Range(0, 20);
        soldier.transform.Translate(new Vector2(i % NumPerLine, i / NumPerLine) * PosInterval);

        soldier.GetComponent<FollowController>().SetFollowTarget(FollowTarget);
        soldier.GetComponent<FollowController>().SetPosition(soldier.transform.position);

        m_SoldierList.Add(soldier);
    }

}
