using com.isartdigital.f2p.manager;
using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.Gameplay.Events;
using Com.IsartDigital.F2P.Sound;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Paul Vincencini
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

    Dictionary<WitchQuestsEnum, WitchQuestText> _WitchQuestDic;

    [Header("Sound")]
    [SerializeField] private SoundEmitter _WitchQuestCompletedSFXEmitter = null;

    private void Start()
    {
        _WitchQuestDic = witchQuestLabels.ToDic();
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
        if(_WitchQuestDic.ContainsKey(currentQuest))
        {
            WitchQuestUiManager.GetInstance().SetQuestName(_WitchQuestDic[currentQuest].name);
            WitchQuestUiManager.GetInstance().SetQuestDesc(_WitchQuestDic[currentQuest].desc);
            WitchQuestUiManager.GetInstance().SetQuestReward(_WitchQuestDic[currentQuest].reward);
        }

        Debug.Log(currentQuest);
    }

    public void Win()
    {
        if (_WitchQuestCompletedSFXEmitter != null)
            _WitchQuestCompletedSFXEmitter.PlaySFXOnShot();

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

    public Dictionary<WitchQuestsEnum, WitchQuestText> ToDic()
    {
        Dictionary<WitchQuestsEnum, WitchQuestText> newDic = new Dictionary<WitchQuestsEnum, WitchQuestText>();

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
    public WitchQuestText value;
}

[Serializable]
public struct WitchQuestText
{
    public string name;
    public string desc;
    public string reward;
}
