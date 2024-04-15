using com.isartdigital.f2p.gameplay.card;
using Com.IsartDigital.F2P.Biomes;
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
        private static GridManager _Instance = null;

        public static GridManager GetInstance()
        {
            if(_Instance == null) 
                _Instance = new GridManager();
            return _Instance;
        }
        #endregion

        private const string BIOMES_PATH = "Assets/Ressource/Prefab/Gameplay/Biomes";

        [Header("Grid Parameters")]

        [HideInInspector] public Vector2 _NumCard = Vector2.one * 3;
        [SerializeField] public Vector2 _GridSizePercent;
        [SerializeField] public Vector2 _Offset;

        [Space(2)]
        [SerializeField] private bool _IsGridPredefined = false;

        [Header("GameObject")]

        [SerializeField] public GameObject _CardBackgroundPrefab;

        [HideInInspector] public Vector2 _ScreenSizeInGameUnit;
        [HideInInspector] public Vector2 _GridSize;
        [HideInInspector] public float _CardSize;
        [HideInInspector] public float _CardRatio;

        [HideInInspector] public GameObject[,] _Cards;

        [Header("Biomes")]
        [SerializeField] private Transform[] _BiomePrefabs = null;

        private void Awake()
        {
            if (_Instance != null)
            {
                DestroyImmediate(this);
                return;
            }
            _Instance = this;

            _Cards = new GameObject[(int)_NumCard.x, (int)_NumCard.y];

            _ScreenSizeInGameUnit = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            _GridSize = _ScreenSizeInGameUnit * new Vector2(_GridSizePercent.x, _GridSizePercent.y);

            if (_IsGridPredefined)
            {
                CardContainer lContainer = null;
                int lLinearIdx = 0;
                for (int i = 0; i < (int)_NumCard.x; i++)
                {
                    for (int j = 0; j < (int)_NumCard.y; j++)
                    {
                        lContainer = transform.GetChild(lLinearIdx).GetComponent<CardContainer>();
                        lContainer.gridPosition = new Vector2(i, j);

                        _Cards[i, j] = lContainer.transform.GetChild(0).gameObject;

                        lLinearIdx++;
                    }
                }
            }
            else
            {
                foreach (Transform child in transform)
                    Destroy(child.gameObject);

                if (_CardBackgroundPrefab == null)
                {
                    Debug.LogError("GridManager : Champ serialisé _CardBackgroundPrefab non assigné");
                    return;
                }

                int lXArrayIndex = 0;
                for (int x = -1; x <= 1; x++)
                {
                    float lXPos = _GridSize.x * x;

                    int lYArrayIndex = 0;
                    for (int y = -1; y <= 1; y++)
                    {
                        float lYPos = _GridSize.y * y;

                        _Cards[lXArrayIndex, lYArrayIndex] = Instantiate(_CardBackgroundPrefab, new Vector3(lXPos + _Offset.x, lYPos + _Offset.y, 0), Quaternion.identity, transform);
                        CardContainer lCard = _Cards[lXArrayIndex, lYArrayIndex].GetComponent<CardContainer>();
                        lCard.gridPosition = new Vector2(lXArrayIndex, lYArrayIndex);
                        lYArrayIndex++;
                    }
                    lXArrayIndex++;
                }
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
            for (int i = 0; i < _NumCard.x; i++)
            {
                for (int j = 0; j < _NumCard.y; j++)
                {
                    if (pWorldPosition == (Vector2)_Cards[i, j].transform.position)
                        return new Vector2(i, j);
                }
            }
            return Vector2.one * -1f;
            /* return new Vector2((int)((pWorldPosition.x - _Offset.x / _GridSize.x),
                                (int)((pWorldPosition.y - _Offset.y / _GridSize.y));*/
        }

        /// <summary>
        /// Get the card at the specified position (in grid position)
        /// </summary>
        /// <param name="pPosition">Grid position format</param>
        /// <returns></returns>
        public Biome GetCardByGridCoordinate(Vector2 pPosition) => _Cards[(int)pPosition.x, (int)pPosition.y].GetComponent<Biome>();
        #endregion

        #region Biome related
        public void ReplaceAtIndex(Vector2 pGridPosition, Transform pTransform) 
        {
            int x = (int)pGridPosition.x;
            int y = (int)pGridPosition.y;

            Biome lBiome = Instantiate(pTransform, _Cards[x, y].transform.parent).GetComponent<Biome>();
            _Cards[x, y] = lBiome.gameObject;
        }

        public void RemoveAtIndex(Vector2 pGridPosition) => _Cards[(int)pGridPosition.x, (int)pGridPosition.y] = null;

        public Transform GetRandomBiome() => _BiomePrefabs[UnityEngine.Random.Range(0, _BiomePrefabs.Length)];
        #endregion

        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }
    }
}
