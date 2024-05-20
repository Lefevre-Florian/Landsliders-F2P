using FMOD.Studio;
using FMODUnity;

using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Sound
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private EventReference _SoundReference = default;

        // Variables
        private EventInstance _SoundInstance = default;

        private bool _InstancePlaying = false;
        private PLAYBACK_STATE _InstanceState = PLAYBACK_STATE.STOPPED;

        // Events
        public event Action OnSoundEnded;
        public event Action OnSoundStarted;

        private void Update()
        {
            if (!_InstancePlaying)
                return;

            _SoundInstance.getPlaybackState(out _InstanceState);
            if (_InstanceState == PLAYBACK_STATE.STOPPED)
            {
                _InstancePlaying = false;

                StopLoop();
                PlaySFXLooping();
            }
        }

        public void PlaySFXOnShot() => RuntimeManager.PlayOneShot(_SoundReference);

        public void PlaySFXLooping()
        {
            if (_InstancePlaying)
                return;

            _InstancePlaying = true;
            _SoundInstance = RuntimeManager.CreateInstance(_SoundReference);
            _SoundInstance.start();

            OnSoundStarted?.Invoke();
        }

        public void StopSFXLoopingImmediate()
        {
            _SoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            StopLoop();
        }

        public void StopSFXLoopingFade()
        {
            _SoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            StopLoop();
        }

        private void StopLoop()
        {
            if (!_InstancePlaying)
                return;

            _SoundInstance.release();
            _InstancePlaying = false;

            OnSoundEnded?.Invoke();
        }

        private void OnDestroy()
        {
            if (_InstanceState != PLAYBACK_STATE.STOPPED)
                _SoundInstance.release();
            _SoundInstance = default;
        }
    }
}
