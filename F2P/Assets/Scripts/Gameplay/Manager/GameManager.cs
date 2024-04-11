using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro.EditorUtilities;

using UnityEngine;
using UnityEngine.Events;

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

    public enum State
    {
        MovingCard,
        MovingPlayer,
        FieldEffect
    }

    [Header("Card Parameters")]
    [SerializeField] private int _MaxCardStocked = 12;

    [Header("Prefab")]
    [SerializeField] private GameObject _Player;

    [Header("Player Starting GridPosition")]
    [SerializeField] private Vector2 _BasePlayerGridPos;

    // Variables
    private int _CurrentPriority = 1;
    private int _TurnNumber = 1;
    private int _CardStocked = 12;

    private Vector3 _BasePlayerGridPosToPixel;

    [HideInInspector] public bool cardPlayed;
    [HideInInspector] public bool playerMoved;
    [HideInInspector] public bool playerCanMove;

    [HideInInspector]public State currentState;

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

    private void Start()
    {
        _BasePlayerGridPosToPixel = GridManager.GetInstance().GetIndexCoordonate((int)_BasePlayerGridPos.x, (int)_BasePlayerGridPos.y);
        Instantiate(_Player, _BasePlayerGridPosToPixel, Quaternion.identity);
        _Player.GetComponent<Player>().baseGridPos = _BasePlayerGridPos;

        CardPlaced.AddListener(SetModeMovingPlayer);
    }

    public void NextTurn()
    {
        _TurnNumber++;
        cardPlayed = false;
        OnTurnPassed?.Invoke();

        SetModeMovingCard();
    }

    public void SetModeMovingCard()
    {
        currentState = State.MovingCard;
        playerCanMove = false;
        playerMoved = false;
    }

    public void SetModeMovingPlayer()
    {
        currentState = State.MovingPlayer;
        playerCanMove = true;
    }

    public void SetModeFieldEffect()
    {
        currentState = State.FieldEffect;
        playerCanMove = false;
        playerMoved = true;
        cardPlayed = false;
    }

    public static UnityEvent CardPlaced = new UnityEvent();
    public static UnityEvent PlayerMoved = new UnityEvent();


    private void OnDestroy()
    {
        if (_Instance == this)
          _Instance = null;
        GameManager.CardPlaced.RemoveAllListeners();
        GameManager.PlayerMoved.RemoveAllListeners();
    }
}
