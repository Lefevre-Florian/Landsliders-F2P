using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE
{
    public class TutorialManager : MonoBehaviour
    {
        #region Singleton
        private static TutorialManager _Instance = null;

        public static TutorialManager GetInstance()
        {
            if(_Instance == null) 
				_Instance = new TutorialManager();
            return _Instance;
        }

        private TutorialManager() : base() {}
        #endregion

        [Header("FTUE Flow")]
        [SerializeField][Range(1, 3)] private int _Phase = 1;

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

        }


        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _Instance = null;   
            }
        }
    }
}
