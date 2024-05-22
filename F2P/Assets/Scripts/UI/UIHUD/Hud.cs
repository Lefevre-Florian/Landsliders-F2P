using UnityEngine;
using UnityEngine.SceneManagement;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.UIHUD
{
    public class Hud : MonoBehaviour
    {
        #region Singleton
        private static Hud _Instance = null;

        public static Hud GetInstance()
        {
            if(_Instance == null) 
				_Instance = new Hud();
            return _Instance;
        }

        private Hud() : base() {}
        #endregion

        [Header("Sub-Button")]
        [SerializeField] private GameObject _PauseButton = null;
        [Header("Sub-screens")]
        [SerializeField] private Transform _PauseScreen = null;
        [SerializeField] private Transform _LoseScreen = null;
        [SerializeField] private Transform _WinScreen = null;

        [Header("Sign")]
        [SerializeField] private GameObject _HandTurnMask = null;
        [SerializeField] private GameObject _PlayerTurnMask = null;

        [Header("Scene management")]
        [SerializeField] private int _MainMenuIDX = 0;
        [SerializeField] private bool _UseLoadingScreen = false;

        // Variables
        private GameManager _GameManager = null;

        private GameObject _CurrentActiveLayer = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }

        private void Start()
        {
            _GameManager = GameManager.GetInstance();
            _GameManager.OnGameover += DisplayGameEndPanel;

            _PauseScreen.gameObject.SetActive(false);

            // Flow (Renderer)
            SwitchToCardMode();

            _GameManager.OnAllEffectPlayed += SwitchToCardMode;
            GameManager.CardPlaced.AddListener(SwitchToMoveMode);
        }

        public void Pause()
        {
            GameFlowManager.Paused?.Invoke();

            Time.timeScale = 0f;
            _PauseScreen.gameObject.SetActive(true);
        }

        public void Resume()
        {
            GameFlowManager.Resumed?.Invoke();

            Time.timeScale = 1f;
            _PauseScreen.gameObject.SetActive(false);
        }

        public void Retry() => LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void MainMenu() => LoadScene(_MainMenuIDX);

        private void DisplayGameEndPanel(bool pStatus)
        {
            _PauseButton.SetActive(false);

            if (pStatus)
                _WinScreen.gameObject.SetActive(true);
            else
                _LoseScreen.gameObject.SetActive(true);
        }

        private void LoadScene(int pSceneIDX)
        {
            Time.timeScale = 1f;
            if (!_UseLoadingScreen)
                SceneManager.LoadScene(pSceneIDX);
            else
                LoadManager.GetInstance().StartLoading(pSceneIDX);
        }

        /// Rendering turn phases
        private void SwitchToMoveMode()
        {
            TEMPCard.OnFocus -= SwitchCurrentLayerState;

            SwitchLayerState(_HandTurnMask, true);
            SwitchLayerState(_PlayerTurnMask, false);

            _CurrentActiveLayer = _HandTurnMask;
        }

        private void SwitchToCardMode()
        {
            SwitchLayerState(_HandTurnMask, false);
            SwitchLayerState(_PlayerTurnMask, true);

            _CurrentActiveLayer = _PlayerTurnMask;
            TEMPCard.OnFocus += SwitchCurrentLayerState;
        }


        private void SwitchLayerState(GameObject pLayer, bool pState) => pLayer.SetActive(pState);

        private void SwitchCurrentLayerState(bool pState) => _CurrentActiveLayer.SetActive(!pState);

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _Instance = null;

                _GameManager.OnGameover -= DisplayGameEndPanel;
                _GameManager.OnAllEffectPlayed -= SwitchToCardMode;
                _GameManager = null;

                GameManager.CardPlaced.RemoveListener(SwitchToMoveMode);
                TEMPCard.OnFocus -= SwitchCurrentLayerState;
            }
        }
    }
}
