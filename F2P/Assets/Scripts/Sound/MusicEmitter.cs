using FMOD.Studio;
using FMODUnity;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Sound
{
    public class MusicEmitter : MonoBehaviour
    {
        [SerializeField] private EventReference _MusicEvent = default;

        // Variables
        private EventInstance _Instance = default;

        private void Start()
        {
            _Instance = RuntimeManager.CreateInstance(_MusicEvent);
            _Instance.start();
        }

        private void OnDestroy()
        {
            if (_Instance.isValid())
                _Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
