using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;

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

    private const string _CARDCONTAINERTAG = "CardContainer";

    public enum State
    {
        Fixed,
        Movable,
        Moving
    }
    
    public Vector2 baseGridPos;
    [SerializeField] private float _LerpDuration;
    
    // Variables
    private Vector2 _ActualGridPos;
    private Vector2 _PreviousGridPos;

    private Vector2 _GridPosSelected;
    private Vector2 _WorldPosSelected;

    private State _CurrentState;

    private float _LerpTimer;

    private Action DoAction;

    // Get / Set
    public Vector2 GridPosition { get { return _ActualGridPos; } }
    public Vector2 PreviousGridPosition { get { return _PreviousGridPos; } }

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
        _ActualGridPos = baseGridPos;
        _PreviousGridPos = baseGridPos;
        GameManager.CardPlaced.AddListener(SetModeMovable);
        GameManager.PlayerMoved.AddListener(SetModeFixed);
    }

    private void Update()
    {
        DoAction?.Invoke();
        if (Input.GetMouseButtonUp(0) && _CurrentState == State.Movable)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (!hit) return;
            else
            {
                if (hit.collider.tag == _CARDCONTAINERTAG)
                {
                    _GridPosSelected = hit.collider.GetComponent<CardContainer>().gridPosition;

                    if(_GridPosSelected !=_ActualGridPos && _GridPosSelected != _PreviousGridPos)
                    {
                        if (Mathf.Abs(_ActualGridPos.x - _GridPosSelected.x) <= 1 && Mathf.Abs(_ActualGridPos.y - _GridPosSelected.y) <= 1)
                        {
                            _WorldPosSelected = GridManager.GetInstance().GetIndexCoordonate((int)_GridPosSelected.x, (int)_GridPosSelected.y);
                            SetModeMove();
                        }
                    }
                }
            }
        }
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
        yield return new WaitForSeconds(0.5f);
        _CurrentState = State.Movable;
    }

    public void SetModeMovable()
    {
        StartCoroutine(DelayedStateMovable());
    }

    private void SetModeMove()
    {
        _CurrentState = State.Moving;
        DoAction = DoActionMove;
    }

    private void DoActionMove()
    {
        _LerpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_LerpTimer / _LerpDuration);
        transform.position = Vector3.Lerp(GridManager.GetInstance().GetIndexCoordonate((int)_ActualGridPos.x, (int)_ActualGridPos.y), GridManager.GetInstance().GetIndexCoordonate((int)_GridPosSelected.x, (int)_GridPosSelected.y), t);
        if (t >= 1f)
        {
            _LerpTimer = 0f;
            _PreviousGridPos = _ActualGridPos;
            _ActualGridPos = _GridPosSelected;
            GameManager.PlayerMoved.Invoke();
        }
    }
    #endregion

    private void OnDestroy()
    {
        if (_Instance != this)
            return;

        _Instance = null;

        GameManager.CardPlaced.RemoveListener(SetModeMovable);
        GameManager.PlayerMoved.RemoveListener(SetModeFixed);
    }

}
