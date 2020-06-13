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

    private List<EntityBaseController> m_CharacterList = new List<EntityBaseController>();
    private string Enemy_Resource_Path = "Enemy";

    public void CreateSoldiers()
    {
        CreateSoldier();
        CreateEnemy();
    }

    public void CreateSoldier()
    {
        GameObject soldier = Instantiate(SoldierResource, transform) as GameObject;
        soldier.transform.position = new Vector2(Random.Range(0, 20), Random.Range(0, 20));

        FollowController followController = soldier.GetComponent<FollowController>();

        if (followController == null)
            return;

        followController.SetFollowTarget(FollowTarget);
        followController.SetPosition(soldier.transform.position);

        m_CharacterList.Add(followController);
    }

    public void CreateEnemy()
    {
        GameObject enemy = Instantiate(Resources.Load(Enemy_Resource_Path, typeof(GameObject)), transform) as GameObject;
        enemy.transform.position = new Vector2(Random.Range(0, 20), Random.Range(0, 20));

        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyController == null)
            return;

        enemyController.SetPosition(enemy.transform.position);

        m_CharacterList.Add(enemyController);


        var enemyCtrl = enemy.GetComponent<EnemyController>();
        if (enemyCtrl != null)
        {
            enemyCtrl.FollowTarget = m_CharacterList.Find((item) => item is FollowController);
        }
    }

    public List<EntityBaseController> GetCharacters()
    {
        return m_CharacterList;
    }
}
