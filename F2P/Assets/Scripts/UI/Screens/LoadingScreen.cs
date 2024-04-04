using System;
using TMPro;
using UnityEngine;

// Author (CR): Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        // const
        private const char PERCENT_SIGN = '%';

        [SerializeField] private TextMeshProUGUI _LoadingTxt = null;

        // Variables
        private LoadManager _LoadManager = null;

        private void Start()
        {
            _LoadManager = LoadManager.GetInstance();
            _LoadManager.OnLoadingProgressed += UpdateProgress;
        }

        private void UpdateProgress(float pProgress) => _LoadingTxt.text = pProgress.ToString() + PERCENT_SIGN;

        private void OnDestroy() 
        {
            _LoadManager.OnLoadingProgressed -= UpdateProgress;
            _LoadManager = null; 
        }

    }
}
