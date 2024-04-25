using com.isartdigital.f2p.gameplay.manager;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

// Author (CR): Elias Dridi
public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _Instance = null;

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
        BiomeEffect,
        GameEnd
    }

    [Header("Card Parameters")]
    [SerializeField] private int _MaxCardStocked = 12;

    [Header("Prefab")]
    [SerializeField] private GameObject _Player;

    [Header("Player Starting GridPosition")]
    [SerializeField] private Vector2 _BasePlayerGridPos;

    [Header("Game flow")]
    [SerializeField][Range(0.1f, 1f)] private float _SecondsBetweenEachPriorityExecution = 0.5f;

    // Variables
    private int _CurrentPriority = 1;
    private int _MaxPriority = 12; // Temp value will be reduce to 1

    private int _TurnNumber = 1;
    private int _CardStocked = 12;

    private Vector3 _BasePlayerGridPosToPixel;

    private Coroutine _EffectTimer = null;

    [HideInInspector] public bool cardPlayed;
    [HideInInspector] public bool playerMoved;
    [HideInInspector] public bool playerCanMove;

    [HideInInspector] public State currentState;

    // Get / Set
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
    public int Turn { get { return _TurnNumber; } }
    // Events
    public event Action OnTurnPassed;
    public event Action<int> OnEffectPlayed;

    public static UnityEvent CardPlaced = new UnityEvent();
    public static UnityEvent PlayerMoved = new UnityEvent();

    private void Start()
    {
        _BasePlayerGridPosToPixel = GridManager.GetInstance().GetWorldCoordinate((int)_BasePlayerGridPos.x, (int)_BasePlayerGridPos.y);
        Instantiate(_Player, _BasePlayerGridPosToPixel, Quaternion.identity);
        _Player.GetComponent<Player>().baseGridPos = _BasePlayerGridPos;

        CardPlaced.AddListener(SetModeMovingPlayer);
        PlayerMoved.AddListener(SetModeBiomeEffect);
    }

    public void NextTurn()
    {
        _TurnNumber++;
        cardPlayed = false;
        OnTurnPassed?.Invoke();

        SetModeMovingCard();
    }

    #region States

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

    public void SetModeBiomeEffect()
    {
        currentState = State.BiomeEffect;
        playerCanMove = false;
        playerMoved = true;
        cardPlayed = false;

        if (_EffectTimer != null)
            StopCoroutine(_EffectTimer);
        _EffectTimer = StartCoroutine(EffectTurnByTurn());
    }

    public void SetModeGameover()
    {
        currentState = State.GameEnd;
        playerCanMove = false;
        
        ///TODO Trigger popup
    }
    #endregion

    #region Utilities
    private IEnumerator EffectTurnByTurn()
    {
        while (_CurrentPriority != _MaxPriority)
        {
            OnEffectPlayed?.Invoke(_CurrentPriority);
            _CurrentPriority += 1;

            yield return new WaitForSeconds(_SecondsBetweenEachPriorityExecution);
        }
        _CurrentPriority = 1;

        if(_EffectTimer != null)
        {
            StopCoroutine(_EffectTimer);
            _EffectTimer = null;
        }

        NextTurn();
    }
    #endregion

    private void OnDestroy()
    {
        if (_Instance == this)
          _Instance = null;

        StopAllCoroutines();
        _EffectTimer = null;

        CardPlaced.RemoveAllListeners();
        PlayerMoved.RemoveAllListeners();
    }
}
