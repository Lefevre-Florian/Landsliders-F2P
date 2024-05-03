using Com.IsartDigital.F2P.Biomes;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace com.isartdigital.f2p.manager
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] QuestDic questsDic;
        Dictionary<QuestsEnum, MonoBehaviour> _QuestDic;

        public static QuestsEnum currentQuest;

        [SerializeField] private QuestsEnum currentQuestDebug;

        public enum QuestsEnum
        {
            NONE,
            VolcanQuest,
            CanyonQuest,
            FieldQuest,
            FlyingIslandQuest,
            CardQuest,
            VortexQuest
        }

        private void Awake()
        {
            currentQuest = currentQuestDebug;
            GameFlowManager.LoadMap.AddListener(Init);
            _QuestDic = questsDic.ToDic();
        }

        private void Init()
        {
            if(currentQuestDebug == QuestsEnum.NONE)
            {
                string[] lQuestsArray = Enum.GetNames(typeof(QuestsEnum));
                int rand = UnityEngine.Random.Range(1, lQuestsArray.Length);

                currentQuest = (QuestsEnum)Enum.Parse(typeof(QuestsEnum), lQuestsArray[rand]);
            }

            if(_QuestDic.ContainsKey(currentQuest)) gameObject.AddComponent(_QuestDic[currentQuest].GetType());

            Debug.Log(currentQuest);

        }

        private void OnDestroy()
        {
            GameFlowManager.LoadMap.RemoveListener(Init);
        }


    }

    [Serializable]
    public class QuestDic
    {
        [SerializeField] private QuestItem[] questArray;

        public Dictionary<QuestManager.QuestsEnum, MonoBehaviour> ToDic()
        {
            Dictionary<QuestManager.QuestsEnum, MonoBehaviour> newDic = new Dictionary<QuestManager.QuestsEnum, MonoBehaviour>();

            foreach (QuestItem item in questArray)
            {
                newDic.Add(item.key, item.value);
            }

            return newDic;
        }
    }

    [Serializable]
    public class QuestItem
    {
        [SerializeField]
        public QuestManager.QuestsEnum key;

        [SerializeField]
        public MonoBehaviour value;
    }
}


