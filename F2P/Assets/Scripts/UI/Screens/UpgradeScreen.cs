using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.UI.Screens
{
    public class UpgradeScreen : Screen
    {
        [Header("Utils")]
        [SerializeField] private TextMeshProUGUI _DescrLabel = null;
        [SerializeField] private RawImage _Image = null;

        // Variables
        private RenderTexture _VirtualTexture = null;

        public override void Open()
        {
            Vector2 lSize = _Image.GetComponent<RectTransform>().rect.size;
            TexturePhotographer lPhotographer = TexturePhotographer.GetInstance();

            _VirtualTexture = lPhotographer.CreateEmptyTexture(lSize);
            _VirtualTexture.Create();

            base.Open();
        }

        public void SetContent(string pDescription, GameObject pModel)
        {
            _DescrLabel.text = pDescription;
            _Image.texture = _VirtualTexture;

            TexturePhotographer.GetInstance().StartRecording(_VirtualTexture, pModel, new Vector2(.5f, .5f));
        }

        public override void Close()
        {
            
            if(_VirtualTexture != null)
            {
                TexturePhotographer.GetInstance()
                                   .StopRecording();

                _VirtualTexture.Release();
                _VirtualTexture = null;
            }
            base.Close();
        }
    }
}
