using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerSurviveSwampQuest : MonoBehaviour
{
    public static UnityEvent surviveSwampTrigger = new UnityEvent();

    public void InvokeTrigger() => surviveSwampTrigger.Invoke();
}
