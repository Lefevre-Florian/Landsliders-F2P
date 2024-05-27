using Com.IsartDigital.F2P.Sound;

using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class Swipe : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private Transform _SwipeContainer = null;
        [SerializeField] private Transform _LockContainer = null;

        [Header("Settings")]
        [SerializeField][Range(.1f, .8f)] private float _SwipeMinPercent = .25f;
        [SerializeField][Range(.1f, 1f)] private float _SwipeDuration = .25f;

        [Header("Sound")]
        [SerializeField] private SoundEmitter _SoundEmitter = null;

        // Variables
        private float _InputStart = 0f;
        private int _CurrentScreenIdx = 0;

        private Action _Action = null;

        private float _ElapsedTime = 0f;
        private float _Speed = 0f;

        private Vector3 _InitialPosition = default;
        private Vector3 _EndPosition = default;

        private int _Direction = 0;
        
        private Screen[] _ScreenNavigation = null;
        private Screen[] _LockScreens = null;

        private void Start()
        {
            int lLength = _LockContainer.childCount;
            if(lLength > 0)
            {
                _LockScreens = new Screen[lLength];
                for (int i = 0; i < lLength; i++)
                {
                    _LockScreens[i] = _LockContainer.GetChild(i).GetComponent<Screen>();
                    _LockScreens[i].OnScreenClosed += EnableSwipe;
                    _LockScreens[i].OnScreenOpened += DisableSwipe;
                }
            }

            _Speed = 1f / _SwipeDuration;

            //Check which screen is open
            lLength = _SwipeContainer.childCount;
            _ScreenNavigation = new Screen[lLength];

            for (int i = 0; i < lLength; i++)
            {
                _ScreenNavigation[i] = _SwipeContainer.GetChild(i)
                                                      .GetComponent<Screen>();
                if (_ScreenNavigation[i].isActiveAndEnabled)
                    _CurrentScreenIdx = i;
            }

            SetModeTrack();
        }

        private void Update()
        {
            if (_Action != null)
                _Action();
        }

        private void SetModeVoid() => _Action = null;

        private void SetModeTrack()
        {
            _InputStart = 0f;
            _Action = DoActionTrack;
        }

        private void SetModeSwipe() => _Action = DoActionSwipe;

        private void DoActionTrack()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _InputStart = Input.mousePosition.x / UnityEngine.Device.Screen.width;
                SetModeSwipe();
            }
        }

        private void DoActionSwipe()
        {
            if (Input.GetMouseButtonUp(0))
            {
                float lSize = _InputStart - (Input.mousePosition.x / UnityEngine.Device.Screen.width);
                if(Mathf.Abs(lSize) >= _SwipeMinPercent)
                {
                    _Direction = (int)Mathf.Sign(lSize);
                    if(_CurrentScreenIdx + _Direction >= 0 && _CurrentScreenIdx + _Direction < _ScreenNavigation.Length)
                    {
                        _ScreenNavigation[_CurrentScreenIdx].Close();
                        _ScreenNavigation[_CurrentScreenIdx + _Direction].Open();
                        _CurrentScreenIdx += _Direction;

                        _SoundEmitter.PlaySFXOnShot();
                        SetModeAnimate();
                    }
                    else
                    {
                        SetModeTrack();
                    }
                }
                else
                {
                    SetModeTrack();
                }
            }
        }

        private void SetModeAnimate()
        {
            _ElapsedTime = 0f;

            _EndPosition = _ScreenNavigation[_CurrentScreenIdx].transform.position;
            _InitialPosition = _EndPosition + Vector3.right * _Direction * UnityEngine.Screen.width;

            _ScreenNavigation[_CurrentScreenIdx].transform.position = _InitialPosition;

            _Action = DoActionAnimate;
        }

        private void DoActionAnimate()
        {
            _ElapsedTime += Time.deltaTime * _Speed;
            if(_ElapsedTime >= 1f)
            {
                _ElapsedTime = 0f;

                _ScreenNavigation[_CurrentScreenIdx].transform.position = _EndPosition;
                _ScreenNavigation[_CurrentScreenIdx].Open();

                SetModeTrack();
            }
            else
            {
                _ScreenNavigation[_CurrentScreenIdx].transform.position = Vector3.Lerp(_InitialPosition, _EndPosition, _ElapsedTime);
            }
        }

        private void EnableSwipe() => SetModeTrack();
        private void DisableSwipe() => SetModeVoid();

        private void OnDestroy()
        {
            if(_LockScreens != null && _LockScreens.Length > 0)
            {
                int lLength = _LockScreens.Length;
                for (int i = 0; i < lLength; i++)
                {
                    _LockScreens[i].OnScreenClosed -= EnableSwipe;
                    _LockScreens[i].OnScreenOpened -= DisableSwipe;
                }

                _LockScreens = null;
            }
        }
    }
}
