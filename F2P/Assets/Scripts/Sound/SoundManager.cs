using FMOD.Studio;
using FMODUnity;

using System;
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
        
        [Serializable]
        public class VCAType
        {
            [SerializeField] private string _VCAPath = "vca:/";
            [SerializeField] private SoundType _Type = SoundType.VFX;

            public string VCAPath { get { return _VCAPath; } }
            public SoundType Type { get { return _Type; } }
        }

        [Serializable]
        public class FMODBankScene : FMODBank
        {
            [Header("Scene")]
            [SerializeField] private int[] _ScenesLinked = new int[] { 0};

            public int[] ScenesLinked { get { return _ScenesLinked; } }
        }

        [Header("Banks")]
        [SerializeField] private FMODBank[] _AlwaysLoadedBank = default;
        [SerializeField] private FMODBankScene[] _Banks = default;

        [Header("VCA")]
        [SerializeField] private VCAType[] _VCATypes = null;

        // Variables
        private List<FMODBankScene> _LoadedBanks = new List<FMODBankScene>();

        private Dictionary<SoundType, List<VCA>> _VCAs = new Dictionary<SoundType, List<VCA>>();

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
            {
                _AlwaysLoadedBank[i].isLoaded = true;
                RuntimeManager.LoadBank(_AlwaysLoadedBank[i].path);
            }
                
            int lID = 0;
            lLength = _Banks.Length;
            for (int i = 0; i < lLength; i++)
            {
                lID++;
                _Banks[i].SetBankID(lID);
                RuntimeManager.UnloadBank(_Banks[i].path);
            }

            if(_VCATypes != null && _VCATypes.Length != 0)
            {
                lLength = _VCATypes.Length;
                for (int i = 0; i < lLength; i++)
                    LoadVCA(_VCATypes[i]);

                if(Save.data != null)
                {
                    UpdateVolume(SoundType.VFX, Save.data.sfxVolume * Convert.ToInt32(Save.data.soundStatus));
                    UpdateVolume(SoundType.MUSIC, Save.data.musicVolume * Convert.ToInt32(Save.data.soundStatus));
                }
            }

            SceneManager.activeSceneChanged += LoadSceneBank;

            #if UNITY_EDITOR
            LoadBankGroup(_Banks.ToList<FMODBankScene>().FindAll(x => x.ScenesLinked.ToList().Contains(SceneManager.GetActiveScene().buildIndex)).ToArray());
            #endif
        }

        private void LoadVCA(VCAType pVCA)
        {
            if (string.IsNullOrEmpty(pVCA.VCAPath))
                return;

            if (_VCAs.ContainsKey(pVCA.Type))
                _VCAs[pVCA.Type].Add(RuntimeManager.GetVCA(pVCA.VCAPath));
            else
                _VCAs.Add(pVCA.Type, new List<VCA>() { RuntimeManager.GetVCA(pVCA.VCAPath) });
        }

        private void LoadSceneBank(Scene pOldScene, Scene pNextScene)
        {
            List<FMODBankScene> lBanks = _Banks.ToList<FMODBankScene>();

            UnloadBankGroup(lBanks.FindAll(x => x.ScenesLinked.ToList().Contains(pOldScene.buildIndex)).ToArray());
            LoadBankGroup(lBanks.FindAll(x => x.ScenesLinked.ToList().Contains(pNextScene.buildIndex)).ToArray());
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

        public void UpdateVolume(SoundType pType, float pValue)
        {
            if (_VCAs.Count == 0)
                return;

            int lLength = 0;
            pValue = (float)Math.Round(pValue * 2, 2);

            switch (pType)
            {
                case SoundType.GLOBAL:
                    foreach (KeyValuePair<SoundType, List<VCA>> lItem in _VCAs)
                    {
                        lLength = lItem.Value.Count;
                        for (int i = 0; i < lLength; i++)
                        {
                            if (lItem.Key == SoundType.MUSIC)
                                _VCAs[lItem.Key][i].setVolume(Save.data != null ? Save.data.musicVolume * Convert.ToInt32(Save.data.soundStatus) : pValue);
                            else if(lItem.Key == SoundType.VFX)
                                _VCAs[lItem.Key][i].setVolume(Save.data != null ? Save.data.sfxVolume * Convert.ToInt32(Save.data.soundStatus) : pValue);
                        }
                    }
                    break;

                case SoundType.VFX | SoundType.MUSIC:
                    lLength = _VCAs[pType].Count;
                    print(pType);
                    for (int i = 0; i < lLength; i++)
                        _VCAs[pType][i].setVolume(pValue);
                    break;
            }
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
