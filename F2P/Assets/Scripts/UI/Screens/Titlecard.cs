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

        [Header("Application management")]
        [SerializeField][Range(15, 120)] private int _TargetedFrameRate = 30;

        [Header("Screen")]
        [SerializeField] private TextMeshProUGUI _ExpLevelLabel = null;

        private void Start()
        {
            Application.targetFrameRate = _TargetedFrameRate;

            // Reset timescale (in case)
            Time.timeScale = 1f;

            _ExpLevelLabel.text = Save.data.exp.ToString();
        }

        public void Play()
        {
            if (!_IsUsingLoadingScreen)
                SceneManager.LoadScene(_GameBuildIDX);
            else
                LoadManager.GetInstance().StartLoading(_GameBuildIDX);
        }
    }
}