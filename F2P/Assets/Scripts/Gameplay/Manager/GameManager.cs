using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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

    [SerializeField] private int _MaxCardStocked = 12;
    private int _CardStocked = 12;
    public int cardStocked
    {
        get
        {
            return _CardStocked;
        }
        set
        {
            cardStocked = value;
        }        
    }
    private int _TurnNumber = 1;
    public bool cardPlayed;

    public void NextTurn()
    {
        _TurnNumber++;
        cardPlayed = false;
    }
   
}
