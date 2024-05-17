using System;
using TMPro;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class QuestUiManager : MonoBehaviour
    {
        #region Singleton
        private static QuestUiManager _Instance = null;

        public static QuestUiManager GetInstance()
        {
            if(_Instance == null) 
				_Instance = new QuestUiManager();
            return _Instance;
        }

        private QuestUiManager() : base() {}
        #endregion

        [SerializeField] private TextMeshProUGUI _QuestNameLabel;
        [SerializeField] private TextMeshProUGUI _QuestDescLabel;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }

        public void SetQuestName(string pName) => _QuestNameLabel.text = pName;
        public void SetQuestDesc(string pName) => _QuestDescLabel.text = pName;


        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }

    }
}
