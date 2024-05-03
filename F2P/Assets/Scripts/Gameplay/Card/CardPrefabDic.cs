using Com.IsartDigital.F2P.Biomes;

using System;
using System.Collections.Generic;

using UnityEngine;

public class CardPrefabDic : MonoBehaviour
{
    [SerializeField] CTGODic _Biomes;

    private static Dictionary<BiomeType, GameObject> prefabDic;
    private static List<GameObject> prefabList;

    private void Awake()
    {
        prefabDic = _Biomes.ToDic();
        prefabList = _Biomes.ToList();
    }

    public static GameObject GetRandomPrefab() 
    {
        return prefabList[UnityEngine.Random.Range(0, prefabList.Count)];   
    }

    public static GameObject GetPrefab(BiomeType type)
    {
        return prefabDic[type];
    }
}

[Serializable]
public class CTGODic
{
    [SerializeField] CTGOItem[] _Dict;

    public Dictionary<BiomeType, GameObject> ToDic()
    {
        Dictionary<BiomeType, GameObject> newDic = new Dictionary<BiomeType, GameObject>();

        foreach (CTGOItem item in _Dict)
        {
            newDic.Add(item.key, item.value);
        }

        return newDic;
    }

    public List<GameObject> ToList()
    {
        List<GameObject> newList = new List<GameObject>();

        foreach (CTGOItem item in _Dict)
        {
            newList.Add(item.value);
        }

        return newList;
    }
}

[Serializable]
public class CTGOItem
{
    [SerializeField]
    public BiomeType key;

    [SerializeField]
    public GameObject value;
}
