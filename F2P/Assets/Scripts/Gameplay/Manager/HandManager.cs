using com.isartdigital.f2p.gameplay.manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

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
    [SerializeField] private int _MaxCardHold = 4;
    [SerializeField] private int _StartingCardNb = 4;
    [Header("Grid Parameters")]


    [SerializeField] public Vector2 _GridSizePercent;
    [SerializeField] public Vector2 _Offset;

    [Header("GameObject")]

    [SerializeField] public GameObject _CardSlotPrefab;
    [SerializeField] public GameObject _CardPrefab;


    [HideInInspector] public Vector2 _ScreenSizeInGameUnit;
    [HideInInspector] public Vector2 _GridSize;

    [HideInInspector] public GameObject[] _CardsSlot;

    [HideInInspector] public bool[] _AvailableCardSlots;

    private void Start()
    {
        CardSlot();
        for (int i = 0; i < _StartingCardNb; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
            for (int i = 0; i < _AvailableCardSlots.Length; i++)
            {
                if (_AvailableCardSlots[i] == true)
                {
                    GameObject lcardGO = Instantiate(_CardPrefab, _CardsSlot[i].transform);
                    TEMPCard ltempCard = lcardGO.GetComponent<TEMPCard>();
                    ltempCard.handIndex = i;
                    _AvailableCardSlots[i] = false;
                    return;
                }
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

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
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
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}

