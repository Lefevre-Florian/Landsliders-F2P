using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexQuest : Quests
{
    public void ValidQuest(Vector2 replaceIndex)
    {
        if (replaceIndex == Player.GetInstance().GridPosition) Debug.Log("WIN");
    }
}
