using FMODUnity;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Sound
{
    public class SoundManager : MonoBehaviour
    {
        #region Singleton
        private static SoundManager _Instance = null;

        public static SoundManager GetInstance()
        {
            if(_Instance == null) 
				_Instance = new SoundManager();
            return _Instance;
        }

        private SoundManager() : base() {}
        #endregion

        [Header("Banks")]
        [SerializeField] private FMODBank[] _AlwaysLoadedBank = default;
        [SerializeField] private FMODBank[] _Banks = default;

        // Variables
        private List<FMODBank> _LoadedBanks = new List<FMODBank>();

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            int lLength = _AlwaysLoadedBank.Length;
            for (int i = 0; i < lLength; i++)
                RuntimeManager.LoadBank(_AlwaysLoadedBank[i].path);

            int lID = 0;
            lLength = _Banks.Length;
            for (int i = 0; i < lLength; i++)
            {
                lID++;
                _Banks[i].SetBankID(lID);
                RuntimeManager.UnloadBank(_Banks[i].path);
            }

            SceneManager.activeSceneChanged += LoadSceneBank;
        }

        private void LoadSceneBank(Scene pOldScene, Scene pNextScene)
        {
            List<FMODBank> lBanks = _Banks.ToList<FMODBank>();

            UnloadBankGroup(lBanks.FindAll(x => x.SceneLinked == pOldScene.buildIndex).ToArray());
            LoadBankGroup(lBanks.FindAll(x => x.SceneLinked == pNextScene.buildIndex).ToArray());
        }

        public void LoadSpecificBank(FMODBank pBank)
        {
            if (_Banks.Length <= 0)
                return;

            int lIdx = _Banks.ToList().FindIndex(x => pBank.path == x.path);
            if(lIdx != -1)
            {
                if (_Banks[lIdx].isLoaded)
                    return;

                _Banks[lIdx].isLoaded = true;
                RuntimeManager.LoadBank(_Banks[lIdx].path);
                _LoadedBanks.Add(_Banks[lIdx]);
            }
        }

        public void LoadBankGroup(FMODBank[] pBanks)
        {
            int lLength = pBanks.Length;
            for (int i = 0; i < lLength; i++)
                LoadSpecificBank(pBanks[i]);
        }

        public void UnloadSpecificBank(FMODBank pBank)
        {
            if (_LoadedBanks.Count == 0)
                return;

            int lIdx = _LoadedBanks.FindIndex(x => x.path == pBank.path);
            if(lIdx != -1)
            {
                _LoadedBanks[lIdx].isLoaded = false;
                RuntimeManager.UnloadBank(_LoadedBanks[lIdx].path);
                _LoadedBanks.RemoveAt(lIdx);
            }
        }

        public void UnloadBankGroup(FMODBank[] pBanks)
        {
            int lLength = pBanks.Length;
            for (int i = 0; i < lLength; i++)
                UnloadSpecificBank(pBanks[i]);
        }

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                SceneManager.activeSceneChanged -= LoadSceneBank;

                _Instance = null;
            }
        }
    }
}
