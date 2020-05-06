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

    public float PosInterval;
    public int NumPerLine;

    private List<GameObject> m_SoldierList = new List<GameObject>();
    
    private void Start()
    {
        for (int i = 0; i < NumOfSoldiers; i++){
            GameObject soldier = Instantiate(SoldierResource, transform) as GameObject;
            soldier.transform.Translate(new Vector2(i % NumPerLine, i / NumPerLine) * PosInterval);

            soldier.GetComponent<FollowController>().SetFollowTarget(FollowTarget);
            soldier.GetComponent<FollowController>().SetPosition(soldier.transform.position);
            m_SoldierList.Add(soldier);
        }
    }

}
