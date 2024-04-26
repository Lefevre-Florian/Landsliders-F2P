using com.isartdigital.f2p.gameplay.card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanQuest : Quests
{
    public void ValidQuest()
    {
        if(GetComponent<CardContainer>().gridPosition == Vector2.one) StartCoroutine(WaitBurn());
    }

    IEnumerator WaitBurn()
    {
        yield return new WaitForEndOfFrame();
        if(HandManager.GetInstance()._CardInHand > 0) Debug.Log("T'as win");
    }
}
