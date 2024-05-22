using TMPro;

using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class UpgradeScreen : Screen
    {
        [Header("Utils")]
        [SerializeField] private TextMeshProUGUI _DescrLabel = null;
        [SerializeField] private RawImage _Image = null;

        public void SetContent(string pDescription, Texture2D pTexture2D)
        {
            _DescrLabel.text = pDescription;
            _Image.texture = pTexture2D;
        }
    }
}
