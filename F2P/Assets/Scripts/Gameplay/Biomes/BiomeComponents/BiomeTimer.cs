using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeTimer : MonoBehaviour
    {
        [Header("Design - Settings")]
        [SerializeField][Range(0, 10)] private int _Timer = 0;
        [SerializeField] private Action _Action = null;

        [Header("Render")]
        [SerializeField] private TextMeshProUGUI _Label = null;

        // Variables
        private int _InternalTimer = 0;

        private GameManager _GameManager = null;

        private void Start()
        {
            _GameManager = GameManager.GetInstance();

            ///TODO : connect to event turn passed to update timer
        }

        private void ClockTicking()
        {
            --_InternalTimer;
            if (_InternalTimer == 0)
                _InternalTimer = _Timer;

            if (_Label != null)
                _Label.text = _InternalTimer.ToString();
        }

        private void OnDestroy()
        {
            _GameManager = null;
        }

    }
}
