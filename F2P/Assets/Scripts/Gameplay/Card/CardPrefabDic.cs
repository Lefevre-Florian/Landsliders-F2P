using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.Biomes;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPrefabDic : MonoBehaviour
{
    [SerializeField] private CTGODic _BiomesHandDic;
    [SerializeField] private CTGODic _BiomesMapDic;
    [SerializeField] private bool _IsForced = false;

    private static Dictionary<BiomeType, Card> prefabHandDic;
    private static Dictionary<BiomeType, Card> prefabMapDic;
    public static List<Card> prefabHandList;
    public static List<Card> prefabMapList;

    private static float currentLevel = 0;

    private void Awake()
    {
        prefabHandDic = _BiomesHandDic.ToDic();
        prefabHandList = _BiomesHandDic.ToList();
        prefabMapDic = _BiomesMapDic.ToDic();
        prefabMapList = _BiomesMapDic.ToList();

        if(!(_IsForced || Save.data == null))
        {

            int lLength = Save.data.cardPrefabs.Length;
            List<BiomeType> lKeys = new List<BiomeType>();
            for (int i = 0; i < lLength; i++)
                lKeys.Add(Save.data.cardPrefabs[i].GetComponent<Biome>().Type);

            Card lCard;
            for (int i = 0; i < lLength; i++)
            {
                lCard = prefabHandDic[lKeys[i]];
                lCard.GO = Save.data.cardPrefabs[i];
                prefabHandDic[lKeys[i]] = lCard;
            }

            prefabHandDic.ToList().RemoveAll(x => !lKeys.Contains(x.Key));

            for (int i = 0; i < lLength; i++)
                prefabHandList[prefabHandList.FindIndex(x => x.GO.GetComponent<Biome>().Type == lKeys[i])] = prefabHandDic[lKeys[i]];
        }
    }

    public static GameObject GetRandomPrefab(List<Card> pList, float pEvaluator) 
    {
        float lTotalWeight = GetTotalWeight(pList, pEvaluator);

        float lRand = UnityEngine.Random.Range(0, lTotalWeight);

        float lCurrentProp = 0;

        for (int i = 0; i < prefabHandList.Count; i++)
        {
            lCurrentProp += pList[i].chanceToSpawn.Evaluate(pEvaluator);
            if (lRand < lCurrentProp) return prefabHandList[i].GO;
        }

        return null;
    }

    public static GameObject GetPrefab(BiomeType type)
    {
        return prefabHandDic[type].GO;
    }

    private static float GetTotalWeight(List<Card> pList, float pEvaluator)
    {
        float ret = 0;
        foreach (Card pCard in pList)
            ret += pCard.chanceToSpawn.Evaluate(pEvaluator);

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
