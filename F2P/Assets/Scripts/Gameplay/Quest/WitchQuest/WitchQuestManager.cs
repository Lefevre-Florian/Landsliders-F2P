using Com.IsartDigital.F2P.Gameplay.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static com.isartdigital.f2p.manager.QuestManager;

public class WitchQuestManager : MonoBehaviour
{

    public static WitchQuestsEnum currentQuest;

    [SerializeField] private WitchQuestsEnum currentQuestDebug;

    public enum WitchQuestsEnum
    {
        NONE,
        AlignSwampQuest,
        SurviveSwampQuest,
        FrozenLake
    }

    private void Start()
    {
        Witch.OnWitchPosition.AddListener(GiveQuest);
        currentQuest = currentQuestDebug;
    }

    private void GiveQuest()
    {
        if (currentQuestDebug == WitchQuestsEnum.NONE)
        {
            string[] lQuestsArray = Enum.GetNames(typeof(WitchQuestsEnum));
            int rand = UnityEngine.Random.Range(1, lQuestsArray.Length);

            currentQuest = (WitchQuestsEnum)Enum.Parse(typeof(WitchQuestsEnum), lQuestsArray[rand]);
        }

        Debug.Log(currentQuest);
    }

    private void OnDestroy()
    {
        Witch.OnWitchPosition.RemoveListener(GiveQuest);
    }
}
