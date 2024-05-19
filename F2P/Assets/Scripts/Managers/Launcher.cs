using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.FileSystem;
using Com.IsartDigital.F2P.UI.Screens;

using UnityEngine;
using UnityEngine.SceneManagement;

// Author (CR) : Lefevre Florian
namespace com.isartdigital.f2p.manager
{
    public class Launcher : MonoBehaviour
    {
        [Header("Utils")]
        [SerializeField] private SplashScreen _SplashScreen = null;

        [Header("Game flow")]
        [SerializeField] private int _SCNFtueIdx = 0;
        [SerializeField] private int _SCNGameIdx = 0;

        [Header("Application management")]
        [SerializeField][Range(15, 120)] private int _TargetedFrameRate = 30;

        // Variables
        private DatabaseManager _Database = null;

        private bool _ResourceLoadingCompleted = false;
        private bool _SplashArtCompleted = false;

        private void Start()
        {
            _Database = DatabaseManager.GetInstance();
            _Database.OnResourcesLoaded += LoadResourceComplete;
            _SplashScreen.OnFinished += SplashAnimationComplete;

            Application.targetFrameRate = _TargetedFrameRate;
        }

        private void LoadResourceComplete()
        {
            _ResourceLoadingCompleted = true;
            LaunchGame();
        }

        private void SplashAnimationComplete()
        {
            _SplashArtCompleted = true;
            LaunchGame();
        }

        private void LaunchGame()
        {
            if(_ResourceLoadingCompleted && _SplashArtCompleted)
                SceneManager.LoadScene(Save.data.ftuecomplete ? _SCNGameIdx : _SCNFtueIdx); 
        }

        private void OnDestroy()
        {
            if (_Database != null)
                _Database.OnResourcesLoaded -= LoadResourceComplete;

            if (_SplashScreen != null)
                _SplashScreen.OnFinished -= SplashAnimationComplete;

            _SplashScreen = null;
            _Database = null;
        }
    }
}
