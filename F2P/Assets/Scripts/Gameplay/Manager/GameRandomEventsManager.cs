using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Gameplay.Manager
{
    public enum GameEventType { Etheral_Rift = 0, Dragon_Lair = 1, Witch = 2, Wisp = 3, Goblin_Treasure = 4, Mist = 5}
    public class GameRandomEventsManager : MonoBehaviour
    {
        #region Singleton
        private static GameRandomEventsManager _Instance = null;

        public static GameRandomEventsManager GetInstance()
        {
            if(_Instance == null) 
				_Instance = new GameRandomEventsManager();
            return _Instance;
        }

        private GameRandomEventsManager() : base() {}
        #endregion

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }

        [Header("Max of on board events")]
        [SerializeField] private int _MaxGameEventsNumber = 3;
        private int _GameEventsCount = 0;

        public int GameEventCount { get { return _GameEventsCount; } set { _GameEventsCount = value; } }


        [Header("Chance of event on card played")]
        [SerializeField][Range(0f,1f)]private float _GameEventChance = 0.3f;
        private float _RandomValue;

        [SerializeField] private int _MaxEtheralRiftsNumber = 4;
        [SerializeField] private int _MaxDragonLairsNumber = 2;
        [SerializeField] private int _MaxWitchesNumber = 2;
        [SerializeField] private int _MaxWispsNumber = 2;
        [SerializeField] private int _MaxGoblinTreasuresNumber = 2;
        [SerializeField] private int _MaxMistsNumber = 2;

        [SerializeField] private GameObject _EtheralRift;
        [SerializeField] private GameObject _DragonLair;
        [SerializeField] private GameObject _Witch;
        [SerializeField] private GameObject _Wisp;
        [SerializeField] private GameObject _GoblinTreasure;
        [SerializeField] private GameObject _Mist;

        private List<GameEventType> _Eventdeck = new List<GameEventType>();

        private GameEventType _GameEventSelected;

        private GameObject _InstantiatedGameEvent;

        private void Start()
        {
            AddToEventDeckList(_MaxEtheralRiftsNumber, GameEventType.Etheral_Rift);
            AddToEventDeckList(_MaxDragonLairsNumber, GameEventType.Dragon_Lair);
            AddToEventDeckList(_MaxWitchesNumber, GameEventType.Witch);
            AddToEventDeckList(_MaxWispsNumber, GameEventType.Wisp);
            AddToEventDeckList(_MaxGoblinTreasuresNumber, GameEventType.Goblin_Treasure);
            AddToEventDeckList(_MaxMistsNumber, GameEventType.Mist);

            for (int i = _Eventdeck.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                GameEventType lGameEventType = _Eventdeck[i];
                _Eventdeck[i] = _Eventdeck[j];
                _Eventdeck[j] = lGameEventType;
            }

            GameManager.CardPlaced.AddListener(OnCardPlaced);
        }

        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;

            GameManager.CardPlaced.RemoveListener(OnCardPlaced);
        }

        private void AddToEventDeckList(int pInt, GameEventType pGameEventType)
        {
            for (int i = 0; i < pInt; i++)
            {
                _Eventdeck.Add(pGameEventType);
            }
        }

        private void OnCardPlaced()
        {
            if (_GameEventsCount < _MaxGameEventsNumber && _Eventdeck.Count > 0)
            {
                _RandomValue = UnityEngine.Random.value;

                if (_RandomValue < _GameEventChance)
                {
                    _GameEventSelected = _Eventdeck[_Eventdeck.Count - 1];
                    _Eventdeck.RemoveAt(_Eventdeck.Count - 1);
                    InstantiateGameEvent(_GameEventSelected);
                    _GameEventsCount += 1;
                }
            }
        }

        private GameObject InstantiateGameEvent(GameEventType pGameEvenType)
        {
            switch (pGameEvenType)
            {
                case GameEventType.Etheral_Rift:
                    return PlaceGameEvent(_EtheralRift);
                case GameEventType.Dragon_Lair:
                    return PlaceGameEvent(_DragonLair);
                case GameEventType.Witch:
                    return PlaceGameEvent(_Witch);
                case GameEventType.Wisp:
                    return PlaceGameEvent(_Wisp);
                case GameEventType.Goblin_Treasure:
                    return PlaceGameEvent(_GoblinTreasure);
                case GameEventType.Mist:
                    return PlaceGameEvent(_Mist);
                default:
                    return null;
            }
        }

        private GameObject PlaceGameEvent(GameObject pGameObject)
        {
            _InstantiatedGameEvent = Instantiate(pGameObject);
            _InstantiatedGameEvent.transform.SetParent(GameManager.GetInstance()._LastCardPlayed.transform);
            _InstantiatedGameEvent.transform.localPosition = Vector3.back;
            return _InstantiatedGameEvent;
        }
    }
}
