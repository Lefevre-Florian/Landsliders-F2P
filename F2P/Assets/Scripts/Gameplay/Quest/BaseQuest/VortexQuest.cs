using com.isartdigital.f2p.manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.quest
{
    public class VortexQuest : Quests
    {
        public void ValidQuest(Vector2 replaceIndex)
        {
            if (replaceIndex == Player.GetInstance().GridPosition) QuestManager.ValidQuest.Invoke();
        }
    }
}