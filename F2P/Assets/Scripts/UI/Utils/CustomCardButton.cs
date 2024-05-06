using Com.IsartDigital.F2P.FileSystem;

using System;
using System.Linq;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class CustomCardButton : MonoBehaviour
    {
        #region Tracking
        private const string TRACKER_NAME = "biomeUpgradeUnlocked";
        private const string TRACKER_BIOME_PARAMETER = "biomeType";
        #endregion

        private const string CMD_QUERY = "SELECT name, description, fragment FROM BIOME WHERE id = ";
        private const string CMD_UPGRADE_QUERY = "SELECT id, name, description, fragment FROM BIOME WHERE id = (SELECT fk_upgrade FROM BIOME WHERE id = ";

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

        public void Enable(int pId)
        {
            _ID = pId;

            List<object> lResult = DatabaseManager.GetInstance().GetRowWhere(CMD_QUERY, _ID);

            _Name = lResult[0].ToString();
            _Description = lResult[1].ToString();
            _FragmentRequired = Convert.ToInt32(lResult[2]);

            _3DModelRenderer = TexturePhotographer.GetInstance();
            _Image = GetComponent<RawImage>();

            _Image.texture = _3DModelRenderer.CreateTextureBiome(ImageSize, 
                                                                 Save.data.cardPrefabs[Save.data.cards.ToList().IndexOf(pId)].transform.GetChild(0).gameObject,
                                                                 new Vector2(.5f, .5f));

            Draw();

            _Loaded = true;
        }

        public void Upgrade()
        {
            DatabaseManager lDatabase = DatabaseManager.GetInstance();
            List<object> lResult = lDatabase.GetRow(CMD_UPGRADE_QUERY + _ID + ")");

            int lOldID = _ID;

            _ID = Convert.ToInt32(lResult[0]);
            _Name = lResult[1].ToString();
            _Description = lResult[2].ToString();
            _FragmentRequired = Convert.ToInt32(lResult[3]);

            Save.data.cards[Save.data.cards.ToList().IndexOf(lOldID)] = _ID;
            lDatabase.WriteDataToSaveFile();

            DataTracker.GetInstance().SendAnalytics(TRACKER_NAME, new Dictionary<string, object>() { { TRACKER_BIOME_PARAMETER, _Name } });
            Draw();
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
