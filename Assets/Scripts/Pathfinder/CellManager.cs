using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CellManager : Singleton<CellManager> {

    public CellShape cellShape;

    public List<BaseCell> cells { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();

        cells = new List<BaseCell>();
    }

    public void Update()
    {
        RefreshCellG();
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

        if (id<0 || id > cells.Count -1)
            return null;
        return cells[id];
    }

    public void RefreshCellG()
    {
        List<SceneObjController> characters = SoldierManager.Instance.GetCharacters();
        if(characters == null || characters.Count == 0)
        {
            return;
        }

        foreach(var cell in cells)
        {
            cell.SetPassableDifficulty(0);
        }

        foreach (var character in characters)
        {
            if (character == null)
                continue;

            BaseCell cell = GetCellByID(character.CurrentCellId);
            if (cell == null)
                continue;

            cell.SetPassableDifficulty(cell.PassableDifficulty + GameBlackBoard.CELL_PASSABLE_DIFFICULTY_ADD_PER_CHARACTER);
        }
    }
    
}
