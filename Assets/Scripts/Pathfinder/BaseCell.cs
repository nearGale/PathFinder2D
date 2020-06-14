using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCell : MonoBehaviour {

    [SerializeField]
    public BaseCell[] neighbors;

    public Coordinates coordinates  {get;set;}

    public Sprite type;
    public int parentID { get; protected set; }
    public int ID { get; protected set; }
    //public int gVal { get; protected set; }
    public float beamFVal;
    public bool block { get; protected set; }

    public float PassableDifficulty = 1;

    public SpriteRenderer spriteRenderer { get; private set; }

    protected virtual void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentID = -1;
        //gVal = 0;
        block = false;
        beamFVal = -1;
    }

    public void SetID(int i) {
        ID = i;
    }

    public void SetIsBlock(bool isBlock) {
        block = isBlock;
    }

    public void SetParent(int parentCell) {
        parentID = parentCell;
    }
    public void ResetPassableDifficulty()
    {
        PassableDifficulty = 1;
    }

    public void SetPassableDifficulty(float difficulty)
    {
        PassableDifficulty = difficulty;
    }

    public virtual float CalG()
    {
        BaseCell parentCell = CellManager.Instance.GetCellByID(parentID);
        //if (parentCell == null)
        //{
        //    // 无父节点
        //    return 0;
        //}
        //gVal = parentCell.gVal + 1;
        //return gVal;

        if (parentCell == null)
        {
            // 无父节点
            return PassableDifficulty;
        }
        return parentCell.CalG() + PassableDifficulty;
    }
}
