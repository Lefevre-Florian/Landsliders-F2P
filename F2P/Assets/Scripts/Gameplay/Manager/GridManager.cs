using com.isartdigital.f2p.gameplay.card;

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

// Author (CR): Paul Vincencini
namespace com.isartdigital.f2p.gameplay.manager
{
    public class GridManager : MonoBehaviour
    {
        #region Singleton
        private static GridManager _Instance;

        public static GridManager GetInstance()
        {
            if(_Instance == null) 
                _Instance = new GridManager();
            return _Instance;
        }
        #endregion

        private void Awake()
        {
            if(_Instance != null)
            {
                DestroyImmediate(this);
                return;
            }
            _Instance = this;
        }

        private const string BIOMES_PATH = "Assets/Ressource/Prefab/Gameplay/Biomes";

        [Header("Grid Parameters")]

        [HideInInspector] public Vector2 _NumCard = Vector2.one * 3;
        [SerializeField] public Vector2 _GridSizePercent;
        [SerializeField] public Vector2 _Offset;

        [Header("GameObject")]

        [SerializeField] public GameObject _CardBackgroundPrefab;

        [HideInInspector] public Vector2 _ScreenSizeInGameUnit;
        [HideInInspector] public Vector2 _GridSize;
        [HideInInspector] public float _CardSize;
        [HideInInspector] public float _CardRatio;

        [HideInInspector] public GameObject[,] _Cards;

        [Header("Biomes")]
        [SerializeField] private Transform[] _BiomePrefabs = null;

        private void Start()
        {
            _Cards = new GameObject[(int)_NumCard.x, (int)_NumCard.y];

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

           if(_CardBackgroundPrefab == null)
           {
                Debug.Log("GridManager : Champ serialisé _CardBackgroundPrefab non assigné");
                return;
           }

            _ScreenSizeInGameUnit = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            _GridSize = _ScreenSizeInGameUnit * new Vector2(_GridSizePercent.x, _GridSizePercent.y);

            int lXArrayIndex = 0;
            for (int x = -1; x <= 1; x++)
            {
                float lXPos = _GridSize.x * x;

                int lYArrayIndex = 0;
                for (int y = -1; y <= 1; y++)
                {
                    float lYPos = _GridSize.y * y;

                    _Cards[lXArrayIndex, lYArrayIndex] = Instantiate(_CardBackgroundPrefab, new Vector3(lXPos + _Offset.x, lYPos + _Offset.y, 0), Quaternion.identity, transform);
                    CardContainer lCard = _Cards[lXArrayIndex,lYArrayIndex].GetComponent<CardContainer>();
                    lCard.gridPosition = new Vector2(lXArrayIndex, lYArrayIndex);
                    lYArrayIndex++;
                }
                lXArrayIndex++;
            }
        }

        #region Coordinates Utils
        /// <summary>
        /// Get the world position of an element based on the grid position
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public Vector2 GetIndexCoordonate(int pX, int pY)
        {
            if(pX > 2 || pX < 0 || pY > 2 || pY < 0)
            {
                Debug.Log("Les parametres rentré sont : pX = " + pX.ToString() + ", pY = " + pY.ToString() + "Cependant la fonction a pour intervale x[0,2] et y[0,2]");
                return Vector2.zero;
            }

            return new Vector2(_GridSize.x * (pX - 1) + _Offset.x, _GridSize.y * (pY - 1) + _Offset.y);
        }

        /// <summary>
        /// Get the index position of an element based on the world position
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public Vector2 GetGridCoordinate(int pX, int pY) => GetGridCoordinate(new Vector2(pX, pY));

        /// <summary>
        /// Get the index position of an element based on the world position
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public Vector2 GetGridCoordinate(Vector2 pWorldPosition)
        {
            return new Vector2((pWorldPosition.x - _Offset.x) / _GridSize.x,
                               (pWorldPosition.y - _Offset.y) / _GridSize.y);
        }

        /// <summary>
        /// Get the card at the specified position (in grid position)
        /// </summary>
        /// <param name="pPosition">Grid position format</param>
        /// <returns></returns>
        public GameObject GetCardByGridCoordinate(Vector2 pPosition) => _Cards[(int)pPosition.x, (int)pPosition.y];
        #endregion

        public Transform GetRandomBiome() => _BiomePrefabs[UnityEngine.Random.Range(0, _BiomePrefabs.Length)];

        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }
    }
}
