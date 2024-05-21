using com.isartdigital.f2p.manager;
using Com.IsartDigital.F2P.Gameplay.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum WitchQuestsEnum
{
    NONE,
    AlignSwampQuest,
    SurviveSwampQuest,
    FrozenLake,
    SurviveCenterQuest
}

public class WitchQuestManager : MonoBehaviour
{

    public static WitchQuestsEnum currentQuest;

    [SerializeField] private WitchQuestsEnum currentQuestDebug;

    public static UnityEvent WitchWinEvent = new UnityEvent();

    [SerializeField] public WitchQuestLabelsDic witchQuestLabels = new WitchQuestLabelsDic();


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

[Serializable]
public class WitchQuestLabelsDic
{
    [SerializeField] WitchQuestLabelItem[] _Dict;

    public Dictionary<WitchQuestsEnum, QuestText> ToDic()
    {
        Dictionary<WitchQuestsEnum, QuestText> newDic = new Dictionary<WitchQuestsEnum, QuestText>();

        foreach (WitchQuestLabelItem item in _Dict)
        {
            newDic.Add(item.key, item.value);
        }

        return newDic;
    }
}

[Serializable]
public class WitchQuestLabelItem
{
    [SerializeField]
    public WitchQuestsEnum key;

    [SerializeField]
    public QuestText value;
}

[Serializable]
public struct WitchQuestText
{
    public string name;
    public string desc;
    public string reward;
}
