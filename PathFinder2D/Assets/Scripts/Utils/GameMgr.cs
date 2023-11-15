using System.Collections;
using System.Collections.Generic;

public enum GameState {
    Loading,
    Playing,
    Ending,
}

public enum ELayer
{
Default,
Wall = 9,
Entity =10,
}
public class GameMgr : Singleton<GameMgr> {
    public GameState state { get; private set; }

    // TODO: 改用状态机
    public void SetState(GameState newState) {
        state = newState;
    }

    protected override void OnInit() {
        base.OnInit();

        CellManager.Instance.Init();
        MapManager.Instance.Init();
        MessageManager.Instance.Init();
        SoldierManager.Instance.Init();
    }

    public void Start()
    {
        MapManager.Instance.GridSystem.GenerateMap();
        SoldierManager.Instance.CreateSoldiers();
    }

    public void Update()
    {
        MapManager.Instance.Update();
        CellManager.Instance.Update();
    }
}
