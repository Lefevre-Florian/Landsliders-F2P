using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.IsartDigital.F2P.UI
{
    public class LoadManager : MonoBehaviour
    {
        #region Singleton
        private static LoadManager _Instance = null;

        public static LoadManager GetInstance()
        {
            if(_Instance == null) 
				_Instance = new LoadManager();
            return _Instance;
        }

        private LoadManager() : base() {}
        #endregion
        
        // const
        private const float SCENE_LOADING_PERCENT = .9f;    // must be inferior or equal to 0.9
        private const float FLOAT_TO_PERCENT = 100f;

        [Header("Prefabs")]
        [SerializeField] private GameObject _LoadingScreenPrefab;

        // Variables
        private Coroutine _LoadingTimer = null;

        // Events
        public event Action<float> OnLoadingProgressed;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }

        public void StartLoading(int pScnIDX)
        {
            if (_LoadingTimer == null)
                _LoadingTimer = StartCoroutine(LoadingScene(pScnIDX));   
        }

        /// <summary>
        /// Loading screen renderer and logic computation
        /// </summary>
        /// <param name="pScnIDX"></param>
        /// <returns></returns>
        private IEnumerator LoadingScene(int pScnIDX)
        {
            GameObject lLoadingScreen = Instantiate(_LoadingScreenPrefab, transform.parent);

            AsyncOperation lAsyncLoader = SceneManager.LoadSceneAsync(pScnIDX);
            lAsyncLoader.allowSceneActivation = false;

            while (!lAsyncLoader.isDone)
            {
                if (lAsyncLoader.progress >= SCENE_LOADING_PERCENT)
                {
                    OnLoadingProgressed?.Invoke(1f * FLOAT_TO_PERCENT);
                    lAsyncLoader.allowSceneActivation = true;

                    ClearCoroutine();
                }

                OnLoadingProgressed?.Invoke(lAsyncLoader.progress * FLOAT_TO_PERCENT);
                yield return null;
            }
        }

        private void ClearCoroutine()
        {
            if(_LoadingTimer != null)
            {
                StopCoroutine(_LoadingTimer);
                _LoadingTimer = null;
            }
        }

        private void OnDestroy()
        {
            // Security & Cleaning
            ClearCoroutine();

            if (_Instance == this)
                _Instance = null;
        }

    }
}
