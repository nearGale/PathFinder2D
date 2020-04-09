using System;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : Singleton<CellManager> {

    public CellShape cellShape;

    public List<BaseCell> cells { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();

        cells = new List<BaseCell>();
    }

    public void AddCell(BaseCell cell) {
        cells.Add(cell);
    }

    public BaseCell GetCellByID(int id) {
        if (id < 0 || id >= cells.Count)
            return null;
        return cells[id];
    }

    public int GetIdByCoordinates(int x, int y)
    {
        int id = x + MapManager.Instance.MapWidth * y;
        return id;
    }

    public BaseCell GetCellByCoordinates(int x, int y)
    {
        int id = GetIdByCoordinates(x, y);

        if (id > cells.Count)
            return null;

        return cells[id];
    }
}
