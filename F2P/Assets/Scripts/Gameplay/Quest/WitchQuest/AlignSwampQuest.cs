using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.Gameplay.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignSwampQuest : MonoBehaviour
{
    private WitchQuestManager.WitchQuestsEnum myQuest = WitchQuestManager.WitchQuestsEnum.AlignSwampQuest;

    private void Start()
    {
        GetComponent<TEMPCard>().OnPlaced += CheckAroundCanyon;
    }

    private void CheckAroundCanyon()
    {
        Debug.Log("kp");
        if(myQuest != WitchQuestManager.WitchQuestsEnum.AlignSwampQuest) return;
        Vector3 baseDir = Vector3.up;
        Vector3 currentDir;
        for (int i = 0; i < 8; i++)
        {
            currentDir = Quaternion.AngleAxis(45 * i, Vector3.forward) * baseDir;
            int lXIndexToCheck = Mathf.RoundToInt(currentDir.x) + Mathf.RoundToInt(GetComponent<CardContainer>().gridPosition.x);
            int lYIndexToCheck = Mathf.RoundToInt(currentDir.y) + Mathf.RoundToInt(GetComponent<CardContainer>().gridPosition.y);

            if (lXIndexToCheck < 0 || lXIndexToCheck > 2 || lYIndexToCheck < 0 || lYIndexToCheck > 2) continue;

            if (CheckBiomeAtIndex(lXIndexToCheck, lYIndexToCheck)) continue;

            if (!CheckBiomeAfter(new Vector2Int(lXIndexToCheck, lYIndexToCheck), currentDir) && 
                !CheckBiomeBehind(new Vector2Int(lXIndexToCheck, lYIndexToCheck), currentDir)) continue;

            Debug.Log("WIN");
            return;
        }
    }

    private bool CheckBiomeAtIndex(int pXIndex, int pYIndex)
    {
        return GridManager.GetInstance()._Cards[pXIndex, pYIndex].GetComponent<Biome>().Type == BiomeType.swamp;
    }

    private bool CheckBiomeBehind(Vector2Int pIndex, Vector2 currentDir)
    {
        Vector2Int finalDir = new Vector2Int(Mathf.RoundToInt(currentDir.x), Mathf.RoundToInt(currentDir.y));
        Vector2Int finalIndex = pIndex - 2 * finalDir;

        if(finalIndex.x < 0 || finalIndex.x > 2 || finalIndex.y < 0 || finalIndex.y > 2) return false;

        return CheckBiomeAtIndex(finalIndex.x, finalIndex.y);
    }

    private bool CheckBiomeAfter(Vector2Int pIndex, Vector2 currentDir)
    {
        Vector2Int finalDir = new Vector2Int(Mathf.RoundToInt(currentDir.x), Mathf.RoundToInt(currentDir.y));
        Vector2Int finalIndex = pIndex + finalDir;

        if (finalIndex.x < 0 || finalIndex.x > 2 || finalIndex.y < 0 || finalIndex.y > 2) return false;

        return CheckBiomeAtIndex(finalIndex.x, finalIndex.y);
    }
}
