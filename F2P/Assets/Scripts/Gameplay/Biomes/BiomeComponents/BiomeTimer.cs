using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeTimer : MonoBehaviour
    {
        [Serializable]
        public struct TupleActionTime
        {
            public float time;
            public UnityEvent method;
        }


        [Header("Design - Settings")]
        [SerializeField] private bool _AlwaysStart = true;
        [SerializeField][Range(0, 10)] private int _Timer = 0;
        [SerializeField] private TupleActionTime[] _TimedActions = new TupleActionTime[0];

        [Header("Render")]
        [SerializeField] private TextMeshProUGUI _Label = null;

        // Variables
        private int _InternalTimer = 0;

        private GameManager _GameManager = null;

        private void Start()
        {
            _InternalTimer = _Timer;

            //GetComponent<TEMPCard>().OnPlaced += Enable;
            Enable();
        }

        private void Enable()
        {
            _GameManager = GameManager.GetInstance();
            if (_AlwaysStart)
                _GameManager.OnTurnPassed += ClockTicking;
        }

        public void StartTicking()
        {
            if (_AlwaysStart)
                return;
            _GameManager.OnTurnPassed += ClockTicking;
        }

        public void StopTicking() => _GameManager.OnTurnPassed -= ClockTicking;

        private void ClockTicking()
        {
            --_InternalTimer;

            int lLength = _TimedActions.Length;
            for (int i = 0; i < lLength; i++)
            {
                if (_TimedActions[i].time == _InternalTimer)
                    _TimedActions[i].method.Invoke();        
            }

            if (_InternalTimer == 0)
                _InternalTimer = _Timer;

            if (_Label != null)
                _Label.text = _InternalTimer.ToString();
        }

        private void OnDestroy()
        {
            //GetComponent<TEMPCard>().OnPlaced -= Enable;
            StopTicking();
            _GameManager = null;
        }
    }
}
