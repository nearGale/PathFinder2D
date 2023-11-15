using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PrefabType
{
    General,
    Soilder,
}

public class ObjectManager : MonoSingleton<ObjectManager>
{
    [System.Serializable]
    public struct PrefabPair
    {
        public PrefabType type;
        public GameObject prefab;
    }

    public PrefabPair[] PrefabArray;
    private Dictionary<PrefabType, GameObject> m_PrefabDict;

    void Start()
    {
        m_PrefabDict = new Dictionary<PrefabType, GameObject>();
        for (int i = 0; i < PrefabArray.Length; i++)
        {
            if (!m_PrefabDict.ContainsKey(PrefabArray[i].type))
            {
                m_PrefabDict.Add(PrefabArray[i].type, PrefabArray[i].prefab);
            }
        }
    }

    public GameObject Instantiate(PrefabType type)
    {
        GameObject prefabObj;
        if(m_PrefabDict.TryGetValue(type, out prefabObj))
        {
            return GameObject.Instantiate(prefabObj);
        }
        return null;
    }

    public GameObject Instantiate(PrefabType type, Transform parent, bool worldPositionStays)
    {
        GameObject prefabObj;
        if (m_PrefabDict.TryGetValue(type, out prefabObj))
        {
            return GameObject.Instantiate(prefabObj, parent, worldPositionStays);
        }
        return null;
    }
}
