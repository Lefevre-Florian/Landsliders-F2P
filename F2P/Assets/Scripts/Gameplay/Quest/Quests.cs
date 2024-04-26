using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Quests : MonoBehaviour
{
    [SerializeField] private QuestManager.QuestsEnum myQuest;

    private void Awake()
    {
        if (myQuest != QuestManager.currentQuest) DestroyImmediate(this);
    }

    
}
