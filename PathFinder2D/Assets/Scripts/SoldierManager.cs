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
    public Object GeneralResource;
    private Transform FollowTarget;
    
    public float PosInterval;
    public int NumPerLine;

    private List<EntityBaseController> m_CharacterList = new List<EntityBaseController>();
    private string Enemy_Resource_Path = "Enemy";

    public void CreateSoldiers()
    {
        CreateGeneral();
        CreateSoldier();
        CreateEnemy();
    }

    private void CreateGeneral()
    {
        GameObject general = Instantiate(GeneralResource, transform) as GameObject;
        general.transform.position = new Vector2(Random.Range(0, 20), Random.Range(0, 20));

        WalkController walkController = general.GetComponent<WalkController>();

        if (walkController == null)
            return;

        walkController.SetPosition(general.transform.position);

        m_CharacterList.Add(walkController);

        FollowTarget = general.transform;
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
            IEntity followTarget = m_CharacterList.Find((item) => item is FollowController);
            if (followTarget == null)
            {
                followTarget = m_CharacterList.Find((item) => item is WalkController);
            }
            enemyCtrl.SetFollowTarget(followTarget);
        }
    }

    public List<EntityBaseController> GetCharacters()
    {
        return m_CharacterList;
    }
}
