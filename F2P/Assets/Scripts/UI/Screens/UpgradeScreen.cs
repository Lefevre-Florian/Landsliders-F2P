using Com.IsartDigital.F2P.Cards;
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

            GameObject lObj = TexturePhotographer.GetInstance().StartRecording(_VirtualTexture, pModel, new Vector2(.45f, .45f));
            if (lObj != null)
            {
                CardRenderer lCard = lObj.GetComponent<CardRenderer>();
                lCard.EnableAnimation();
                lCard.FlipCard();
            }
                
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
