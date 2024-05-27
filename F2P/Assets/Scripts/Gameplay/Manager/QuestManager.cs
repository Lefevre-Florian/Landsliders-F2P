using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.Sound;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Paul Vincencini
namespace com.isartdigital.f2p.manager
{
    public class QuestManager : MonoBehaviour
    {

        public static QuestsEnum currentQuest;

        [SerializeField] private QuestsEnum currentQuestDebug;

        public static UnityEvent ValidQuest = new UnityEvent();

        [SerializeField] private QuestLabelsDic _QuestLabelsDic = new QuestLabelsDic();
        private static Dictionary<QuestsEnum, QuestText> questDic;

        [Header("Sound")]
        [SerializeField] private SoundEmitter _QuestCompletedSFXEmitter = null;

        public enum QuestsEnum
        {
            NONE,
            VolcanQuest,
            CanyonQuest,
            FieldQuest,
            FlyingIslandQuest,
            CardQuest,
            VortexQuest,
            WitchQuest,

            FTUE
        }

        private void Awake()
        {
            currentQuest = currentQuestDebug;
            GameFlowManager.LoadMap.AddListener(Init);
            ValidQuest.AddListener(WinDebug);
            questDic = _QuestLabelsDic.ToDic();

            if (currentQuestDebug == QuestsEnum.NONE)
            {
                string[] lQuestsArray = Enum.GetNames(typeof(QuestsEnum));
                lQuestsArray.ToList().Remove(QuestsEnum.FTUE.ToString());

                int rand = UnityEngine.Random.Range(1, lQuestsArray.Length - 1);

                currentQuest = (QuestsEnum)Enum.Parse(typeof(QuestsEnum), lQuestsArray[rand]);

            }
        }

        private void Init()
        {
            
           if(questDic.ContainsKey(currentQuest)) QuestUiManager.GetInstance().SetQuestName(questDic[currentQuest].name);
           if (questDic.ContainsKey(currentQuest)) QuestUiManager.GetInstance().SetQuestDesc(questDic[currentQuest].desc);
        }

        private void WinDebug()
        {
            if (_QuestCompletedSFXEmitter != null)
                _QuestCompletedSFXEmitter.PlaySFXOnShot();
            GameManager.GetInstance().SetModeWin();
        }

        private void OnDestroy()
        {
            GameFlowManager.LoadMap.RemoveListener(Init);
            ValidQuest.RemoveListener(WinDebug);
        }
    }

    [Serializable]
    public class QuestLabelsDic
    {
        [SerializeField] QuestLabelItem[] _Dict;

        public Dictionary<QuestManager.QuestsEnum, QuestText> ToDic()
        {
            Dictionary<QuestManager.QuestsEnum, QuestText> newDic = new Dictionary<QuestManager.QuestsEnum, QuestText>();

            foreach (QuestLabelItem item in _Dict)
            {
                newDic.Add(item.key, item.value);
            }

            return newDic;
        }
    }

    [Serializable]
    public class QuestLabelItem
    {
        [SerializeField]
        public QuestManager.QuestsEnum key;

        [SerializeField]
        public QuestText value;
    }

    [Serializable]
    public struct QuestText
    {
        public string name;
        public string desc;
    }
}


