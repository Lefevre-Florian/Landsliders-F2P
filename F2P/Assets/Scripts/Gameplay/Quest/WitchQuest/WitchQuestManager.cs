using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WitchQuestManager : MonoBehaviour
{
    private List<UnityEvent> _AllQuests = new List<UnityEvent>();

    [HideInInspector] public UnityEvent alignSwampQuest;

    private void Start()
    {
        _AllQuests.Add(alignSwampQuest);

    }

    private void GiveQuest()
    {
        int rand = Mathf.FloorToInt(Random.Range(0, _AllQuests.Count));
        _AllQuests[rand].Invoke();
    }
}
