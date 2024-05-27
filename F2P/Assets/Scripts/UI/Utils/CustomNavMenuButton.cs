using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class CustomNavMenuButton : MonoBehaviour
    {
        [SerializeField] private Screen _Screen = null;
        
        // Variables
        private Image _BtnBackground = null;

        private void Start()
        {
            _BtnBackground = GetComponent<Image>();

            _Screen.OnScreenOpened += ShowBackground;
            _Screen.OnScreenClosed += HideBackground;

            if (_Screen.isActiveAndEnabled)
                ShowBackground();
            else
                HideBackground();
        }

        private void ShowBackground() => _BtnBackground.enabled = true;

        private void HideBackground() => _BtnBackground.enabled = false;

        private void OnDestroy()
        {
            if (_Screen == null)
                return;

            _Screen.OnScreenOpened -= ShowBackground;
            _Screen.OnScreenClosed -= HideBackground;
        }
    }
}
