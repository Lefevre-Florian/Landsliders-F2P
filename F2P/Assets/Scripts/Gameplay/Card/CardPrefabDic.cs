using Com.IsartDigital.F2P;
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
        if(Save.data == null)
        {
            prefabDic = _Biomes.ToDic();
            prefabList = _Biomes.ToList();
        }
        else
        {
            prefabList = new List<GameObject>();
            prefabDic = new Dictionary<BiomeType, GameObject>();

            int lLength = Save.data.cardPrefabs.Length;
            for (int i = 0; i < lLength; i++)
                prefabList.Add(Save.data.cardPrefabs[i]);

            for (int i = 0; i < lLength; i++)
                prefabDic.Add(prefabList[i].GetComponent<Biome>().Type,
                              Save.data.cardPrefabs[i]);
        }
        
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
