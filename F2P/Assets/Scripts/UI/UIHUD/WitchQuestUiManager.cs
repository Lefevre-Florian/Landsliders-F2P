using System;
using TMPro;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class WitchQuestUiManager : MonoBehaviour
    {
        #region Singleton
        private static WitchQuestUiManager _Instance = null;

        public static WitchQuestUiManager GetInstance()
        {
            if(_Instance == null) 
				_Instance = new WitchQuestUiManager();
            return _Instance;
        }

        private WitchQuestUiManager() : base() {}
        #endregion

        [SerializeField] private TextMeshProUGUI _NameLabel;
        [SerializeField] private TextMeshProUGUI _DescLabel;
        [SerializeField] private TextMeshProUGUI _RewardLabel;

        private string _CurrentName;
        private string _CurrentDesc;
        private string _CurrentReward;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }

        public void UpdateText()
        {
            _NameLabel.text = _CurrentName;
            _DescLabel.text = _CurrentDesc;
            _RewardLabel.text = _CurrentReward;
        }

        public void SetQuestName(string pName) => _CurrentName = pName;
        public void SetQuestDesc(string pName) => _CurrentDesc = pName;
        public void SetQuestReward(string pName) => _CurrentReward = pName;

        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }

    }
}
