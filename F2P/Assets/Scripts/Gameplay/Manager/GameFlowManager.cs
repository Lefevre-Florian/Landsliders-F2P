using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameFlowManager : MonoBehaviour
{
    public static UnityEvent LoadMap = new UnityEvent();
    public static UnityEvent InitGrid = new UnityEvent();

    public static UnityEvent HandLoaded = new UnityEvent();
    public static UnityEvent PlayerLoaded = new UnityEvent();

    void Start()
    {
        LoadMap.Invoke();
        InitGrid.Invoke();
    }

    private void OnDestroy()
    {
        InitGrid.RemoveAllListeners();
        LoadMap.RemoveAllListeners();
    }

}
