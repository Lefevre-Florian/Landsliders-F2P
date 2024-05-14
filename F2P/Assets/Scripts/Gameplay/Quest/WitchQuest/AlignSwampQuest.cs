using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignSwampQuest : MonoBehaviour
{
    private void Start()
    {
        //CheckAroundCanyon();
    }

    private bool CheckAroundCanyon()
    {
        Vector3 baseDir = Vector3.up;
        Vector3 currentDir;
        for (int i = 0; i < 8; i++)
        {
            currentDir = Quaternion.AngleAxis(45 * i, Vector3.forward) * baseDir;
            int lXIndexToCheck = Mathf.RoundToInt(currentDir.x) + Mathf.RoundToInt(GetComponent<CardContainer>().gridPosition.x);
            int lYIndexToCheck = Mathf.RoundToInt(currentDir.y) + Mathf.RoundToInt(GetComponent<CardContainer>().gridPosition.y);

            if (lXIndexToCheck < 0 || lXIndexToCheck > 2 || lYIndexToCheck < 0 || lYIndexToCheck > 2) continue;

            if (CheckBiomeAtIndex(lXIndexToCheck, lYIndexToCheck)) continue;

            if (!CheckBiomeAtIndex(lXIndexToCheck + Mathf.RoundToInt(currentDir.x), lYIndexToCheck + Mathf.RoundToInt(currentDir.y)) && 
                !CheckBiomeAtIndex(lXIndexToCheck - 2 * Mathf.RoundToInt(currentDir.x), lYIndexToCheck - 2 * Mathf.RoundToInt(currentDir.y))) continue;

            return true;
        }

        return false;
    }

    private bool CheckBiomeAtIndex(int pXIndex, int pYIndex)
    {
        return GridManager.GetInstance()._Cards[pXIndex, pYIndex].GetComponent<Biome>().Type == BiomeType.swamp;
    }
}
