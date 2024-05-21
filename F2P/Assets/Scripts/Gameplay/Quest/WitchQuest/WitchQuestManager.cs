using com.isartdigital.f2p.manager;
using Com.IsartDigital.F2P.Gameplay.Events;
using System;
using UnityEngine;
using UnityEngine.Events;

public class WitchQuestManager : MonoBehaviour
{

    public static WitchQuestsEnum currentQuest;

    [SerializeField] private WitchQuestsEnum currentQuestDebug;

    public static UnityEvent WitchWinEvent = new UnityEvent();

    public enum WitchQuestsEnum
    {
        NONE,
        AlignSwampQuest,
        SurviveSwampQuest,
        FrozenLake,
        SurviveCenterQuest
    }

    private void Start()
    {
        Witch.OnWitchPosition.AddListener(GiveQuest);
        currentQuest = currentQuestDebug;
        WitchWinEvent.AddListener(Win);
    }

    private void GiveQuest()
    {
        if (currentQuestDebug == WitchQuestsEnum.NONE)
        {
            string[] lQuestsArray = Enum.GetNames(typeof(WitchQuestsEnum));
            int rand = UnityEngine.Random.Range(1, lQuestsArray.Length);

            currentQuest = (WitchQuestsEnum)Enum.Parse(typeof(WitchQuestsEnum), lQuestsArray[rand]);
        }
        if (currentQuest == WitchQuestsEnum.SurviveCenterQuest) SurviveCenterWitchQuest.StartEvent.Invoke();
        Debug.Log(currentQuest);
    }

    public void Win()
    {
        if (QuestManager.currentQuest == QuestManager.QuestsEnum.WitchQuest) QuestManager.ValidQuest.Invoke();
        else HandManager.GetInstance().AddCardToDeck(4);
    }

    private void OnDestroy()
    {
        Witch.OnWitchPosition.RemoveListener(GiveQuest);
        WitchWinEvent.RemoveListener(Win);
    }
}
