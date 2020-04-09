using System.Collections;
using System.Collections.Generic;

public enum GameState {
    Loading,
    Playing,
    Ending,
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
    }

    public void Update()
    {
        MapManager.Instance.Update();

    }
}
