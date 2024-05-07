using com.isartdigital.f2p.manager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.quest
{
    public class Quests : MonoBehaviour
    {
        [SerializeField] private QuestManager.QuestsEnum myQuest;

        protected virtual void Start()
        {
            if (myQuest != QuestManager.currentQuest) DestroyImmediate(this);
        }


    }
}