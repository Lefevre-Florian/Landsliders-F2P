using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using System;
using System.Collections.Generic;
using UnityEngine;

// Author (CR) : Paul Vincencini
public class TEMPCard : MonoBehaviour
{

    RaycastHit2D _Hit;
    // Variables
    public int handIndex;

    private bool _Snapable;
    [HideInInspector]
    public Vector3 snapPos;
    private GameObject _SnapParent;
    private GameObject _ClosestSnapParent;

    
    public State currentState;
    private Vector3 _GridPlacement;

    private const string CARDPLAYED_TAG = "CardPlayed";
    private const string PLAYER_NAME = "Player";

    private HandManager _HandManager = HandManager.GetInstance();
    private Action DoAction;

    private List<Collider2D> _CollidingObjects = new List<Collider2D>();

    // Event
    public event Action OnPlaced;

    public enum State
    {
        InHand,
        Moving,
        Played
    }

    void Start()
    {
        _HandManager = HandManager.GetInstance();      
    }

    private void Update()
    {
      if(DoAction!=null)   DoAction();
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "Player" && collision.GetComponent<TEMPCard>().currentState != State.InHand && collision.GetComponent<Biome>().Type != GetComponent<Biome>().Type)
        {
            _CollidingObjects.Add(collision);
            _Snapable = true;
            snapPos = collision.transform.position;
            _SnapParent = collision.gameObject;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _CollidingObjects.Remove(collision);
        _Snapable = false;

        if(_HandManager != null)
            snapPos = _HandManager._CardsSlot[handIndex].transform.position;
      
        _SnapParent = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name != "Player" && collision.GetComponent<TEMPCard>().currentState != State.InHand && collision.GetComponent<Biome>().Type != GetComponent<Biome>().Type)
        {
            if (_CollidingObjects.Count > 1)
            {
                for (int i = 0; i < _CollidingObjects.Count; i++)
                {
                    if (i > 1)
                    {
                        if (Vector2.Distance(_CollidingObjects[i].transform.position, transform.position) < Vector2.Distance(_ClosestSnapParent.transform.position, transform.position))
                        {
                            _ClosestSnapParent = _CollidingObjects[i].gameObject;
                        }
                    }
                    else _ClosestSnapParent = _CollidingObjects[i].gameObject;

                }
                _Snapable = true;
                snapPos = _ClosestSnapParent.transform.position;
                _SnapParent = _ClosestSnapParent.gameObject;
            }
            else
            {
                _Snapable = true;
                snapPos = collision.transform.position;
                _SnapParent = collision.gameObject;
            }
        }
    }

    public void SetModeInHand()
    {
        currentState = State.InHand;
        gameObject.SetActive(true);
        DoAction = DoActionInHand;        
    }

    private void DoActionInHand()
    {
        if (GameManager.GetInstance().currentState != GameManager.State.MovingCard) return;
         _Hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (!_Hit) return;
        if (Input.GetMouseButtonDown(0) && _Hit.collider.transform == transform)
        {
            SetModeMoving();
        }
    }

    public void SetModeMoving()
    {
        currentState = State.Moving;
        DoAction = DoActionMoving;
    }

    private void DoActionMoving()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 2);
        if (Input.GetMouseButtonUp(0) && _Snapable && _SnapParent.GetComponent<Biome>().Type != GetComponent<Biome>().Type)
        {
            _GridPlacement = _SnapParent.GetComponent<CardContainer>().gridPosition;
            transform.position = snapPos;
            _HandManager._AvailableCardSlots[handIndex] = true;
            GetComponent<CardContainer>().gridPosition = _SnapParent.GetComponent<CardContainer>().gridPosition;
            GridManager.GetInstance()._Cards[(int)GetComponent<CardContainer>().gridPosition.x,(int) GetComponent<CardContainer>().gridPosition.y] = gameObject;
            Destroy(_SnapParent.transform.gameObject);
            transform.SetParent(GridManager.GetInstance().transform);
            GameManager.CardPlaced.Invoke();
            tag = CARDPLAYED_TAG;
            SetModePlayed();
        }
        else if (Input.GetMouseButtonUp(0) && !_Snapable) 
        {
            transform.position = snapPos;
            SetModeInHand();
        }
    }
    public void SetModePlayed()
    {
        currentState |= State.Played;
        OnPlaced?.Invoke();
        enabled = false;
    }
}
