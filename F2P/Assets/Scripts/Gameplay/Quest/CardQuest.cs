using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.quest
{
    public class CardQuest : Quests
    {
        private int[] lastTurnCardGet = new int[4];

        private void Start()
        {
            GameManager.PlayerMoved.AddListener(ShiftArray);
        }

        private void ShiftArray()
        {
            for (int i = 1; i < lastTurnCardGet.Length - 1; i++)
            {
                lastTurnCardGet[i] = lastTurnCardGet[i - 1];
            }

            lastTurnCardGet[0] = 0;
        }

        public void AddCard(int pCardToAdd)
        {
            lastTurnCardGet[0] += pCardToAdd;

            int totalCardGainLastFourTurn = 0;
            foreach (int pNbCard in lastTurnCardGet)
                totalCardGainLastFourTurn += pNbCard;

            if (totalCardGainLastFourTurn > 6) Debug.Log("WIN");
        }
    }
}