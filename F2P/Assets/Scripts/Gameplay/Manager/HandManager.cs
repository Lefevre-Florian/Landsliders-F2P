using Com.IsartDigital.F2P.UI.UIHUD;
using com.isartdigital.f2p.gameplay.quest;
using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P;

using UnityEngine;

using System;
using System.Collections.Generic;
using com.isartdigital.f2p.manager;


// Author (CR) : Elias Dridi
public class HandManager : MonoBehaviour
{
    #region Singleton
    private static HandManager _Instance = null;

    public static HandManager GetInstance()
    {
        if (_Instance == null) 
            _Instance = new HandManager();
        return _Instance;
    }
    #endregion

    #region Tracking
    private const string TRACKER_NAME = "biomeMostPlayed";

    private const string TRACKER_BIOME_TYPE_PARAMETER = "biomeType";
    #endregion

    private void Awake()
    {
        if (_Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        _Instance = this;
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

    public int _TotalCards { get { return _Deck.Length + _CardInHand; } }

    private List<Tuple<BiomeType, int>> _BiomePlayedTracking = new List<Tuple<BiomeType, int>>();
    
    private void Start()
    {
        CardSlot();
        CreateDeck();
        for (int i = 0; i < _StartingCardNb; i++)
        {
            DrawCard();
        }

        QuestManager.ValidQuest.AddListener(TrackWinCondition);
        GameManager.CardPlaced.AddListener(CardPlayedThenDraw);
    }

    public void TrackBiome(BiomeType pType)
    {
        int lIndex = _BiomePlayedTracking.FindIndex(x => x.Item1 == pType);
        if (lIndex == -1)
            _BiomePlayedTracking.Add(new Tuple<BiomeType, int>(pType, 1));
        else
            _BiomePlayedTracking[lIndex] = new Tuple<BiomeType, int>(pType, _BiomePlayedTracking[lIndex].Item2 + 1);
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
                    ltempCard.snapPos = _CardsSlot[i].transform.position;
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
            if (_CardInHand == 0)
                GameManager.GetInstance()
                           .SetModeGameover();
        }
    }

    public void BurnCard(int pNbCards = 1)
    {
        int lRemainingCardToRemove = pNbCards;
        if (lRemainingCardToRemove < _Deck.Length)
        {
            for (int i = 0; i < lRemainingCardToRemove; i++)
                Destroy(_DeckContainer.transform.GetChild(UnityEngine.Random.Range(0, _DeckContainer.transform.childCount)).gameObject);

            Array.Resize(ref _Deck, _Deck.Length - lRemainingCardToRemove);
        }
        else
        {
            if (_Deck.Length > 0)
            {
                int lLength = _DeckContainer.transform.childCount;
                for (int i = 0; i < lLength; i++)
                    Destroy(_DeckContainer.transform.GetChild(i).gameObject);
            }

            lRemainingCardToRemove -= _Deck.Length;
            _Deck = new GameObject[0];


            if (lRemainingCardToRemove > _CardInHand)
                GameManager.GetInstance().SetModeGameover();
            else
            {
                for (int i = 0; i < lRemainingCardToRemove; i++)
                    Destroy(_HandContainer.transform.GetChild(UnityEngine.Random.Range(0, _HandContainer.transform.childCount)).gameObject);
                _CardInHand -= lRemainingCardToRemove;
            }
        }
    }

    public void AddCardToDeck(int pNbCards)
    {
        int lStartIdx = _Deck.Length - 1;
        Array.Resize(ref _Deck, _Deck.Length + pNbCards);

        if (TryGetComponent<CardQuest>(out CardQuest cd)) cd.AddCard(pNbCards);

        int lLength = _Deck.Length;
        for (int i = lStartIdx; i < lLength; i++)
            _Deck[i] = CreateCard();
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
            _Deck[i] = CreateCard();
    }

    private GameObject CreateCard()
    {
        GameObject lCard = Instantiate(CardPrefabDic.GetRandomPrefab());
        lCard.GetComponent<TEMPCard>().enabled = true;
        lCard.SetActive(false);
        lCard.transform.SetParent(_DeckContainer.transform, true);

        return lCard;
    }

    private void CardSlot()
    {
        _CardsSlot = new GameObject[_MaxCardHold];
        _AvailableCardSlots = new bool[_MaxCardHold];
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

    private void TrackWinCondition()
    {
        int lMax = 0;
        int lLength = _BiomePlayedTracking.Count;

        BiomeType lMaxUsed = default;
        for (int i = 0; i < lLength; i++)
        {
            if (_BiomePlayedTracking[i].Item2 > lMax)
            {
                lMax = _BiomePlayedTracking[i].Item2;
                lMaxUsed = _BiomePlayedTracking[i].Item1;
            }
        }

        DataTracker.GetInstance().SendAnalytics(TRACKER_NAME, new Dictionary<string, object>() { { TRACKER_BIOME_TYPE_PARAMETER, lMaxUsed } });
    }

    private void OnDestroy()
    {
        if (_Instance == this)
        {

            _Instance = null;

            QuestManager.ValidQuest.RemoveListener(TrackWinCondition);
            GameManager.CardPlaced.RemoveListener(CardPlayedThenDraw);
        }
    }
}

