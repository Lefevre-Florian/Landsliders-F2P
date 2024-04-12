using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    NONE,
    Plaine,
    Montagne
}

public class CardPrefabDic : MonoBehaviour
{
    [SerializeField] CTGODic test;
    private Dictionary<CardType, GameObject> prefabDic;
    private List<GameObject> prefabList;
    private void Awake()
    {
        prefabDic = test.ToDic();
        prefabList = test.ToList();
    }

    private GameObject GetRandomPrefab() 
    {
        return prefabList[UnityEngine.Random.Range(0, prefabList.Count)];
    }

    private GameObject GetPrefab(CardType type)
    {
        return prefabDic[type];
    }
}

[Serializable]
public class CTGODic
{
    [SerializeField] CTGOItem[] dic;

    public Dictionary<CardType, GameObject> ToDic()
    {
        Dictionary<CardType, GameObject> newDic = new Dictionary<CardType, GameObject>();

        foreach (CTGOItem item in dic)
        {
            newDic.Add(item.key, item.value);
        }

        return newDic;
    }

    public List<GameObject> ToList()
    {
        List<GameObject> newList = new List<GameObject>();

        foreach (CTGOItem item in dic)
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
    public CardType key;
    [SerializeField]
    public GameObject value;
}
