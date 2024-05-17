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

        // Variables
        private float _StartPosition = 0f;
        private int _CurrentScreenIdx = 0;

        private Action _Action = null;
        
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
            _StartPosition = 0f;
            _Action = DoActionTrack;
        }

        private void SetModeSwipe() => _Action = DoActionSwipe;

        private void DoActionTrack()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _StartPosition = Input.mousePosition.x / UnityEngine.Device.Screen.width;
                SetModeSwipe();
            }
        }

        private void DoActionSwipe()
        {
            if (Input.GetMouseButtonUp(0))
            {
                float lSize = _StartPosition - (Input.mousePosition.x / UnityEngine.Device.Screen.width);
                if(Mathf.Abs(lSize) >= _SwipeMinPercent)
                {
                    int lSide = (int)Mathf.Sign(lSize);
                    if(_CurrentScreenIdx + lSide >= 0 && _CurrentScreenIdx + lSide < _ScreenNavigation.Length)
                    {
                        _ScreenNavigation[_CurrentScreenIdx].Close();
                        _ScreenNavigation[_CurrentScreenIdx + lSide].Open();

                        _CurrentScreenIdx += lSide;
                    }
                }
                SetModeTrack();
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
