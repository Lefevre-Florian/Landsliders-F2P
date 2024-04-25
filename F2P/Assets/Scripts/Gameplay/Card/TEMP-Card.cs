using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using System;
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

    [HideInInspector]
    public State currentState;
    private Vector3 _GridPlacement;

    private const string CARDPLAYED_TAG = "CardPlayed";
    private const string PLAYER_NAME = "Player";

    private HandManager _HandManager = HandManager.GetInstance();
    private Action DoAction;

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
    

    /*
      TODO : faire une liste des colliders actuels et vérifier quel est le plus proche 
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "Player" && collision.GetComponent<TEMPCard>().currentState != State.InHand  )
        {
            _Snapable = true;
            snapPos = collision.transform.position;
            _SnapParent = collision.gameObject;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _Snapable = false;

        if(_HandManager != null)
            snapPos = _HandManager._CardsSlot[handIndex].transform.position;
      
        _SnapParent = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.name != "Player" && collision.GetComponent<TEMPCard>().currentState != State.InHand)
        {
            _Snapable = true;
            snapPos = collision.transform.position;
            _SnapParent = collision.gameObject;
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
        if (Input.GetMouseButtonUp(0) && _Snapable)
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
