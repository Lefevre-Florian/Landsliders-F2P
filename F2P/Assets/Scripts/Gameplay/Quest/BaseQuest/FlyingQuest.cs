using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using com.isartdigital.f2p.manager;
using Com.IsartDigital.F2P.Biomes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.quest
{
    public class FlyingQuest : Quests
    {
        public void CheckQuest()
        {
            Vector2 gridPos = GetComponent<CardContainer>().gridPosition;
            Vector2 baseDir = Vector2.right;

            for (int i = 0; i < 4; i++)
            {
                Vector2 lIndexToCheck = gridPos + (Vector2)(Quaternion.AngleAxis(90 * i, Vector3.forward) * baseDir);
                if (lIndexToCheck.x < 0 || lIndexToCheck.y < 0 || lIndexToCheck.x > 2 || lIndexToCheck.y > 2) continue;

                GameObject lCardToCheck = GridManager.GetInstance()._Cards[Mathf.RoundToInt(lIndexToCheck.x), Mathf.RoundToInt(lIndexToCheck.y)];
                if (lCardToCheck.GetComponent<Biome>().Type != BiomeType.grassland) return;
            }

            QuestManager.ValidQuest.Invoke();
        }
    }
}