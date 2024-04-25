using com.isartdigital.f2p.gameplay.manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetterTest : MonoBehaviour
{
    [SerializeField] private MapScriptableObj _predifinedMap;
    private GameObject[,] map = new GameObject[3, 3];
    private void Awake()
    {
        GameFlowManager.LoadMap.AddListener(Init);
    }

    private void Init()
    {
        if (_predifinedMap == null) return;

        List<GameObject[]> list = new List<GameObject[]>{
            _predifinedMap.Map0, _predifinedMap.Map1, _predifinedMap.Map2
        };

        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++)
            {
                map[i, j] = list[2-j][i];
            }
        }

        GridManager.GetInstance()._Cards = map;
    }

    private void OnDestroy()
    {
        
        GameFlowManager.LoadMap.RemoveListener(Init);
    }
}
