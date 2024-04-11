using System;
using UnityEngine;

// Author (CR): Elias Dridi
public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _Instance;

    public static GameManager GetInstance()
    {
        if (_Instance == null) 
            _Instance = new GameManager();
        return _Instance;
    }

    private GameManager() : base() { }
    #endregion

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
    }

    [SerializeField] private int _MaxCardStocked = 12;

    // Variables
    private int _CardStocked = 12;

    private int _TurnNumber = 1;

    public bool cardPlayed;

    private int _CurrentPriority = 1;
    
    // Get / Set
    public int CurrentPriority
    {
        get { return _CurrentPriority; }
    }
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

    // Events
    public event Action OnTurnPassed;

    public void NextTurn()
    {
        _TurnNumber++;
        cardPlayed = false;

        OnTurnPassed?.Invoke();
    }

    private void OnDestroy()
    {
        // Singleton cleaning process
        if (_Instance == this)
            _Instance = null;
    }

}
