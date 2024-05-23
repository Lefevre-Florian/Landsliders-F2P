using Com.IsartDigital.F2P.FileSystem;
using Com.IsartDigital.F2P.UI.Screens;

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
        private const string TRACKER_BIOME_UNLOCKED_NAME = "biomeUpgradeUnlocked";
        private const string TRACKER_TOTAL_BIOME_UPGRADED_NAME = "numberOfUpgradePerformed";

        private const string TRACKER_TOTAL_UPGRADE_PARAMETER = "numberOfUpgrade";
        private const string TRACKER_TOTAL_PLAYTIME = "timeInHourMinute";

        private const string TRACKER_BIOME_NAME_PARAMETER = "biomeType";
        #endregion

        private const string CMD_QUERY = "SELECT name, description, fragment FROM BIOME WHERE id = ";
        private const string CMD_UPGRADE_QUERY = "SELECT id, name, description, fragment FROM BIOME WHERE id = (SELECT fk_upgrade FROM BIOME WHERE id = ";

        [Header("UI-Elements")]
        [SerializeField] private RectTransform _DefaultStateOverlay = null;
        [SerializeField] private RectTransform _UpgradeStateOverlay = null;
        [SerializeField] private RectTransform _LockedStateOverlay = null;

        [Header("UI - State : Default")]
        [SerializeField] private Image _ExplicationIcon = null;
        [SerializeField] private TextMeshProUGUI _FragReqLabel = null;

        // Variables
        private Texture2D _Texture = null;

        private string _Name = "";
        private string _Description = "";

        private int _FragmentRequired = 0;

        private int _ID = 0;

        private UpgradeScreen _UpgradeScreen = null;
        private ConsentAskScreen _ConsentScreen = null;

        private bool _Loaded = false;

        // Get / Set
        public Vector2 ImageSize { get { return GetComponent<RectTransform>().rect.size; } }

        private void OnEnable()
        {
            if (_Loaded)
                Draw();
        }

        public void Enable(int pId, Texture2D pRenderer, Sprite pExplication, UpgradeScreen pUpgradeScreen, ConsentAskScreen pConsentScreen)
        {
            _ID = pId;

            List<object> lResult = DatabaseManager.GetInstance().GetRowWhere(CMD_QUERY, _ID);

            _Name = lResult[0].ToString();
            _Description = lResult[1].ToString();
            _FragmentRequired = Convert.ToInt32(lResult[2]);

            _Texture = pRenderer;
            if (pExplication == null)
                _ExplicationIcon.gameObject.SetActive(false);
            else
                _ExplicationIcon.sprite = pExplication;

            GetComponent<RawImage>().texture = _Texture;

            Draw();

            _UpgradeScreen = pUpgradeScreen;
            _ConsentScreen = pConsentScreen;

            _Loaded = true;
        }

        #region Upgrade
        public void RequestUpgrade()
        {
            _ConsentScreen.Open();
            _ConsentScreen.OnValidate += Upgrade;
            _ConsentScreen.OnCanceled += ClearUpgrade;
        }

        private void ClearUpgrade()
        {
            _ConsentScreen.OnValidate -= Upgrade;
            _ConsentScreen.OnCanceled -= ClearUpgrade;
        }
        
        private void Upgrade()
        {
            ClearUpgrade();

            DatabaseManager lDatabase = DatabaseManager.GetInstance();
            List<object> lResult = lDatabase.GetRow(CMD_UPGRADE_QUERY + _ID + ")");

            int lOldID = _ID;

            _ID = Convert.ToInt32(lResult[0]);
            _Name = lResult[1].ToString();
            _Description = lResult[2].ToString();
            _FragmentRequired = Convert.ToInt32(lResult[3]);

            Save.data.cards[Save.data.cards.ToList().IndexOf(lOldID)] = _ID;
            lDatabase.WriteDataToSaveFile();

            // Track : Biome upgraded
            DataTracker.GetInstance().SendAnalytics(TRACKER_BIOME_UNLOCKED_NAME, new Dictionary<string, object>() { { TRACKER_BIOME_NAME_PARAMETER, _Name } });

            // Track : Total number of biome upgraded
            Save.data.numberOfUpgrade += 1;
            TimeSpan lDuration = Save.data.totalPlaytime + (DateTime.UtcNow - Save.data.startTime).Duration();

            DataTracker.GetInstance().SendAnalytics(TRACKER_TOTAL_BIOME_UPGRADED_NAME, 
                                                    new Dictionary<string, object>() { { TRACKER_TOTAL_UPGRADE_PARAMETER, Save.data.numberOfUpgrade },
                                                                                       { TRACKER_TOTAL_PLAYTIME, lDuration.Hours + ":" + lDuration.Minutes} });

            Draw();
            _UpgradeScreen.Open();
            _UpgradeScreen.SetContent(_Description, _Texture);
        } 
        #endregion

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

        private void OnDestroy() => ClearUpgrade();
    }
}
