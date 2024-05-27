using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Paul Vincencini
public class GameFlowManager : MonoBehaviour
{
    public static UnityEvent LoadMap = new UnityEvent();
    public static UnityEvent InitGrid = new UnityEvent();

    public static UnityEvent HandLoaded = new UnityEvent();
    public static UnityEvent PlayerLoaded = new UnityEvent();

    public static UnityEvent Paused = new UnityEvent();
    public static UnityEvent Resumed = new UnityEvent();

    void Start()
    {
        LoadMap.Invoke();
        InitGrid.Invoke();
    }

    private void OnDestroy()
    {
        InitGrid.RemoveAllListeners();
        LoadMap.RemoveAllListeners();

        Paused.RemoveAllListeners();
    }

}
