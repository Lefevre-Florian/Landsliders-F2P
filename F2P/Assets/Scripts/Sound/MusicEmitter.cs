using FMOD.Studio;
using FMODUnity;

using System.Collections;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Sound
{
    public class MusicEmitter : MonoBehaviour
    {
        private const string MUSIC_BY_SCENE_FMOD_PARAMETER = "SCENE";

        [SerializeField] private EventReference _MusicEvent = default;
        [SerializeField] private MusicType _Type;

        [Space(5)]
        [SerializeField] private string _FadeParameter = "";

        [Space(5)]
        [SerializeField] private bool _ImmediateStart = true;

        // Variables
        private EventInstance _Instance = default;

        private Coroutine _Coroutine = null;

        private void Start()
        {
            _Instance = RuntimeManager.CreateInstance(_MusicEvent);
            _Instance.setParameterByName(MUSIC_BY_SCENE_FMOD_PARAMETER, (int)_Type);

            if(_ImmediateStart)
                _Instance.start();
        }

        public void SetFade(float pDuration)
        {
            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
            _Coroutine = StartCoroutine(PerformFade(pDuration));
        }

        public IEnumerator PerformFade(float pDuration)
        {
            float lStep = 1f / pDuration;
            float t = 0;

            _Instance.getParameterByName(_FadeParameter, out t);

            float lTarget = t == 0f ? 1f : 0f;
            if(lTarget == 0f)
            {
                while (t < 1f)
                {
                    t += Time.deltaTime * lStep;

                    _Instance.setParameterByName(_FadeParameter, t);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (t > 0f)
                {
                    t -= Time.deltaTime * lStep;

                    _Instance.setParameterByName(_FadeParameter, t);
                    yield return new WaitForEndOfFrame();
                }
            }

            _Instance.setParameterByName(_FadeParameter, lTarget);

            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
            _Coroutine = null;
        }

        public void SetImmediateFade(int pValue) => _Instance.setParameterByName(_FadeParameter, pValue);

        public void Play() => _Instance.start();

        public void Stop() => _Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        private void OnDestroy()
        {
            if (_Instance.isValid())
                _Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            StopAllCoroutines();
            _Coroutine = null;
        }
    }

    public enum MusicType
    {
        MENU = 0,
        GAMEPLAY = 1
    }
}
