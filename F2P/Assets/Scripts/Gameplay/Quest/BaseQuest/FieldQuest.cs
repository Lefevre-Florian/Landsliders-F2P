using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.quest
{
    public class FieldQuest : Quests
    {
        public void CheckWin(int nbCardGive)
        {
            if (nbCardGive < 6) return;
            Debug.Log("t'as gagn� gars t'es le meilleur");
        }
    }
}
