using com.isartdigital.f2p.gameplay.manager;
using System;
using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomePlayerContact : MonoBehaviour
    {
        [Header("Architecture")]
        [SerializeField] private UnityEvent _OnPlayerCollision = null;

        // Variables
        private Player _Player = null;

        private Vector2 _GridPosition;

        private void Start()
        {
            _GridPosition = GridManager.GetInstance()
                                       .GetGridCoordinate(transform.position);

            _Player = Player.GetInstance();
            GameManager.PlayerMoved.AddListener(ComputeCollision);
        }

        private void ComputeCollision()
        {
            // Collision
            if (_GridPosition == _Player.GridPosition)
                _OnPlayerCollision?.Invoke();
        }

        private void OnDestroy()
        {
            _Player = null;
            GameManager.PlayerMoved.RemoveListener(ComputeCollision);
        }
    }
}
