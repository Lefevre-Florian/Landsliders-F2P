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

        [Header("Sub-screens")]
        [SerializeField] private Transform _PauseScreen = null;

        [Header("Scene management")]
        [SerializeField] private int _MainMenuIDX = 0;
        [SerializeField] private bool _UseLoadingScreen = false;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }

        private void Start() => _PauseScreen.gameObject.SetActive(false);

        public void Pause()
        {
            Time.timeScale = 0f;
            _PauseScreen.gameObject.SetActive(true);
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            _PauseScreen.gameObject.SetActive(false);
        }

        public void Retry() => LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void MainMenu() => LoadScene(_MainMenuIDX);

        private void LoadScene(int pSceneIDX)
        {
            Time.timeScale = 1f;
            if (!_UseLoadingScreen)
                SceneManager.LoadScene(pSceneIDX);
            else
                LoadManager.GetInstance().StartLoading(pSceneIDX);
        }

        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }

    }
}
