using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurviveSwampQuest : MonoBehaviour
{
    [SerializeField] private int _TimeToComplete = 3;

    private int _Timer = 0;

    private void Start()
    {
        GameManager.GetInstance().OnTurnPassed += UpdateCheck;
        TriggerSurviveSwampQuest.surviveSwampTrigger.AddListener(StartCheck);
    }

    public void StartCheck()
    {
        if (WitchQuestManager.currentQuest != WitchQuestManager.WitchQuestsEnum.SurviveSwampQuest) return;
        if(_Timer == 0) _Timer = _TimeToComplete;
    }

    private void UpdateCheck()
    {
        if (_Timer == 0) return;
        _Timer--;
        if (_Timer == 0) Debug.Log("Win");
        
    }

    private void OnDestroy()
    {
        TriggerSurviveSwampQuest.surviveSwampTrigger.RemoveListener(StartCheck);
    }
}
