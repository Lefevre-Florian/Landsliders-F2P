using com.isartdigital.f2p.manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.isartdigital.f2p.gameplay.quest
{
    public class CanyonQuest : Quests
    {
        public static UnityEvent ValidSignal = new UnityEvent();

        protected override void Start()
        {
            base.Start();
            ValidSignal.AddListener(ValidQuest);
        }

        private void ValidQuest()
        {
            QuestManager.ValidQuest.Invoke();
        }

        private void OnDestroy()
        {
            ValidSignal.RemoveListener(ValidQuest);
        }

    }
}