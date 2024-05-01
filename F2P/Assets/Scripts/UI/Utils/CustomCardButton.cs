using Com.IsartDigital.F2P.FileSystem.Cache;

using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class CustomCardButton : MonoBehaviour
    {
        [Header("UI-Elements")]
        [SerializeField] private RectTransform _DefaultStateOverlay = null;
        [SerializeField] private RectTransform _UpgradeStateOverlay = null;
        [SerializeField] private RectTransform _LockedStateOverlay = null;

        // Variables
        private RawImage _Image = null;

        private BiomeCache _Info = null;

        public void Enable(BiomeCache pInfo = null)
        {   _Image = GetComponent<RawImage>();

            _Info = pInfo;
        }

        private void OnDestroy()
        {

        }
    }
}
