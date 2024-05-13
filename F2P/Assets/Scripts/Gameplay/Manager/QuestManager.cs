using System;

using UnityEngine;
using UnityEngine.Events;

namespace com.isartdigital.f2p.manager
{
    public class QuestManager : MonoBehaviour
    {

        public static QuestsEnum currentQuest;

        [SerializeField] private QuestsEnum currentQuestDebug;

        public static UnityEvent ValidQuest = new UnityEvent();

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
            ValidQuest.AddListener(WinDebug);
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

        private void WinDebug()
        {
            Debug.Log("Win");

            GameManager.GetInstance().SetModeWin();
        }

        private void OnDestroy()
        {
            GameFlowManager.LoadMap.RemoveListener(Init);
            ValidQuest.RemoveListener(WinDebug);
        }
    }
}


