using com.isartdigital.f2p.manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.quest
{
    public class FieldQuest : Quests
    {
        public void CheckWin(int nbCardGive)
        {
            if (nbCardGive < 4) return;
            QuestManager.ValidQuest.Invoke(); ;
        }
    }
}
