using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierManager : MonoBehaviour
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


    private List<GameObject> m_SoldierList = new List<GameObject>();
    
    private void Start()
    {
        for (int i = 0; i < NumOfSoldiers; i++){
            GameObject soldier = Instantiate(SoldierResource, transform) as GameObject;
            soldier.GetComponent<FollowController>().SetFollowTarget(FollowTarget);
            m_SoldierList.Add(soldier);
        }
    }

}
