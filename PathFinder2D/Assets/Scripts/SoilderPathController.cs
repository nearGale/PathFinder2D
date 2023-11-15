using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilderPathController : SceneObjController
{
    private List<int> m_Path;

    void Start()
    {
    }

    void Update()
    {
        
    }

    private void MoveTo(int targetCellId)
    {
        MapManager.Instance.PathFinder.FindPath(m_CurrentCellId, targetCellId, PathFindAlg.Astar, SetPath);
    }

    private void SetPath(List<int> path)
    {
        m_Path = path;
    }
}
