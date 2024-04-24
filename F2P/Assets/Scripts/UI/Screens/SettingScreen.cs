using Com.IsartDigital.F2P.Database;

using UnityEngine;

namespace Com.IsartDigital.F2P.UI
{
    public class SettingScreen : Screen
    {
        [Header("UI - Toggle")]
        [SerializeField] private CustomToggle _SoundToggle = null;
        [SerializeField] private CustomToggle _MusicToggle = null;

        // Variables
        private DatabaseManager _SaveSystem; 

        private bool _WasModified = false;

        private void Start()
        {
            _SaveSystem = DatabaseManager.GetInstance();

            _SoundToggle.SetToggle(DatabaseManager.playerSave.soundStatus);
            _MusicToggle.SetToggle(DatabaseManager.playerSave.musicStatus);
        }

        public void UpdateSound(CustomToggle pToggle)
        {
            DatabaseManager.playerSave.soundStatus = pToggle.IsOn;
            CallForModification();
        }

        public void UpdateMusic(CustomToggle pToggle)
        {
            DatabaseManager.playerSave.musicStatus = pToggle.IsOn;
            CallForModification();
        }

        private void CallForModification() => _WasModified = true;

        private void OnDisable()
        {
            if (_WasModified)
            {
                _SaveSystem.WriteDataToSaveFile();
                _WasModified = false;
            }
        }
    }
}
