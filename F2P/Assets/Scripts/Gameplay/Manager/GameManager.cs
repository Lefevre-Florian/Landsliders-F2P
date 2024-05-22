using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.Gameplay;

using System;
using System.Collections;
using System.Collections.Generic;

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

    #region Tracking   
    private const string TRACKER_NAME = "gameDuration";

    private const string TRACKER_GAME_DURATION_REALTIME_PARAMETER = "timeInSecondMinute";
    private const string TRACKER_GAME_DURATION_TURN_PARAMETER = "numberOfTurn";
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
    private int _MaxPriority = 12;

    private int _TurnNumber = 1;
    private int _CardStocked = 12;

    private Coroutine _EffectTimer = null;

    private DateTime _GameStartTime = default;

    [HideInInspector] public bool cardPlayed;
    [HideInInspector] public bool playerMoved;
    [HideInInspector] public bool playerCanMove;

    [HideInInspector] public State currentState;

    [HideInInspector] public GameObject _LastCardPlayed;

    [HideInInspector] public List<Dragon> randomEventObjects = new List<Dragon>();

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

    public float EffectDuration { get { return _SecondsBetweenEachPriorityExecution; } }

    // Events
    public event Action OnTurnPassed;

    public event Action<int> OnEffectPlayed;
    public event Action OnAllEffectPlayed;

    public event Action<bool> OnGameover;

    public static UnityEvent CardPlaced = new UnityEvent();
    public static UnityEvent PlayerMoved = new UnityEvent();

    private void Start()
    {
        Vector3 lWorldPosition = GridManager.GetInstance().GetWorldCoordinate(_BasePlayerGridPos);
        Instantiate(_Player, lWorldPosition, Quaternion.identity);
        _Player.GetComponent<Player>().baseGridPos = _BasePlayerGridPos;

        CardPlaced.AddListener(SetModeMovingPlayer);
        PlayerMoved.AddListener(SetModeBiomeEffect);
        OnAllEffectPlayed += CheckDesertLose;

        _GameStartTime = DateTime.UtcNow;
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

        OnGameover?.Invoke(false);
    }

    public void SetModeWin()
    {
        currentState = State.GameEnd;
        playerCanMove = false;

        // Track game duration
        TimeSpan lDuration = (DateTime.UtcNow - _GameStartTime).Duration();
        DataTracker.GetInstance().SendAnalytics(TRACKER_NAME, 
                                                new Dictionary<string, object>() {
                                                    { TRACKER_GAME_DURATION_TURN_PARAMETER, _TurnNumber},
                                                    {TRACKER_GAME_DURATION_REALTIME_PARAMETER,  lDuration.Minutes + ":" + lDuration.Seconds}
                                                });

        OnGameover?.Invoke(true);
    }
    #endregion

    #region Utilities
    private IEnumerator EffectTurnByTurn()
    {
        while (_CurrentPriority != _MaxPriority + 1)
        {
            OnEffectPlayed?.Invoke(_CurrentPriority);
            _CurrentPriority += 1;

            yield return new WaitForSeconds(_SecondsBetweenEachPriorityExecution);
        }
        _CurrentPriority = 1;

        OnAllEffectPlayed?.Invoke();

        if (randomEventObjects.Count > 0)
        {
            List<Dragon> lRandomEventObjects = new List<Dragon>(randomEventObjects);

            while (lRandomEventObjects.Count > 0)
            {
                for (int i = lRandomEventObjects.Count - 1; i >= 0; i--)
                {
                    if(lRandomEventObjects[i].IsDone)
                    {
                        lRandomEventObjects.RemoveAt(i);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        if (_EffectTimer != null)
        {
            StopCoroutine(_EffectTimer);
            _EffectTimer = null;
        }

        NextTurn();
    }

    private void CheckDesertLose()
    {
        GameObject[,] lCard = GridManager.GetInstance()._Cards;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (lCard[i, j].GetComponent<Biome>().Type != BiomeType.desert) return;
            }
        }

        SetModeGameover();
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

        OnAllEffectPlayed -= CheckDesertLose;
    }
}
