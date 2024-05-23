using Com.IsartDigital.F2P.Sound;
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class Titlecard : Screen
    {
        [Header("Scene management")]
        [SerializeField] private bool _IsUsingLoadingScreen = false;
        [SerializeField] private int _GameBuildIDX = 0;

        [Header("Screen")]
        [SerializeField] private TextMeshProUGUI _ExpLevelLabel = null;

        [Header("Sound")]
        [SerializeField] private MusicEmitter _Music = null;

        private void Start()
        {
            // Reset timescale (in case)
            Time.timeScale = 1f;

            UpdateExp();

            Save.OnDataUpdated += UpdateExp;
        }

        public override void Open()
        {
            base.Open();
            if(_Music != null)
                _Music.SetImmediateFade(0);
        }

        public void Play()
        {
            Save.data.totalGame += 1;

            if (!_IsUsingLoadingScreen)
                SceneManager.LoadScene(_GameBuildIDX);
            else
                LoadManager.GetInstance().StartLoading(_GameBuildIDX);
        }

        private void UpdateExp() => _ExpLevelLabel.text = Save.data.exp.ToString();

        private void OnDestroy() => Save.OnDataUpdated -= UpdateExp;

    }
}
