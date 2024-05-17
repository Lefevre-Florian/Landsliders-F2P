using System;
using System.Collections;
using System.Collections.Generic;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Manager
{
    public enum GameEventType { Etheral_Rift = 0, Dragon_Lair = 1, Witch = 2, Wisp = 3, Goblin_Treasure = 4, Mist = 5}

    [System.Serializable]
    public struct WeightedGameEvent
    {
        public GameEventType gameEvent;
        public float weight;
    }

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

        [SerializeField] private GameObject _EtheralRift;
        [SerializeField] private GameObject _DragonLair;
        [SerializeField] private GameObject _Witch;
        [SerializeField] private GameObject _Wisp;
        [SerializeField] private GameObject _GoblinTreasure;
        [SerializeField] private GameObject _Mist;

        private GameEventType _GameEventSelected;

        private GameObject _InstantiatedGameEvent;

        [SerializeField] private GameObject _GridContainer;
        private List<GameObject> _StartGridList = new List<GameObject>();

        public List<WeightedGameEvent> _Events = new List<WeightedGameEvent>();

        private void Start()
        {
            StartCoroutine(OnStart());

            GameManager.CardPlaced.AddListener(OnCardPlaced);
        }

        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;

            GameManager.CardPlaced.RemoveListener(OnCardPlaced);
        }

        private void OnCardPlaced()
        {
            if (_GameEventsCount < _MaxGameEventsNumber)
            {
                _RandomValue = UnityEngine.Random.value;

                if (_RandomValue < _GameEventChance)
                {
                    _GameEventSelected = GetRandomEvent(_Events);

                    InstantiateGameEvent(_GameEventSelected, GameManager.GetInstance()._LastCardPlayed.transform);
                    _GameEventsCount += 1;
                }
            }
        }

        private GameEventType GetRandomEvent(List<WeightedGameEvent> pWeightedList)
        {
            float lTotalWeight = 0f;

            foreach (WeightedGameEvent lWeighted in pWeightedList)
            {
                lTotalWeight += lWeighted.weight;
            }

            float lRandomValue = UnityEngine.Random.Range(0, lTotalWeight);

            foreach (WeightedGameEvent lWeighted in pWeightedList)
            {
                if (lRandomValue < lWeighted.weight)
                {
                    return lWeighted.gameEvent;
                }

                lRandomValue -= lWeighted.weight;
            }

            return default(GameEventType);
        }

        private GameObject InstantiateGameEvent(GameEventType pGameEvenType, Transform pParent)
        {
            switch (pGameEvenType)
            {
                case GameEventType.Etheral_Rift:
                    return PlaceGameEvent(_EtheralRift, pParent);
                case GameEventType.Dragon_Lair:
                    return PlaceGameEvent(_DragonLair, pParent);
                case GameEventType.Witch:
                    return PlaceGameEvent(_Witch, pParent);
                case GameEventType.Wisp:
                    return PlaceGameEvent(_Wisp, pParent);
                case GameEventType.Goblin_Treasure:
                    return PlaceGameEvent(_GoblinTreasure, pParent);
                case GameEventType.Mist:
                    return PlaceGameEvent(_Mist, pParent);
                default:
                    return null;
            }
        }

        private GameObject PlaceGameEvent(GameObject pGameObject, Transform pParent)
        {
            _InstantiatedGameEvent = Instantiate(pGameObject);
            _InstantiatedGameEvent.transform.SetParent(pParent);
            _InstantiatedGameEvent.transform.localPosition = Vector3.back;
            return _InstantiatedGameEvent;
        }

        private List<GameObject> GetAllChildren(GameObject pParent)
        {
            List<GameObject> lChildren = new List<GameObject>();

            for (int i = 0; i < pParent.transform.childCount; i++)
            {
                if (Player.GetInstance().transform.position != pParent.transform.GetChild(i).transform.position)
                {
                    lChildren.Add(pParent.transform.GetChild(i).gameObject);
                }
            }

            return lChildren;
        }

        private IEnumerator OnStart()
        {
            yield return new WaitForEndOfFrame();

            _StartGridList = GetAllChildren(_GridContainer);

            for (int i = _StartGridList.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                GameObject lObject = _StartGridList[i];
                _StartGridList[i] = _StartGridList[j];
                _StartGridList[j] = lObject;
            }

            foreach (GameObject lGameObject in _StartGridList)
            {
                if (_GameEventsCount < _MaxGameEventsNumber)
                {
                    _RandomValue = UnityEngine.Random.value;

                    if (_RandomValue < _GameEventChance)
                    {
                        _GameEventSelected = GetRandomEvent(_Events);

                        InstantiateGameEvent(_GameEventSelected, lGameObject.transform);
                        _GameEventsCount += 1;
                    }
                }
            }
        }
    }
}
