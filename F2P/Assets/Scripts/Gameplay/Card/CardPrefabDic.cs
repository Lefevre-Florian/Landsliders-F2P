using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.Biomes;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPrefabDic : MonoBehaviour
{
    [SerializeField] private CTGODic _Biomes;
    [SerializeField] private bool _IsForced = false;

    private static Dictionary<BiomeType, Card> prefabDic;
    private static List<Card> prefabList;

    private static float currentLevel = 0;

    private void Awake()
    {
        if(_IsForced || Save.data == null)
        {
            prefabDic = _Biomes.ToDic();
            prefabList = _Biomes.ToList();
        }
        else
        {
            prefabDic = _Biomes.ToDic();
            prefabList = _Biomes.ToList();

            int lLength = Save.data.cardPrefabs.Length;
            List<BiomeType> lKeys = new List<BiomeType>();
            for (int i = 0; i < lLength; i++)
                lKeys.Add(Save.data.cardPrefabs[i].GetComponent<Biome>().Type);

            Card lCard;
            for (int i = 0; i < lLength; i++)
            {
                lCard = prefabDic[lKeys[i]];
                lCard.GO = Save.data.cardPrefabs[i];
                prefabDic[lKeys[i]] = lCard;
            }

            prefabDic.ToList().RemoveAll(x => !lKeys.Contains(x.Key));

            for (int i = 0; i < lLength; i++)
                prefabList[prefabList.FindIndex(x => x.GO.GetComponent<Biome>().Type == lKeys[i])] = prefabDic[lKeys[i]];
        }
    }

    public static GameObject GetRandomPrefab() 
    {
        float lTotalWeight = GetTotalWeight();

        float lRand = UnityEngine.Random.Range(0, lTotalWeight);

        float lCurrentProp = 0;

        for (int i = 0; i < prefabList.Count; i++)
        {
            lCurrentProp += prefabList[i].chanceToSpawn.Evaluate(currentLevel);
            if (lRand < lCurrentProp) return prefabList[i].GO;
        }

        return null;
    }

    public static GameObject GetPrefab(BiomeType type)
    {
        return prefabDic[type].GO;
    }

    private static float GetTotalWeight()
    {
        float ret = 0;
        foreach (Card pCard in prefabList)
            ret += pCard.chanceToSpawn.Evaluate(currentLevel);

        return ret;
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
    public AnimationCurve chanceToSpawn;
}
