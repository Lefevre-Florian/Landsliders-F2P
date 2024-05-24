using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.Sound;

using System;
using System.Collections;

using UnityEngine;

// Author (CR) : Elias Dridi
public class Player : MonoBehaviour
{
    #region Singleton
    private static Player _Instance = null;

    public static Player GetInstance()
    {
        if (_Instance == null)
            _Instance = new Player();
        return _Instance;
    }

    private Player() : base() { }
    #endregion

    private const string CARDPLAYED_TAG = "CardPlayed";

    private const int NB_DIRECTION = 8;

    public enum State
    {
        Fixed,
        Movable,
        Moving
    }
    
    public Vector2 baseGridPos;
    [SerializeField] private float _LerpDuration;

    [Header("Sound Effects")]
    [SerializeField] private SoundEmitter _SlidingSFXEmitter = null;
    [SerializeField] private SoundEmitter _MovingSFXEmitter = null;

    [Header("Feedbacks")]
    [SerializeField] private GameObject _MoveSFXPrefab = null;
    [SerializeField] private GameObject _GoBackForbidSFXPrefab = null;
    [SerializeField] private GameObject _ForbidCardPlacedSFX = null;

    // Variables
    [HideInInspector]
    public Vector2 _ActualGridPos;
    private Vector2 _PreviousGridPos;

    private Vector2 _GridPosSelected;
    private Vector2 _WorldPosSelected;

    private State _CurrentState;

    private float _LerpTimer;

    private Action DoAction;

    private bool _IsPaused = false;

    private GameObject[] _MoveSFXs = new GameObject[NB_DIRECTION];
    private GameObject _GoBackSFX = null;

    [HideInInspector]
    public bool isProtected = false;

    private GridManager _GridManager = null;

    // Get / Set
    public Vector2 GridPosition { get { return _ActualGridPos; } }
    public Vector2 PreviousGridPosition { get { return _PreviousGridPos; } }

    public bool _InFTUE = false;

    private void Awake()
    {
        if(_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _Instance = this;
    }

    private void Start()
    {
        _GoBackSFX = Instantiate(_GoBackForbidSFXPrefab, transform);
        _GoBackSFX.transform.position = transform.position;
        _GoBackSFX.SetActive(false);

        _ActualGridPos = baseGridPos;
        _PreviousGridPos = baseGridPos;

        GameManager.CardPlaced.AddListener(SetModeMovable);
        GameManager.CardPlaced.AddListener(HideVFX);

        GameManager.GetInstance().OnTurnPassed += ShowVFX;

        GameManager.PlayerMoved.AddListener(SetModeFixed);
        GameManager.PlayerMoved.AddListener(CheckPlayerCanMove);

        _GridManager = GridManager.GetInstance();

        GameFlowManager.PlayerLoaded.Invoke();

        GameFlowManager.Resumed.AddListener(OnResume);
        GameFlowManager.Paused.AddListener(OnPause);
    }

    private void Update()
    {
        if (_IsPaused)
            return;

        DoAction?.Invoke();

        if (Input.GetMouseButtonUp(0) && _CurrentState == State.Movable)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit && hit.collider.CompareTag(CARDPLAYED_TAG))
            {
                _GridPosSelected = hit.collider.GetComponent<CardContainer>().gridPosition;
                if (_GridPosSelected != _ActualGridPos && _GridPosSelected != _PreviousGridPos
                    && (Mathf.Abs(_ActualGridPos.x - _GridPosSelected.x) <= 1 && Mathf.Abs(_ActualGridPos.y - _GridPosSelected.y) <= 1))
                {
                    _WorldPosSelected = _GridManager.GetWorldCoordinate((int)_GridPosSelected.x, (int)_GridPosSelected.y);
                    if (_GridManager.GetCardByGridCoordinate(_GridPosSelected).IsWalkable)
                    {
                        SetModeMove();   
                    }
                        
                }
            }
        }
    }

    private void OnPause() => _IsPaused = true;

    private void OnResume() => _IsPaused = false;

    [HideInInspector]
    public void SetPosition(Vector2 pPosition)
    {
        _ActualGridPos = _PreviousGridPos = pPosition;
        transform.position = _GridManager.GetWorldCoordinate(pPosition);
    }

    #region State machine
    public void SetModeFixed()
    {
        _CurrentState = State.Fixed;
        DoAction = DoActionFixed;
    }

    private void DoActionFixed() { }

    private IEnumerator DelayedStateMovable()
    {
        yield return new WaitForSeconds(0.1f);
        _CurrentState = State.Movable;
    }

    public void SetModeMovable()
    {
        if (!isProtected)
        {
            // Display sfx
            CreateMovementSFXs();
            if(_PreviousGridPos != _ActualGridPos)
            {
                _GoBackSFX.transform.position = _GridManager.GetWorldCoordinate(_PreviousGridPos);
                _GoBackSFX.SetActive(true);
            }

            StartCoroutine(DelayedStateMovable());
        }
        else
        {
            GameManager.PlayerMoved.Invoke();
        }
    }

    public void SetModeVoid() => DoAction = null;

    public void SetModeMove()
    {
        // Hide sfx
        int lLength = _MoveSFXs.Length;
        for (int i = 0; i < lLength; i++)
            if (_MoveSFXs[i] != null)
                Destroy(_MoveSFXs[i]);

        _GoBackSFX.SetActive(false);

        // Play sound
        if (_MovingSFXEmitter != null)
            _MovingSFXEmitter.PlaySFXLooping();

        _CurrentState = State.Moving;
        DoAction = DoActionMove;

        GetComponent<PlayerAnim>().SetAnimTrig(PlayerAnim.AnimTrig.Transition);

        Vector3 lDir = _GridManager.GetWorldCoordinate((int)_GridPosSelected.x, (int)_GridPosSelected.y) -
                          _GridManager.GetWorldCoordinate((int)_ActualGridPos.x, (int)_ActualGridPos.y);
        lDir = lDir.normalized;

        GetComponent<PlayerAnim>().SetPlayerRot(transform.position + lDir);
    }

    private void DoActionMove()
    {
        _LerpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_LerpTimer / _LerpDuration);
        transform.position = Vector3.Lerp(_GridManager.GetWorldCoordinate((int)_ActualGridPos.x, (int)_ActualGridPos.y),
                                          _GridManager.GetWorldCoordinate((int)_GridPosSelected.x, (int)_GridPosSelected.y), 
                                          t);
        if (t >= 1f)
        {
            _LerpTimer = 0f;
            _PreviousGridPos = _ActualGridPos;
            _ActualGridPos = _GridPosSelected;

            GameManager.PlayerMoved?.Invoke();

            if (_MovingSFXEmitter != null)
                _MovingSFXEmitter.StopSFXLoopingImmediate();

            SetModeVoid();
        }
    }

    public void SetModeSlide(Vector2 pPosition)
    {
        if (_SlidingSFXEmitter != null)
            _SlidingSFXEmitter.PlaySFXOnShot();

        _PreviousGridPos = _ActualGridPos;
        _ActualGridPos = pPosition;
        transform.position = _GridManager.GetWorldCoordinate(pPosition);

        GameManager.PlayerMoved?.Invoke();
    }
    #endregion

    private void CreateMovementSFXs()
    {
        int lLength = NB_DIRECTION;
        float lAngle = ((Mathf.PI * 2f) / NB_DIRECTION) * Mathf.Rad2Deg;

        Vector2 lSample;

        for (int i = 0; i < lLength; i++)
        {
            lSample = _ActualGridPos + (Vector2)(Quaternion.AngleAxis(lAngle * i, Vector3.forward) * Vector3.up);

            lSample.x = lSample.x % 1 <= 0.5f ? Mathf.FloorToInt(lSample.x) : Mathf.CeilToInt(lSample.x);
            lSample.y = lSample.y % 1 <= 0.5f ? Mathf.FloorToInt(lSample.y) : Mathf.CeilToInt(lSample.y);

            if (lSample.x < 0f 
                || lSample.x >= _GridManager._NumCard.x 
                || lSample.y < 0f 
                || lSample.y >= _GridManager._NumCard.y)
                continue;

            if (lSample != _PreviousGridPos && _GridManager.GetCardByGridCoordinate(lSample).IsWalkable)
            {
                _MoveSFXs[i] = Instantiate(_MoveSFXPrefab, transform);
                _MoveSFXs[i].transform.position = _GridManager.GetWorldCoordinate(lSample);
            }
            else
            {
                _MoveSFXs[i] = null;
            }
        }
    }

    private void ShowVFX() => _ForbidCardPlacedSFX.SetActive(true);

    private void HideVFX() => _ForbidCardPlacedSFX.SetActive(false);

    private void CheckPlayerCanMove()
    {
        if (_InFTUE) return;

        Vector3 baseDir = Vector3.right;

        for (int i = 0; i < 8; i++)
        {
            Vector3 dir =  Quaternion.AngleAxis(45 * i, Vector3.forward) * baseDir;
            dir.x = Mathf.RoundToInt(dir.x);
            dir.y = Mathf.RoundToInt(dir.y);

            Vector2Int currentIndex = new Vector2Int(Mathf.RoundToInt(GridPosition.x) + Mathf.RoundToInt(dir.x),
                                                     Mathf.RoundToInt(GridPosition.y) + Mathf.RoundToInt(dir.y));

            if (currentIndex.x < 0 || currentIndex.x > 2 || currentIndex.y < 0 || currentIndex.y > 2) continue;

            if (GridManager.GetInstance()._Cards[currentIndex.x, currentIndex.y].GetComponent<Biome>().GridPosition == PreviousGridPosition) continue;

            if (GridManager.GetInstance()._Cards[currentIndex.x, currentIndex.y].GetComponent<Biome>().IsWalkable) return;

        }

        //GameManager.GetInstance().SetModeGameover();
    }

    private void OnDestroy()
    {
        if (_Instance != this)
            return;

        _Instance = null;

        GameManager.CardPlaced.RemoveListener(SetModeMovable);
        GameManager.PlayerMoved.RemoveListener(SetModeFixed);
        GameManager.PlayerMoved.RemoveListener(CheckPlayerCanMove);

        GameFlowManager.Paused.RemoveListener(OnPause);
        GameFlowManager.Resumed.RemoveListener(OnResume);

        GameManager.GetInstance().OnTurnPassed -= ShowVFX;

        GameManager.CardPlaced.RemoveListener(HideVFX);
    }
}
