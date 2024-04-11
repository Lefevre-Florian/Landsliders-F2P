using com.isartdigital.f2p.gameplay.manager;

using System;

using UnityEngine;
using UnityEngine.Events;

namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomePriority : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField][Min(1)] private int _Priority = 1;
        [SerializeField] private UnityEvent[] _OnTriggered = new UnityEvent[0];

        // Variables
        private GameManager _GameManager = null;

        // Events
        private event UnityAction OnEventExected;

        private void Start()
        {
            _GameManager = GameManager.GetInstance();

            int lLength = _OnTriggered.Length;
            for (int i = 0; i < lLength; i++)
                _OnTriggered[i].AddListener(OnEventExected);
        }

        private void TriggerPriority() => OnEventExected?.Invoke();

        private void OnDestroy()
        {
            _GameManager = null;

            int lLength = _OnTriggered.Length;
            for (int i = 0; i < lLength; i++)
                _OnTriggered[i].RemoveListener(OnEventExected);
            _OnTriggered = null;
        }
    }
}
