using Com.IsartDigital.F2P.Biomes;

using System;
using System.Collections.Generic;

using UnityEngine;

public class CardPrefabDic : MonoBehaviour
{
    [SerializeField] CTGODic _Biomes;

    private static Dictionary<BiomeType, Card> prefabDic;
    private static List<Card> prefabList;

    private void Awake()
    {
        prefabDic = _Biomes.ToDic();
        prefabList = _Biomes.ToList();
    }

    public static GameObject GetRandomPrefab() 
    {
        return prefabList[UnityEngine.Random.Range(0, prefabList.Count)].GO;

        
    }

    public static GameObject GetPrefab(BiomeType type)
    {
        return prefabDic[type].GO;
    }
}

[Serializable]
public class CTGODic
{
    [SerializeField] CTGOItem[] _Dict;

    public Dictionary<BiomeType, Card> ToDic()
    {
        Dictionary<BiomeType, Card> newDic = new Dictionary<BiomeType, Card>();

        foreach (CTGOItem item in _Dict)
        {
            newDic.Add(item.key, item.value);
        }

        return newDic;
    }

    public List<Card> ToList()
    {
        List<Card> newList = new List<Card>();

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
    public Card value;
}

[Serializable]
public struct Card
{
    public GameObject GO;
    public float chanceToSpawn;
}
