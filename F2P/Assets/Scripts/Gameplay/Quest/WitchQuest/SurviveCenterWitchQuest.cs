using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SurviveCenterWitchQuest : MonoBehaviour
{
    public static UnityEvent StartEvent = new UnityEvent();

    [SerializeField] private int _TimeToComplete = 3;
    private int _Timer = 0;
    private void Start()
    {
        StartEvent.AddListener(StartCheck);
    }

    private void StartCheck()
    {
        GameManager.GetInstance().OnTurnPassed += UpdateCheck;
        _Timer = _TimeToComplete;
    }

    private void UpdateCheck()
    {
        _Timer -= 1;
        if (Player.GetInstance().GridPosition == new Vector2(1, 1))
        {
            GameManager.GetInstance().OnTurnPassed -= UpdateCheck;
            Debug.Log("You lose quest");
            return;
        }
        if (_Timer == 0)
        {
            GameManager.GetInstance().OnTurnPassed -= UpdateCheck;
            Debug.Log("Win");
        }


    }

    private void OnDestroy()
    {
        StartEvent.RemoveListener(StartCheck);
    }
}
