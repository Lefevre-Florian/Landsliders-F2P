using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPCard : MonoBehaviour
{
    private SpriteRenderer rend;
    private Color newColor;
    public int handIndex;
    private GameManager _GameManager;
    private bool _Snapable;
    private Vector3 _SnapPos;
    private GameObject _SnapParent;
    void Start()
    {
        _GameManager = FindObjectOfType<GameManager>();
        rend = GetComponentInChildren<SpriteRenderer>();
        RandomColor();
        rend.color = newColor;
    }

    private void RandomColor()
    {
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;

        newColor = new Color(r, g, b,255);
    }

    private void OnMouseUp()
    {
        if (!_GameManager.cardPlayed)
        {
            if(_Snapable)
            {
                transform.position = _SnapPos;
                HandManager.GetInstance()._AvailableCardSlots[handIndex] = true;
                _GameManager.cardPlayed = true;
                if (_SnapParent.transform.childCount > 0) 
                {
                    Destroy(_SnapParent.transform.GetChild(0).gameObject);
                }
                transform.SetParent(_SnapParent.transform, true);
                Destroy(this);
                HandManager.GetInstance()._CardInHand--;
                HandManager.GetInstance().DrawCard();
                
            }
                
            else
            {
                transform.position = _SnapPos;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (!_GameManager.cardPlayed)
        {
            Vector3 positionSourisPixels = Input.mousePosition;
            transform.position = Camera.main.ScreenToWorldPoint(positionSourisPixels + new Vector3(0,0,2));
            
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
        _SnapPos = HandManager.GetInstance()._CardsSlot[handIndex].transform.position;
        _SnapParent = null;
       

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _Snapable = true;
        _SnapPos = collision.transform.position;
        _SnapParent = collision.gameObject;
    }




}
