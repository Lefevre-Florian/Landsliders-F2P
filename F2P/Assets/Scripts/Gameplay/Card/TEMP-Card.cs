using System;
using UnityEngine;

// Author (CR) : Paul Vincencini
public class TEMPCard : MonoBehaviour
{
    // Variables
    private BoxCollider2D _Collider2D;
    public int handIndex;

    private bool _Snapable;
    private Vector3 _SnapPos;
    private GameObject _SnapParent;

    private State _CurrentState;

    private HandManager _HandManager = HandManager.GetInstance();

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

        _Collider2D = GetComponent<BoxCollider2D>();
        _Collider2D.enabled = true;

        _SnapPos = _HandManager._CardsSlot[handIndex].transform.position;
    }

    private void OnMouseUp()
    {
        if (GameManager.GetInstance().currentState == GameManager.State.MovingCard)
        {
            if(_Snapable && _CurrentState == State.Moving)
            {
                transform.position = _SnapPos;
                _HandManager._AvailableCardSlots[handIndex] = true;

                if (_SnapParent.transform.childCount > 0) 
                    Destroy(_SnapParent.transform.GetChild(0).gameObject);

                transform.SetParent(_SnapParent.transform, true);
                GameManager.CardPlaced.Invoke();

                SetModePlayed();
            }
            else
            {
                transform.position = _SnapPos;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (GameManager.GetInstance().currentState == GameManager.State.MovingCard && _CurrentState != State.Played)
        {
            Vector3 positionSourisPixels = Input.mousePosition;
            transform.position = Camera.main.ScreenToWorldPoint(positionSourisPixels + new Vector3(0,0,2));
            _CurrentState = State.Moving;
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _Snapable = true;
        _SnapPos = collision.transform.position;
        _SnapParent = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _Snapable = false;

        if(_HandManager != null)
            _SnapPos = _HandManager._CardsSlot[handIndex].transform.position;
        
        _SnapParent = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _Snapable = true;
        _SnapPos = collision.transform.position;
        _SnapParent = collision.gameObject;
    }

    public void SetModeInHand()
    {
        _CurrentState = State.InHand;
        gameObject.SetActive(true);
    }

    public void SetModeMoving()
    {
        _CurrentState = State.Moving;
    }

    public void SetModePlayed()
    {
        _CurrentState |= State.Played;
        _Collider2D.enabled = false;

        OnPlaced?.Invoke();
        enabled = false;
    }
}
