using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public PathFinder PathFinder;

    public GridSystem GridSystem { get { return m_GridSystem; } }
    private GridSystem m_GridSystem;

    public int MapWidth { get { return m_MapWidth; } }
    private int m_MapWidth;

    public int MapHeight { get { return m_MapHeight; } }
    private int m_MapHeight;

    public float GridSize { get { return m_GridSize; } }
    private float m_GridSize;

    public CellShape Shape { get { return m_Shape; } }
    private CellShape m_Shape;

    public int PathFindNumPerFrame { get { return m_PathFindNumPerFrame; } }
    private int m_PathFindNumPerFrame;

    protected override void OnInit()
    {
        base.OnInit();
        PathFinder = new PathFinder();
        PathFinder.Init();

        m_GridSystem = GameObject.Find("GridSystem").GetComponent<GridSystem>();
        SetPathFindNumPerFrame(GameBlackBoard.MAP_PATHFIND_PER_FRAME);
    }

    public void Update()
    {
        PathFinder.ConductFindPath(m_PathFindNumPerFrame);
    }

    public void SetPathFindNumPerFrame(int num)
    {
        m_PathFindNumPerFrame = num;    
    }

    public void SetMapSize(int width, int height, float gridSize = 1)
    {
        m_MapWidth = width;
        m_MapHeight = height;
        m_GridSize = gridSize;
    }

    public void SetCellShape(CellShape shape)
    {
        m_Shape = shape;
    }

}
