using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanyonQuest : MonoBehaviour
{
    public static UnityEvent ValidSignal = new UnityEvent();

    private void Start()
    {
        ValidSignal.AddListener(ValidQuest);
    }

    private void ValidQuest()
    {
        Debug.Log("Win");
    }

    private void OnDestroy()
    {
        ValidSignal.RemoveListener(ValidQuest);
    }

}
