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
        }

        private void Init()
        {
            if(currentQuestDebug == QuestsEnum.NONE)
            {
                string[] lQuestsArray = Enum.GetNames(typeof(QuestsEnum));
                int rand = UnityEngine.Random.Range(1, lQuestsArray.Length);

                currentQuest = (QuestsEnum)Enum.Parse(typeof(QuestsEnum), lQuestsArray[rand]);
            }

            Debug.Log(currentQuest);

        }

        private void OnDestroy()
        {
            GameFlowManager.LoadMap.RemoveListener(Init);
        }


    }
}


