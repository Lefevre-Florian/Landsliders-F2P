using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static GameManager GetInstance()
    {
        if (instance == null) instance = new GameManager();
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;
    }


    private int _TurnNumber = 1;
    public bool cardPlayed;

    public void NextTurn()
    {
        _TurnNumber++;
        cardPlayed = false;
    }
   
}
