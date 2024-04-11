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
        [SerializeField] private UnityEvent _OnTriggered = null;

        // Variables
        private GameManager _GameManager = null;

        private void Start()
        {
            _GameManager = GameManager.GetInstance();
        }

        private void TriggerPriority() => _OnTriggered.Invoke();

        private void OnDestroy()
        {
            _OnTriggered = null;
            _GameManager = null;
        }
    }
}
