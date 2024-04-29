using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class Swipe : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private Screen[] _ScreenNavigation = null;
        [Space(5)]
        [SerializeField] private Screen[] _LockScreen = null;

        [Header("Settings")]
        [SerializeField] private float _SwipeMinSize = 5f;

        // Variables
        private Vector2 _StartPosition = default;
        private int _CurrentScreenIdx = 0;

        private Action _Action = null;

        private void Start()
        {
            int lLength = _LockScreen.Length;
            for (int i = 0; i < lLength; i++)
            {
                _LockScreen[i].OnScreenClosed += EnableSwipe;
                _LockScreen[i].OnScreenOpened += DisableSwipe;
            }

            //Check which screen is open
            lLength = _ScreenNavigation.Length;
            for (int i = 0; i < lLength; i++)
            {
                if (_ScreenNavigation[i].isActiveAndEnabled)
                {
                    _CurrentScreenIdx = i;
                    break;
                }
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
            _StartPosition = Vector2.zero;
            _Action = DoActionTrack;
        }

        private void SetModeSwipe() => _Action = DoActionSwipe;

        private void DoActionTrack()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _StartPosition = Input.mousePosition;
                SetModeSwipe();
            }
        }

        private void DoActionSwipe()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 lDirection = _StartPosition - (Vector2)Input.mousePosition;
                if(lDirection.magnitude >= _SwipeMinSize)
                {
                    int lSide = (int)Mathf.Sign(lDirection.normalized.x);
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
