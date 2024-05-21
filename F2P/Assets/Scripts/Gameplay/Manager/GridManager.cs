using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.quest;
using com.isartdigital.f2p.manager;
using Com.IsartDigital.F2P.Biomes;

using System.Collections.Generic;

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

        private const string ERR_CARDBACKGROUNDPREFAB_SERIALIZED = "GridManager : Champ serialisé _CardBackgroundPrefab non assigné";

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
        private const string CARDPLAYED_TAG = "CardPlayed";

        [HideInInspector] public GameObject[,] _Cards = new GameObject[3,3];

        // Get / Set 
        public List<Biome> Biomes {
            get
            {
                List<Biome> lBiomes = new List<Biome>();
                for (int i = 0; i < (int)_NumCard.x; i++)
                {
                    for (int j = 0; j < (int)_NumCard.y; j++)
                        lBiomes.Add(_Cards[i, j].GetComponent<Biome>());
                }
                return lBiomes;
            }
        }

        private void Awake()
        {
            if (_Instance != null)
            {
                DestroyImmediate(this);
                return;
            }
            _Instance = this;

            GameFlowManager.InitGrid.AddListener(Init);
        }

        public void Init()
        {
            _ScreenSizeInGameUnit = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            _GridSize = _ScreenSizeInGameUnit * new Vector2(_GridSizePercent.x, _GridSizePercent.y);

            foreach (Transform child in transform)
                Destroy(child.gameObject);

            if (_CardBackgroundPrefab == null)
            {
                Debug.LogError(ERR_CARDBACKGROUNDPREFAB_SERIALIZED);
                return;
            }

            TEMPCard lCard = null;
            int lXArrayIndex = 0;
            for (int x = -1; x <= 1; x++)
            {
                float lXPos = _GridSize.x * x;

                int lYArrayIndex = 0;
                for (int y = -1; y <= 1; y++)
                {
                    float lYPos = _GridSize.y * y;

                    GameObject lPrefabToInstantiate = _Cards[lXArrayIndex, lYArrayIndex] == null ? CardPrefabDic.GetRandomPrefab(CardPrefabDic.prefabMapList, (int)QuestManager.currentQuest) : _Cards[lXArrayIndex, lYArrayIndex];

                    _Cards[lXArrayIndex, lYArrayIndex] = Instantiate( lPrefabToInstantiate,
                                                                     new Vector3(lXPos + _Offset.x, lYPos + _Offset.y, 0),
                                                                     Quaternion.identity,
                                                                     transform);

                    CardContainer lContainer = _Cards[lXArrayIndex, lYArrayIndex].GetComponent<CardContainer>();
                    lContainer.gridPosition = new Vector2(lXArrayIndex, lYArrayIndex);

                    lCard = _Cards[lXArrayIndex, lYArrayIndex].GetComponent<TEMPCard>();

                    lCard.SetModePlayed();

                    lCard.currentState = TEMPCard.State.Played;
                    lCard.GetComponent<TEMPCard>().enabled = false;
                    
                    _Cards[lXArrayIndex, lYArrayIndex].tag = CARDPLAYED_TAG;

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
        public Vector2 GetWorldCoordinate(int pX, int pY)
        {
            if(pX > 2 || pX < 0 || pY > 2 || pY < 0)
            {
                Debug.Log("Les parametres rentré sont : pX = " + pX.ToString() + ", pY = " + pY.ToString() + "Cependant la fonction a pour intervale x[0,2] et y[0,2]");
                return Vector2.zero;
            }

            return new Vector2(_GridSize.x * (pX - 1) + _Offset.x, _GridSize.y * (pY - 1) + _Offset.y);
        }

        /// <summary>
        /// Get the world position of an element based on the grid position
        /// </summary>
        /// <param name="pGridPosition"></param>
        /// <returns></returns>
        public Vector2 GetWorldCoordinate(Vector2 pGridPosition) => GetWorldCoordinate((int)pGridPosition.x, (int)pGridPosition.y);

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
            return new Vector2((int)((pWorldPosition.x - _Offset.x + _GridSize.x) / _GridSize.x), 
                               (int)((pWorldPosition.y - _Offset.y + _GridSize.y) / _GridSize.y));
        }

        /// <summary>
        /// Get the card at the specified position (in grid position)
        /// </summary>
        /// <param name="pPosition">Grid position format</param>
        /// <returns></returns>
        public Biome GetCardByGridCoordinate(Vector2 pPosition) => _Cards[(int)pPosition.x, (int)pPosition.y]?.GetComponent<Biome>();
        #endregion

        #region Grid management
        public void ReplaceAtIndex(Vector2 pGridPosition, Transform pTransform) 
        {
            int x = (int)pGridPosition.x;
            int y = (int)pGridPosition.y;

            Vector3 lWorldPosition = _Cards[x, y].transform.position;

            Biome lBiome = Instantiate(pTransform, _Cards[x, y].transform.parent).GetComponent<Biome>();
            TEMPCard lCard = lBiome.GetComponent<TEMPCard>();
            
            lCard.SetModePlayed();

            lCard.tag = CARDPLAYED_TAG;
            lCard.currentState = TEMPCard.State.Played;
            lCard.enabled = false;

            lCard.GetComponent<CardContainer>().gridPosition = new Vector2(x, y);

            if (_Cards[x, y].GetComponent<Biome>().Type == BiomeType.Canyon) CanyonQuest.ValidSignal.Invoke();
            _Cards[x, y].GetComponent<Biome>().Remove();
            _Cards[x, y] = lBiome.gameObject;
            _Cards[x, y].transform.position = lWorldPosition;
        }

        public void RemoveAtIndex(Vector2 pGridPosition) => _Cards[(int)pGridPosition.x, (int)pGridPosition.y] = null;
        #endregion

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _Instance = null;
                GameFlowManager.InitGrid.RemoveListener(Init);
            }
        }
    }
}
