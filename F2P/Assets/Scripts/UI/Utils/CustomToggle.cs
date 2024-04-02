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
        [SerializeField] private bool _IsOn = false;

        // Variables
        private Image _IconComponent = null;

        private Button _ButtonComponents = null;

        // Events
        private event Action<bool> OnToggleChanged;

        private void Start()
        {
            _IconComponent = GetComponent<Image>();
            _ButtonComponents = GetComponent<Button>();

            _ButtonComponents.onClick.AddListener(ChangeIcon);

            ChangeIcon();
        }
        
        private void ChangeIcon()
        {
            _IsOn = !_IsOn;

            if (_IsOn)
                _IconComponent.sprite = _IconOn;
            else
                _IconComponent.sprite = _IconOff;

            OnToggleChanged?.Invoke(_IsOn);
        }

        //private void OnValidate() => ChangeIcon();

        private void OnDestroy()
        {
            _ButtonComponents.onClick.RemoveListener(ChangeIcon);
            _ButtonComponents = null;

            _IconComponent = null;
        }
    }
}
