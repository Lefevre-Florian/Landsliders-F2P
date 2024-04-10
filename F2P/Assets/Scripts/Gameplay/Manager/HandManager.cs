using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.UI.UIHUD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    private static HandManager instance;

    public static HandManager GetInstance()
    {
        if (instance == null) instance = new HandManager();
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;
    }

    [Header("Card Parameters")]

    [SerializeField] private int _MaxCardHold = 4;
    [SerializeField] private int _StartingCardNb = 4;
    [HideInInspector] public int _CardInHand = 0;
    
    [Header("Grid Parameters")]


    [SerializeField] public Vector2 _GridSizePercent;
    [SerializeField] public Vector2 _Offset;

    [Header("GameObject")]

    [SerializeField] public GameObject _CardSlotPrefab;
    [SerializeField] public GameObject _CardPrefab;

    [Header("Container")]

    [SerializeField] private GameObject _DeckContainer;
    [SerializeField] private GameObject _HandContainer;

    [HideInInspector] public Vector2 _ScreenSizeInGameUnit;
    [HideInInspector] public Vector2 _GridSize;

    [HideInInspector] public GameObject[] _CardsSlot;
    [HideInInspector] public bool[] _AvailableCardSlots;
    private GameObject[] _Deck;

    
    private void Start()
    {
        CardSlot();
        CreateDeck();
        for (int i = 0; i < _StartingCardNb; i++)
        {
            DrawCard();
        }
        GameManager.CardPlaced.AddListener(CardPlayedThenDraw);
    }

    public void DrawCard()
    {
        if (_Deck.Length >= 1)
        {
            for (int i = 0; i < _AvailableCardSlots.Length; i++)
            {
                if (_AvailableCardSlots[i] == true)
                {
                    GameObject lcardGO = _Deck[0];
                    lcardGO.transform.SetParent(_HandContainer.transform, true);
                    lcardGO.transform.position = _CardsSlot[i].transform.position;
                    TEMPCard ltempCard = lcardGO.GetComponent<TEMPCard>();
                    ltempCard.handIndex = i;
                    ltempCard.SetModeInHand();
                    RemoveAtDeck(0);
                    _AvailableCardSlots[i] = false;
                    _CardInHand++;
                    return;
                }
            }
        }
        else
        {
            if (_CardInHand == 0) Hud.GetInstance().Lose();
        }
    }

    public void RemoveAtDeck(int index)
    {
        Array.Copy(_Deck, index + 1, _Deck, index, _Deck.Length - index - 1);
        Array.Resize(ref _Deck, _Deck.Length - 1);
    }

    private void CreateDeck()
    {
        _Deck = new GameObject[GameManager.GetInstance().cardStocked];
        for (int i = 0; i < _Deck.Length; i++)
        {
            _Deck[i] = Instantiate(_CardPrefab);
            _Deck[i].gameObject.SetActive(false);
            _Deck[i].transform.SetParent(_DeckContainer.transform, true);
        }
    }

    private void CardSlot()
    {
        _CardsSlot = new GameObject[_MaxCardHold];
        _AvailableCardSlots = new bool [_MaxCardHold];
        for (int i = 0; i < _AvailableCardSlots.Length; i++)
        {
            _AvailableCardSlots[i] = true;
        }

        if (_CardSlotPrefab == null)
        {
            Debug.Log("GridManager : Champ serialisé _CardPrefab non assigné");
            return;
        }

        _ScreenSizeInGameUnit = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        _GridSize = _ScreenSizeInGameUnit * new Vector2(_GridSizePercent.x, _GridSizePercent.y);

        int lXArrayIndex = 0;

        for (int x = -1; x <= 2; x++)
        {
            float lXPos = _GridSize.x * x;

            _CardsSlot[lXArrayIndex] = Instantiate(_CardSlotPrefab, new Vector3(lXPos + _Offset.x, 0 + _Offset.y, 0), Quaternion.identity, transform);

            lXArrayIndex++;
        }
    }

    public void CardPlayedThenDraw()
    {
        _CardInHand--;
        DrawCard();
    }


    private void OnDestroy()
    {
        if (instance != this) return;
        instance = null;
        GameManager.CardPlaced.RemoveListener(CardPlayedThenDraw);


    }
}

