using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenLakeQuest : MonoBehaviour
{
    void Start()
    {
        GameManager.GetInstance().OnTurnPassed += CheckValidQuest;
    }

    private void CheckValidQuest()
    {
        if (WitchQuestManager.currentQuest != WitchQuestManager.WitchQuestsEnum.FrozenLake) return;

        Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(Player.GetInstance().GridPosition.x),
                                              Mathf.RoundToInt(Player.GetInstance().GridPosition.y));

        if (GridManager.GetInstance()._Cards[playerPos.x, playerPos.y].GetComponent<Biome>().Type == BiomeType.glacier) WitchQuestManager.WitchWinEvent.Invoke(); ;
    }
}
