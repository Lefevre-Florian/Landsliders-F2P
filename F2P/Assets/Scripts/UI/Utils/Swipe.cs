using System;

using UnityEngine;
using UnityEngine.Device;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class Swipe : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private Transform _SwipeContainer = null;
        [Space(5)]
        [SerializeField] private Screen[] _LockScreen = null;

        [Header("Settings")]
        [SerializeField][Range(.1f, .8f)] private float _SwipeMinPercent = .25f;

        // Variables
        private float _StartPosition = 0f;
        private int _CurrentScreenIdx = 0;

        private Action _Action = null;
        
        private Screen[] _ScreenNavigation = null;

        private void Start()
        {
            int lLength = _LockScreen.Length;
            for (int i = 0; i < lLength; i++)
            {
                _LockScreen[i].OnScreenClosed += EnableSwipe;
                _LockScreen[i].OnScreenOpened += DisableSwipe;
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
            int lLength = _LockScreen.Length;
            for (int i = 0; i < lLength; i++)
            {
                _LockScreen[i].OnScreenClosed -= EnableSwipe;
                _LockScreen[i].OnScreenOpened -= DisableSwipe;
            }
        }
    }
}
