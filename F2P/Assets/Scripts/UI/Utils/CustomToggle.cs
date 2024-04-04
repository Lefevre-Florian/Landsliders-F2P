using System;
using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class CustomToggle : MonoBehaviour
    {
        [Header("Toggle")]
        [SerializeField] private Sprite _IconOn = null;
        [SerializeField] private Sprite _IconOff = null;

        [Header("UI")]
        [SerializeField] private bool _IsOn = true;

        // Variables
        private Image _IconComponent = null;

        private Button _ButtonComponents = null;

        #if UNITY_EDITOR
        // Debug
        private bool _EditorIsOn = true;
        #endif

        // Events
        private event Action<bool> OnToggleChanged;

        private void Start()
        {
            _IconComponent = GetComponent<Image>();
            _ButtonComponents = GetComponent<Button>();

            _ButtonComponents.onClick.AddListener(UpdateToggle);
        }
        
        private void UpdateToggle()
        {
            _IsOn = !_IsOn;
            if (_IsOn)
                _IconComponent.sprite = _IconOn;
            else
                _IconComponent.sprite = _IconOff;

            OnToggleChanged?.Invoke(_IsOn);
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            // Debug
            if(_EditorIsOn != _IsOn
               && (_IconOn != null && _IconOff != null))
            {
                if (_IsOn)
                    GetComponent<Image>().sprite = _IconOn;
                else
                    GetComponent<Image>().sprite = _IconOff;
                _EditorIsOn = _IsOn;
            }
        }
        #endif

        private void OnDestroy()
        {
            _ButtonComponents.onClick.RemoveListener(UpdateToggle);
            _ButtonComponents = null;

            _IconComponent = null;
        }
    }
}
