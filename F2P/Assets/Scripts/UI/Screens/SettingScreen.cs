using Com.IsartDigital.F2P.FileSystem;
using Com.IsartDigital.F2P.Sound;

using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.UI
{
    public class SettingScreen : Screen
    {
        [Header("UI - Sliders")]
        [SerializeField] private Slider _SFXSlider = null;
        [SerializeField] private Slider _MusicSlider = null;

        [Header("UI - Toggle")]
        [SerializeField] private CustomToggle _GlobalSoundToggle = null;

        // Variables
        private bool _WasModified = false;

        private SoundManager _SoundManager = null;

        private void Start()
        {
            _SoundManager = SoundManager.GetInstance();

            _MusicSlider.onValueChanged.AddListener(MusicVolumeChanged);
            _SFXSlider.onValueChanged.AddListener(SFXVolumeChanged);
            _GlobalSoundToggle.onToggleChanged.AddListener(SoundStatusChanged);

            if(Save.data != null)
            {
                _MusicSlider.value = Save.data.musicVolume;
                _SFXSlider.value = Save.data.sfxVolume;

                _GlobalSoundToggle.SetToggle(Save.data.soundStatus);
            }
        }

        private void MusicVolumeChanged(float pValue)
        {
            VolumeSaveWrapper(pValue, ref Save.data.musicVolume);

            _SoundManager.UpdateVolume(SoundType.MUSIC);
        }

        private void SFXVolumeChanged(float pValue)
        {
            VolumeSaveWrapper(pValue, ref Save.data.sfxVolume);

            _SoundManager.UpdateVolume(SoundType.VFX);
        }

        private void SoundStatusChanged(bool pStatus)
        {
            Save.data.soundStatus = pStatus;
            _WasModified = true;

            _SoundManager.UpdateVolume(SoundType.GLOBAL);
        }

        private void VolumeSaveWrapper(float pValue, ref float pVar)
        {
            pVar = pValue;
            _WasModified = true;
        }

        private void OnDisable()
        {
            if (_WasModified && Save.data != null)
            {
                DatabaseManager.GetInstance()
                               .WriteDataToSaveFile();
                _WasModified = false;
            }
        }

        private void OnDestroy()
        {
            _MusicSlider.onValueChanged.RemoveListener(MusicVolumeChanged);
            _SFXSlider.onValueChanged.RemoveListener(SFXVolumeChanged);
            _GlobalSoundToggle.onToggleChanged.RemoveListener(SoundStatusChanged);

            _MusicSlider = _SFXSlider = null;
            _GlobalSoundToggle = null;
        }
    }
}
