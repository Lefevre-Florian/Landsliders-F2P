using Com.IsartDigital.F2P.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class CustomCardButton : MonoBehaviour
    {
        private const string CMD_QUERY = "SELECT name, description, fragment FROM BIOME WHERE id = ";

        [Header("UI-Elements")]
        [SerializeField] private RectTransform _DefaultStateOverlay = null;
        [SerializeField] private RectTransform _UpgradeStateOverlay = null;
        [SerializeField] private RectTransform _LockedStateOverlay = null;

        [Header("UI - State : Default")]
        [SerializeField] private TextMeshProUGUI _FragReqLabel = null;

        // Variables
        private RawImage _Image = null;

        private string _Name = "";
        private string _Description = "";

        private int _FragmentRequired = 0;

        private int _ID = 0;

        private TexturePhotographer _3DModelRenderer = null;

        private bool _Loaded = false;

        // Get / Set
        public Vector2 ImageSize { get { return GetComponent<RectTransform>().rect.size; } }

        private void OnEnable()
        {
            if (_Loaded)
                Draw();
        }

        public void Enable(int pId, GameObject pBiome = null)
        {
            _ID = pId;

            List<object> lResult = DatabaseManager.GetInstance().GetRowWhere(CMD_QUERY, _ID);

            _Name = lResult[0].ToString();
            _Description = lResult[1].ToString();
            _FragmentRequired = Convert.ToInt32(lResult[2]);

            _3DModelRenderer = TexturePhotographer.GetInstance();
            _Image = GetComponent<RawImage>();
            _Image.texture = _3DModelRenderer.CreateTextureBiome(ImageSize, pBiome);

            Draw();

            _Loaded = true;
        }

        private void Draw()
        {
            _UpgradeStateOverlay.gameObject.SetActive(false);
            _LockedStateOverlay.gameObject.SetActive(false);
            _DefaultStateOverlay.gameObject.SetActive(false);

            int lIndex = Save.data.fragments.ToList().FindIndex(x => x.id == _ID);
            if(lIndex != -1)
            {
                if (Save.data.fragments[lIndex].fragment == _FragmentRequired)
                {
                    _UpgradeStateOverlay.gameObject.SetActive(true);
                }
                else
                {
                    _DefaultStateOverlay.gameObject.SetActive(true);
                    _FragReqLabel.text = Save.data.fragments[lIndex].fragment + "/" + _FragmentRequired;
                }
            }
            else
                _LockedStateOverlay.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _3DModelRenderer = null;
        }
    }
}
