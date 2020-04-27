using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjController : MonoBehaviour
{
    public int CurrentCellId { get { return m_CurrentCellId; } }
    protected int m_CurrentCellId;


    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        UpdateSelfPos();
    }

    void Update()
    {
        //Debug.Log($"== SceneObjController Update ==");
        UpdateSelfPos();
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {

    }

    protected void UpdateSelfPos()
    {
        CellShape shape = CellManager.Instance.cellShape;
        Coordinates coordinates;

        if (shape == CellShape.SquareWithWall)
        {
            Transform mapTrans = MapManager.Instance.GridSystem.transform;
            if (mapTrans == null)
                return;

            var position = mapTrans.InverseTransformPoint(transform.position);
            coordinates = Coordinates.SquareCoordinatesFromPosition(position, MapManager.Instance.GridSize);

            m_CurrentCellId = CellManager.Instance.GetCellByCoordinates(coordinates.GetX(), coordinates.GetZ()).ID;
        }

    }

    public void SetPos(int currentCellId)
    {
        m_CurrentCellId = currentCellId;
    }
}
