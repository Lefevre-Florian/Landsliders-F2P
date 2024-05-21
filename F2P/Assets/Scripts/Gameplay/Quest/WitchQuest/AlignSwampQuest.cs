using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.Gameplay.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignSwampQuest : MonoBehaviour
{
    private WitchQuestsEnum myQuest = WitchQuestsEnum.AlignSwampQuest;

    private void Start()
    {
        GetComponent<TEMPCard>().OnPlaced += CheckAroundCanyon;
    }

    private void CheckAroundCanyon()
    {
        if(myQuest != WitchQuestsEnum.AlignSwampQuest) return;
        Vector3 baseDir = Vector3.up;
        Vector3 currentDir;
        for (int i = 0; i < 8; i++)
        {
            currentDir = Quaternion.AngleAxis(45 * i, Vector3.forward) * baseDir;
            int lXIndexToCheck = Mathf.RoundToInt(currentDir.x) + Mathf.RoundToInt(GetComponent<CardContainer>().gridPosition.x);
            int lYIndexToCheck = Mathf.RoundToInt(currentDir.y) + Mathf.RoundToInt(GetComponent<CardContainer>().gridPosition.y);


            if (!CheckBiomeAtIndex(lXIndexToCheck, lYIndexToCheck)) continue;

            if (!CheckBiomeAtIndex(lXIndexToCheck + Mathf.RoundToInt(currentDir.x), lYIndexToCheck + Mathf.RoundToInt(currentDir.y)) && 
                !CheckBiomeAtIndex(lXIndexToCheck - 2 * Mathf.RoundToInt(currentDir.x), lYIndexToCheck - 2 * Mathf.RoundToInt(currentDir.y))) continue;

            WitchQuestManager.WitchWinEvent.Invoke();
            return;
        }
    }

    private bool CheckBiomeAtIndex(int pXIndex, int pYIndex)
    {
        if (pXIndex < 0 || pXIndex > 2 || pYIndex < 0 || pYIndex > 2) return false;
        return GridManager.GetInstance()._Cards[pXIndex, pYIndex].GetComponent<Biome>().Type == BiomeType.swamp;
    }
}
